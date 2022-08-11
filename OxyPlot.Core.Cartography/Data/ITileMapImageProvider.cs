namespace OxyPlot.Data
{
    public interface ITileMapImageProvider
    {
        /// <summary>
        /// Gets the map tile image.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        OxyImage? GetImage(int x, int y, int zoom);

        /// <summary>
        /// Call this event when the UI thread nned to invalidate the plot.
        /// </summary>
        event EventHandler<EventArgs> InvalidatePlotOnUI;

        /// <summary>
        /// Image converter to handle unsupported file format by OxyPlot, e.g. Jpeg.
        /// <para>Does nothing by default.</para>
        /// </summary>
        Func<byte[], byte[]> ImageConverter { get; init; }
    }
}
