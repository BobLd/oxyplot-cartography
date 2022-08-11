using System.Collections.Concurrent;

namespace OxyPlot.Data
{
    public class LocalTileMapImageProvider : ITileMapImageProvider
    {
        private readonly string _storageLocation = string.Empty;

        public event EventHandler<EventArgs> InvalidatePlotOnUI;

        private readonly ConcurrentDictionary<string, OxyImage?> images = new ConcurrentDictionary<string, OxyImage?>();

        public LocalTileMapImageProvider(string storageLocation)
        {
            _storageLocation = storageLocation;
            if (!Directory.Exists(_storageLocation))
            {
                throw new DirectoryNotFoundException(_storageLocation);
            }
        }

        /// <inheritdoc/>
        public Func<byte[], byte[]> ImageConverter { get; init; } = new Func<byte[], byte[]>(i => i);

        /// <inheritdoc/>
        public OxyImage? GetImage(int x, int y, int zoom)
        {
            string tileKey = GetTileKey(x, y, zoom);

            if (images.TryGetValue(tileKey, out OxyImage? img))
            {
                return img;
            }

            string pathToImage = Path.Combine(_storageLocation, tileKey);
            if (File.Exists(pathToImage))
            {
                using (var ms = new MemoryStream())
                using (var fs = new FileStream(pathToImage, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                    img = ConvertToOxyImage(ms.ToArray());
                }

                if (!images.TryAdd(tileKey, img))
                {
                    throw new Exception("GetImage: Could not add image");
                }
                return img;
            }

            return null;
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

        private static string GetTileKey(int x, int y, int zoom)
        {
            return $"{zoom}-{x}-{y}";
        }
    }
}
