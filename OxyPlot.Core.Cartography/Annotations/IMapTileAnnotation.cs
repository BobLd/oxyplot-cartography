namespace OxyPlot.Annotations
{
    /// <summary>
    /// Interface that defines annotation that shows a tile based map.
    /// </summary>
    public interface IMapTileAnnotation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the tile grid is visible.
        /// </summary>
        bool IsTileGridVisible { get; set; }

        /// <summary>
        /// Gets or sets the color of the tile grid.
        /// </summary>
        OxyColor TileGridColor { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the tile grid.
        /// </summary>
        int TileGridThickness { get; set; }

        /// <summary>
        /// Gets or sets the copyright notice.
        /// </summary>
        /// <value>The copyright notice.</value>
        string? CopyrightNotice { get; set; }

        /// <summary>
        /// Gets or sets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        int TileSize { get; set; }

        /// <summary>
        /// Gets or sets the min zoom level.
        /// </summary>
        /// <value>The min zoom level.</value>
        int MinZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the max zoom level.
        /// </summary>
        /// <value>The max zoom level.</value>
        int MaxZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        double Opacity { get; set; }

        /// <summary>
        /// Gets or sets the groupname for the Series.
        /// </summary>
        /// <remarks>This groupname may for e.g. be used by the Legend class to group series into separated blocks.</remarks>
        string SeriesGroupName { get; set; }

        /// <summary>
        /// Gets or sets the title of the series.
        /// </summary>
        /// <value>The title that is shown in the legend of the plot.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the series should be rendered in the legend.
        /// </summary>
        bool RenderInLegend { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this series is visible.
        /// </summary>
        bool IsVisible { get; set; }
    }
}
