namespace OxyPlot.Data
{
    public interface IHttpTileMapImageProvider : ITileMapImageProvider, IDisposable
    {
        /// <summary>
        /// Gets or sets the user agent used for requests
        /// </summary>
        /// <value>The user agent</value>
        string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the max number of simultaneous downloads
        /// </summary>
        /// <value>The max number of download.</value>
        int MaxNumberOfDownloads { get; set; }

        /// <summary>
        /// Gets or sets the URL
        /// </summary>
        /// <value>The URL</value>
        string Url { get; init; }
    }
}
