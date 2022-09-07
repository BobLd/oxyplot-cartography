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
    /// <para>
    /// Sets the defaults <see cref="Annotation.Layer"/> to <see cref="AnnotationLayer.BelowSeries"/>.
    /// </para>
    /// </summary>
    /// <remarks>The longitude and latitude range of the map is defined by the range of the x and y axis, respectively.</remarks>
    public class MapTileAnnotation : Annotation, IMapTileAnnotation
    {
        private readonly ITileMapImageProvider _tileMapImageProvider;

        private readonly OxyImage? _loadingImg;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation"/> class.
        /// <para>
        /// Sets the defaults <see cref="Annotation.Layer"/> to <see cref="AnnotationLayer.BelowSeries"/>.
        /// </para>
        /// </summary>
        /// <param name="loadingImage">The image to display when loading the tiles.</param>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(Stream loadingImage, ITileMapImageProvider tileMapImageProvider)
            : this(tileMapImageProvider)
        {
            _loadingImg = new OxyImage(loadingImage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation"/> class.
        /// <para>
        /// Sets the defaults <see cref="Annotation.Layer"/> to <see cref="AnnotationLayer.BelowSeries"/>.
        /// </para>
        /// </summary>
        /// <param name="loadingImage">The image to display when loading the tiles.</param>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(byte[] loadingImage, ITileMapImageProvider tileMapImageProvider)
            : this(tileMapImageProvider)
        {
            _loadingImg = new OxyImage(loadingImage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTileAnnotation"/> class.
        /// <para>
        /// Sets the defaults <see cref="Annotation.Layer"/> to <see cref="AnnotationLayer.BelowSeries"/>.
        /// </para>
        /// </summary>
        /// <param name="tileMapImageProvider"></param>
        public MapTileAnnotation(ITileMapImageProvider tileMapImageProvider)
        {
            _tileMapImageProvider = tileMapImageProvider;
            _tileMapImageProvider.InvalidatePlotOnUI += (s, e) => PlotModel.InvalidatePlot(false);
            base.Layer = AnnotationLayer.BelowSeries;
        }

        /// <summary>
        /// <inheritdoc/> The default is <c>false</c>.
        /// </summary>
        public bool IsTileGridVisible { get; set; }

        /// <summary>
        /// <inheritdoc/> The default is <c>OxyColors.Black</c>.
        /// </summary>
        public OxyColor TileGridColor { get; set; } = OxyColors.Black;

        /// <summary>
        /// <inheritdoc/> The default is <c>2</c>.
        /// </summary>
        public int TileGridThickness { get; set; } = 2;

        /// <inheritdoc/>
        public string? CopyrightNotice { get; set; }

        /// <summary>
        /// <inheritdoc/> The default is <c>256</c>.
        /// </summary>
        public int TileSize { get; set; } = 256;

        /// <summary>
        /// <inheritdoc/> The default is <c>0</c>.
        /// </summary>
        public int MinZoomLevel { get; set; } = 0;

        /// <summary>
        /// <inheritdoc/> The default is <c>20</c>.
        /// </summary>
        public int MaxZoomLevel { get; set; } = 20;

        /// <summary>
        /// <inheritdoc/> The default is <c>1</c>.
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// <inheritdoc/> The default is null.
        /// </summary>
        public string? AnnotationGroupName { get; set; }

        /// <summary>
        /// <inheritdoc/> The default is null.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// <inheritdoc/> The default is true.
        /// </summary>
        public bool RenderInLegend { get; set; } = true;

        /// <summary>
        /// <inheritdoc/> The default is true.
        /// </summary>
        public bool IsMapScaleVisible { get; set; } = true;

        /// <summary>
        /// <inheritdoc/> The default is true.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets the current zoom level.
        /// </summary>
        public int CurrentZoomLevel { get; protected set; }

        private MapScaleBase? _scale;

        public void AddScale(MapScaleBase scale)
        {
            _scale = scale;
            _scale.Parent = this;
        }

        /// <summary>
        /// Renders the annotation on the specified context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public override void Render(IRenderContext rc)
        {
            if (!IsVisible)
            {
                // TODO - Cancel all image loading?
                return;
            }

            var lon0 = XAxis.ActualMinimum;
            var lon1 = XAxis.ActualMaximum;
            var lat0 = YAxis.ActualMinimum;
            var lat1 = YAxis.ActualMaximum;

            // The desired number of tiles horizontally
            double tilesx = PlotModel.Width / TileSize;

            // Calculate the desired zoom level
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

            CurrentZoomLevel = zoom;

            int maxXY = (int)Math.Pow(2, zoom); // See https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y

            // Find tile coordinates for the corners
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

                    // Transform from tile coordinates to lat/lon
                    CartographyHelper.TileToLatLon(x, y, zoom, out double latitude0, out double longitude0);
                    CartographyHelper.TileToLatLon(x + 1, y + 1, zoom, out double latitude1, out double longitude1);

                    // Transform from lat/lon to screen coordinates
                    var s00 = this.Transform(longitude0, latitude0);
                    var s11 = this.Transform(longitude1, latitude1);

                    var r = OxyRect.Create(s00.X, s00.Y, s11.X, s11.Y);

                    // Draw the rectangle
                    if (IsTileGridVisible && TileGridThickness > 0 && !TileGridColor.IsUndefined() && TileGridColor.IsVisible())
                    {
                        rc.DrawRectangle(r, OxyColors.Undefined, TileGridColor, TileGridThickness, EdgeRenderingMode.PreferSpeed);
                    }

                    // Draw the image
                    rc.DrawImage(img, r.Left, r.Top, r.Width, r.Height, Opacity, true);
                }
            }

            // Draw the copyright notice
            RenderCopyrightNotice(rc);

            // Draw the map scale
            RenderMapScale(rc, zoom, lat0);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="legendBox">The bounding rectangle of the legend box.</param>
        public void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ymid = (legendBox.Top + legendBox.Bottom) / 2;

            var midpt = new ScreenPoint(xmid, ymid);

            rc.DrawMarker(
                midpt,
                MarkerType.Square,
                null,
                5,
                OxyColors.Red,
                OxyColors.Black,
                1,
                EdgeRenderingMode);
        }

        protected virtual void RenderCopyrightNotice(IRenderContext rc)
        {
            if (string.IsNullOrEmpty(CopyrightNotice))
            {
                return;
            }

            OxyColor background = OxyColor.FromAColor(200, OxyColors.White);
            var clippingRectangle = GetClippingRect();
            var p1 = new ScreenPoint(clippingRectangle.Right - 5, clippingRectangle.Bottom - 5);
            var size = rc.MeasureText(CopyrightNotice, ActualFont, ActualFontSize, ActualFontWeight);
            var p0 = new ScreenPoint(p1.X - size.Width, p1.Y - size.Height);
            rc.DrawRectangle(new OxyRect(p0, p1), background, background, 0, EdgeRenderingMode);
            rc.DrawText(p1, CopyrightNotice, OxyColors.Black, ActualFont, ActualFontSize, ActualFontWeight, 0,
                HorizontalAlignment.Right, VerticalAlignment.Bottom, size);
        }

        protected virtual void RenderMapScale(IRenderContext rc, int zoom, double latitude)
        {
            if (!IsMapScaleVisible || _scale?.IsMapScaleVisible != true)
            {
                return;
            }

            _scale.Render(rc, zoom, latitude);
        }

        private OxyImage? GetImage(int x, int y, int zoom)
        {
            var img = _tileMapImageProvider.GetImage(x, y, zoom);
            return img ?? _loadingImg;
        }
    }
}
