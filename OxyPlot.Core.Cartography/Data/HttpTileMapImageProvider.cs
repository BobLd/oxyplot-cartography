// --------------------------------------------------------------------------------------------------------------------
// Base on TileMapAnnotation.cs
// https://github.com/oxyplot/oxyplot/blob/release/v2.1.0-Preview1/Source/Examples/ExampleLibrary/Annotations/TileMapAnnotation.cs
// <summary>
//   Provides an annotation that shows a tile based map.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace OxyPlot.Data
{
    public class HttpTileMapImageProvider : IHttpTileMapImageProvider
    {
        private string _fullStoragePath;

        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly ConcurrentDictionary<TileLocation, OxyImage?> _images = new ConcurrentDictionary<TileLocation, OxyImage?>();
        private readonly ConcurrentQueue<TileLocation> _queue = new ConcurrentQueue<TileLocation>();
        private readonly SynchronizationContext _uiSync;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTileMapImageProvider"/> class.
        /// </summary>
        /// <param name="baseStorageLocation"></param>
        /// <param name="uiSynchronizationContext">The UI synchronization context</param>
        public HttpTileMapImageProvider(string baseStorageLocation, SynchronizationContext uiSynchronizationContext)
            : this(uiSynchronizationContext)
        {
            BaseStorageFolder = baseStorageLocation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTileMapImageProvider"/> class.
        /// </summary>
        /// <param name="uiSynchronizationContext">The UI synchronization context</param>
        public HttpTileMapImageProvider(SynchronizationContext uiSynchronizationContext)
        {
            ArgumentNullException.ThrowIfNull(uiSynchronizationContext, nameof(uiSynchronizationContext));

            _uiSync = uiSynchronizationContext;

            MaxNumberOfDownloads = 8;
            UserAgent = "OxyPlot.Core.Cartography";

            _httpClientHandler = new HttpClientHandler
            {
                MaxConnectionsPerServer = MaxNumberOfDownloads,
            };
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }

        /// <summary>
        /// Base storage folder.
        /// </summary>
        public string BaseStorageFolder { get; } = "tile_map_images";

        /// <inheritdoc/>
        public Func<byte[], byte[]> ImageConverter { get; init; } = new Func<byte[], byte[]>(i => i);

        /// <inheritdoc/>
        public event EventHandler<EventArgs> InvalidatePlotOnUI;

        private int _maxNumberOfDownloads;
        /// <inheritdoc/>
        public int MaxNumberOfDownloads
        {
            get
            {
                return _maxNumberOfDownloads;
            }

            set
            {
                _maxNumberOfDownloads = value;
                if (_httpClientHandler != null)
                {
                    _httpClientHandler.MaxConnectionsPerServer = _maxNumberOfDownloads;
                }
            }
        }

        private string _userAgent;
        /// <inheritdoc/>
        public string UserAgent
        {
            get
            {
                return _userAgent;
            }

            set
            {
                _userAgent = value;
                _httpClient?.DefaultRequestHeaders.UserAgent.Clear();
                _httpClient?.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
            }
        }

        private string _url;
        /// <inheritdoc/>
        public string Url
        {
            get
            {
                return _url;
            }

            init
            {
                _url = value;
                var uri = new Uri(_url);
                _fullStoragePath = CartographyHelper.GetStoragePathFromUri(uri);
            }
        }

        /// <inheritdoc/>
        public OxyImage? GetImage(int x, int y, int zoom)
        {
            var tile = new TileLocation()
            {
                X = x,
                Y = y,
                Zoom = zoom
            };

            if (_images.TryGetValue(tile, out OxyImage? img))
            {
                return img;
            }

            string pathToImage = Path.Combine(_fullStoragePath, GetTileKey(x, y, zoom));
            if (File.Exists(pathToImage))
            {
                using (var ms = new MemoryStream())
                using (var fs = new FileStream(pathToImage, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                    img = ConvertToOxyImage(ms.ToArray());
                }

                if (!_images.TryAdd(tile, img))
                {
                    throw new Exception("GetImage: Could not add image");
                }
                return img;
            }

            lock (_queue)
            {
                // 'reserve' an image (otherwise multiple downloads of the same uri may happen)
                _images[tile] = null;
                _queue.Enqueue(new TileLocation()
                {
                    X = x,
                    Y = y,
                    Zoom = zoom,
                });
            }

            Task.Run(() => BeginDownload());
            return null;
        }

        /// <summary>
        /// Starts the next download in the queue.
        /// </summary>
        private async Task BeginDownload()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            try
            {
                if (_queue.TryDequeue(out TileLocation tile))
                {
                    var uri = GetTileUri(tile.X, tile.Y, tile.Zoom);
                    if (string.IsNullOrEmpty(uri))
                    {
                        throw new Exception("uri is null or empty");
                    }

                    using (var httpResponse = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                System.Diagnostics.Debug.WriteLine($"BeginDownload: Success for uri '{uri}'");
                                using (var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                                {
                                    await DownloadCompleted(tile, stream).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"BeginDownload: Error - Received status code '{httpResponse.StatusCode}' with ReasonPhrase '{httpResponse.ReasonPhrase}' for uri '{uri}'");
                                await DownloadFailed(tile).ConfigureAwait(false);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine($"BeginDownload: Operation cancelled for uri '{uri}'");
                            return;
                        }
                        catch (Exception e)
                        {
                            var ie = e;
                            while (ie != null)
                            {
                                System.Diagnostics.Debug.WriteLine(ie.Message);
                                ie = ie.InnerException;
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("BeginDownload: Operation cancelled");
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DownloadFailed(TileLocation tile)
        {
            _uiSync.Send((_) => InvalidatePlotOnUI?.Invoke(this, EventArgs.Empty), null);

            if (!_queue.IsEmpty)
            {
                await BeginDownload().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The download completed, set the image
        /// </summary>
        /// <param name="tile">The tile location</param>
        /// <param name="result">The result</param>
        private async Task DownloadCompleted(TileLocation tile, Stream result)
        {
            try
            {
                if (result == null)
                {
                    return;
                }

                Directory.CreateDirectory(_fullStoragePath);
                var tileKey = GetTileKey(tile.X, tile.Y, tile.Zoom);

                using (var ms = new MemoryStream())
                using (var fs = new FileStream(Path.Combine(_fullStoragePath, tileKey), FileMode.Create, FileAccess.Write))
                {
                    await result.CopyToAsync(ms);
                    ms.WriteTo(fs); // Save to disk
                    _images[tile] = ConvertToOxyImage(ms.ToArray());
                }

                lock (_queue)
                {
                    // Clear old items in the queue, new ones will be added when the plot is refreshed
                    foreach (var queuedUri in _queue)
                    {
                        // Remove the 'reserved' image
                        _images.TryRemove(queuedUri, out _);
                    }
                    _queue.Clear();
                }

                _uiSync.Send((_) => InvalidatePlotOnUI?.Invoke(this, EventArgs.Empty), null);

                if (!_queue.IsEmpty)
                {
                    await BeginDownload().ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private OxyImage ConvertToOxyImage(byte[] image)
        {
            try
            {
                return new OxyImage(ImageConverter(image));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create OxyImage. Make sure the image format is supported by OxyPlot. You must set {nameof(ImageConverter)} if format is not supported.", ex);
            }
        }

        /// <summary>
        /// Gets the tile URI.
        /// </summary>
        /// <param name="x">The tile x.</param>
        /// <param name="y">The tile y.</param>
        /// <param name="zoom">The zoom.</param>
        /// <returns>The uri.</returns>
        private string GetTileUri(int x, int y, int zoom)
        {
            string url = Url.Replace("{X}", x.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
            url = url.Replace("{Y}", y.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
            return url.Replace("{Z}", zoom.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);
        }

        private static string GetTileKey(int x, int y, int zoom)
        {
            return $"{zoom}-{x}-{y}";
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
            _images.Clear();
            _queue.Clear();
        }

        internal struct TileLocation
        {
            public int X { get; init; }
            public int Y { get; init; }
            public int Zoom { get; init; }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Zoom);
            }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                if (obj is TileLocation tile)
                {
                    return X == tile.X && Y == tile.Y && Zoom == tile.Zoom;
                }
                return false;
            }

            public override string ToString()
            {
                return $"{X}, {Y}, {Zoom}";
            }
        }
    }
}
