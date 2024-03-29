﻿// --------------------------------------------------------------------------------------------------------------------
// Base on TileMapAnnotation.cs
// https://github.com/oxyplot/oxyplot/blob/release/v2.1.0-Preview1/Source/Examples/ExampleLibrary/Annotations/TileMapAnnotation.cs
// <summary>
//   Provides an annotation that shows a tile based map.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot.Data;

namespace OxyPlot.Annotations
{
    /// <summary>
    /// Provides an annotation that shows a tile based map.
    /// </summary>
    /// <remarks>The longitude and latitude range of the map is defined by the range of the x and y axis, respectively.</remarks>
    public class MapTileAnnotation : Annotation
    {
        private readonly ITileMapImageProvider _tileMapImageProvider;

        private readonly OxyImage? _loadingImg;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation" /> class.
        /// </summary>
        /// <param name="loadingImage">The image to display when loading the tiles.</param>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(Stream loadingImage, ITileMapImageProvider tileMapImageProvider)
            : this(tileMapImageProvider)
        {
            _loadingImg = new OxyImage(loadingImage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation" /> class.
        /// </summary>
        /// <param name="loadingImage">The image to display when loading the tiles.</param>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(byte[] loadingImage, ITileMapImageProvider tileMapImageProvider)
            : this(tileMapImageProvider)
        {
            _loadingImg = new OxyImage(loadingImage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation" /> class.
        /// </summary>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(ITileMapImageProvider tileMapImageProvider)
        {
            _tileMapImageProvider = tileMapImageProvider;
            _tileMapImageProvider.InvalidatePlotOnUI += (s, e) =>
            {
                try
                {
                    PlotModel.InvalidatePlot(false);
                }
                catch (Exception ex)
                {
                    throw;
                }
            };

            Layer = AnnotationLayer.BelowSeries;
            TileSize = 256;
            MinZoomLevel = 0;
            MaxZoomLevel = 20;
            Opacity = 1.0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile grid is visible. The default is <c>false</c>.
        /// </summary>
        public bool IsTileGridVisible { get; set; }

        /// <summary>
        /// Gets or sets the color of the tile grid. The default is <c>OxyColors.Black</c>.
        /// </summary>
        public OxyColor TileGridColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// Gets or sets the thickness of the tile grid. The default is <c>2</c>.
        /// </summary>
        public int TileGridThickness { get; set; } = 2;

        /// <summary>
        /// Gets or sets the copyright notice.
        /// </summary>
        /// <value>The copyright notice.</value>
        public string? CopyrightNotice { get; set; }

        /// <summary>
        /// Gets or sets the size of the tiles.
        /// </summary>
        /// <value>The size of the tiles.</value>
        public int TileSize { get; set; }

        /// <summary>
        /// Gets or sets the min zoom level.
        /// </summary>
        /// <value>The min zoom level.</value>
        public int MinZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the max zoom level.
        /// </summary>
        /// <value>The max zoom level.</value>
        public int MaxZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        public double Opacity { get; set; }

        /// <summary>
        /// Renders the annotation on the specified context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public override void Render(IRenderContext rc)
        {
            var lon0 = XAxis.ActualMinimum;
            var lon1 = XAxis.ActualMaximum;
            var lat0 = YAxis.ActualMinimum;
            var lat1 = YAxis.ActualMaximum;

            // the desired number of tiles horizontally
            double tilesx = PlotModel.Width / TileSize;

            // calculate the desired zoom level
            var n = tilesx / (((lon1 + 180) / 360) - ((lon0 + 180) / 360));
            var zoom = (int)Math.Round(Math.Log(n) / Math.Log(2));
            if (zoom < MinZoomLevel)
            {
                zoom = MinZoomLevel;
            }

            if (zoom > MaxZoomLevel)
            {
                zoom = MaxZoomLevel;
            }

            double maxXY = Math.Pow(2, zoom); // See https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y

            // find tile coordinates for the corners
            CartographyHelper.LatLonToTile(lat0, lon0, zoom, out double x0, out double y0);
            CartographyHelper.LatLonToTile(lat1, lon1, zoom, out double x1, out double y1);

            double xmax = Math.Max(x0, x1);
            if (xmax > maxXY || double.IsNaN(xmax))
            {
                xmax = maxXY;
            }

            double xmin = Math.Min(x0, x1);
            if (xmin < 0 || double.IsNaN(xmin))
            {
                xmin = 0;
            }

            double ymax = Math.Max(y0, y1);
            if (ymax > maxXY || double.IsNaN(ymax))
            {
                ymax = maxXY;
            }

            double ymin = Math.Min(y0, y1);
            if (ymin < 0 || double.IsNaN(ymin))
            {
                ymin = 0;
            }

            // Add the tiles
            for (var x = (int)xmin; x < xmax; x++)
            {
                for (var y = (int)ymin; y < ymax; y++)
                {
                    var img = GetImage(x, y, zoom);

                    if (img == null)
                    {
                        continue;
                    }

                    // transform from tile coordinates to lat/lon
                    CartographyHelper.TileToLatLon(x, y, zoom, out double latitude0, out double longitude0);
                    CartographyHelper.TileToLatLon(x + 1, y + 1, zoom, out double latitude1, out double longitude1);

                    // transform from lat/lon to screen coordinates
                    var s00 = this.Transform(longitude0, latitude0);
                    var s11 = this.Transform(longitude1, latitude1);

                    var r = OxyRect.Create(s00.X, s00.Y, s11.X, s11.Y);

                    // draw the rectangle
                    if (IsTileGridVisible && TileGridThickness > 0 && !TileGridColor.IsUndefined() && TileGridColor.IsVisible())
                    {
                        rc.DrawRectangle(r, OxyColors.Undefined, TileGridColor, TileGridThickness, EdgeRenderingMode.PreferSpeed);
                    }

                    // draw the image
                    rc.DrawImage(img, r.Left, r.Top, r.Width, r.Height, Opacity, true);
                }
            }

            // draw the copyright notice
            if (!string.IsNullOrEmpty(CopyrightNotice))
            {
                var clippingRectangle = GetClippingRect();
                var p = new ScreenPoint(clippingRectangle.Right - 5, clippingRectangle.Bottom - 5);
                //var textSize = rc.MeasureText(CopyrightNotice, ActualFont, ActualFontSize, ActualFontWeight);
                rc.DrawText(p, CopyrightNotice, OxyColors.Black, ActualFont, ActualFontSize, ActualFontWeight,
                            0, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            }
        }

        private OxyImage? GetImage(int x, int y, int zoom)
        {
            var img = _tileMapImageProvider.GetImage(x, y, zoom);
            return img ?? _loadingImg;
        }
    }
}
