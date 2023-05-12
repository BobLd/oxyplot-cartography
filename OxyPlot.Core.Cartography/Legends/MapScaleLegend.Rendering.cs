using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using OxyPlot.Annotations;
using static System.Formats.Asn1.AsnWriter;

namespace OxyPlot.Legends
{
    public partial class MapScaleLegend
    {
        /// <summary>
        /// Makes the LegendOrientation property safe.
        /// </summary>
        /// <remarks>If Legend is positioned left or right, force it to vertical orientation</remarks>
        public override void EnsureLegendProperties()
        {
            switch (this.LegendPosition)
            {
                case LegendPosition.LeftTop:
                case LegendPosition.LeftMiddle:
                case LegendPosition.LeftBottom:
                case LegendPosition.RightTop:
                case LegendPosition.RightMiddle:
                case LegendPosition.RightBottom:
                    if (this.LegendOrientation == LegendOrientation.Horizontal)
                    {
                        this.LegendOrientation = LegendOrientation.Vertical;
                    }

                    break;
            }
        }

        /// <summary>
        /// Gets the rectangle of the legend box.
        /// </summary>
        /// <param name="legendSize">Size of the legend box.</param>
        /// <returns>A rectangle.</returns>
        public override OxyRect GetLegendRectangle(OxySize legendSize)
        {
            double top = 0;
            double left = 0;
            if (this.LegendPlacement == LegendPlacement.Outside)
            {
                switch (this.LegendPosition)
                {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = this.PlotModel.PlotAndAxisArea.Left - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom:
                        left = this.PlotModel.PlotAndAxisArea.Right + this.LegendMargin;
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = this.PlotModel.PlotAndAxisArea.Top - legendSize.Height - this.LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = this.PlotModel.PlotAndAxisArea.Bottom + this.LegendMargin;
                        break;
                }

                var bounds = this.AllowUseFullExtent
                    ? this.PlotModel.PlotAndAxisArea
                    : this.PlotModel.PlotArea;

                switch (this.LegendPosition)
                {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = bounds.Left;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = bounds.Right - legendSize.Width;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = bounds.Top;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = bounds.Bottom - legendSize.Height;
                        break;
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (bounds.Top + bounds.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (bounds.Left + bounds.Right - legendSize.Width) * 0.5;
                        break;
                }
            }
            else
            {
                switch (this.LegendPosition)
                {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom:
                        left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                        break;
                }

                switch (this.LegendPosition)
                {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                        break;

                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (this.PlotModel.PlotArea.Top + this.PlotModel.PlotArea.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (this.PlotModel.PlotArea.Left + this.PlotModel.PlotArea.Right - legendSize.Width) * 0.5;
                        break;
                }
            }

            return new OxyRect(left, top, legendSize.Width, legendSize.Height);
        }

        public override OxySize GetLegendSize(IRenderContext rc, OxySize availableLegendArea)
        {
            var availableLegendWidth = availableLegendArea.Width;
            var availableLegendHeight = availableLegendArea.Height;

            // Calculate the size of the legend box
            var legendSize = this.MeasureLegends(rc, new OxySize(Math.Max(0, availableLegendWidth), Math.Max(0, availableLegendHeight)));

            // Ensure legend size is valid
            legendSize = new OxySize(Math.Max(0, legendSize.Width), Math.Max(0, legendSize.Height));

            return legendSize;
        }

        /// <summary>
        /// Measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="availableSize">The available size for the legend box.</param>
        /// <returns>The size of the legend box.</returns>
        private OxySize MeasureLegends(IRenderContext rc, OxySize availableSize)
        {
            return this.RenderOrMeasureLegends(rc, new OxyRect(0, 0, availableSize.Width, availableSize.Height), true);
        }

        /// <summary>
        /// Renders or measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="rect">Provides the available size if measuring, otherwise it provides the position and size of the legend.</param>
        /// <param name="measureOnly">Specify if the size of the legend box should be measured only (not rendered).</param>
        /// <returns>The size of the legend box.</returns>
        private OxySize RenderOrMeasureLegends(IRenderContext rc, OxyRect rect, bool measureOnly = false)
        {
            var mapTileAnnotations = this.PlotModel.Annotations.OfType<IMapTileAnnotation>();

            if (mapTileAnnotations?.Any() != true)
            {
                throw new ArgumentNullException("TODO");
            }

            var mapTile = mapTileAnnotations.First();

            int zoom = mapTile.CurrentZoomLevel;
            double latitude = mapTile.YAxis.ActualMinimum;

            // Transform from tile coordinates to lat/lon
            CartographyHelper.TileToLatLon(0, 0, zoom, out double latitude0, out double longitude0);
            CartographyHelper.TileToLatLon(1, 1, zoom, out double latitude1, out double longitude1);

            System.Diagnostics.Debug.WriteLine($"Transform({measureOnly}, offset={mapTile.YAxis.Offset}, scale={mapTile.YAxis.Scale})");

            // Transform from lat/lon to screen coordinates
            var s00 = mapTile.Transform(longitude0, latitude0);
            var s11 = mapTile.Transform(longitude1, latitude1);

            double actualTileWidth = s11.X - s00.X;

            // https://wiki.openstreetmap.org/wiki/Zoom_levels
            // The horizontal distance represented by each square tile,
            // measured along the parallel at a given latitude, is given by:
            //    Stile = C ∙ cos(latitude) / 2^zoomlevel
            // where C means the equatorial circumference of the Earth
            // (40 075 016.686 m ≈ 2π ∙ 6 378 137.000 m for the reference geoid used by OpenStreetMap).
            const double C_m = 2.0 * Math.PI * 6_378_137.0; // The equatorial circumference of the Earth (int meters)
            double S_tile_m = C_m * Math.Cos(latitude * Math.PI / 180.0) / Math.Pow(2, zoom);
            //double S_tile_2 = 1.0 / Math.ScaleB(1.0 / (C * Math.Cos(latitude * Math.PI / 180.0)), zoom);

            // The horizontal distance represented by one pixel is:
            double S_pixel_m = S_tile_m / actualTileWidth; // (in meters)

            // This formula assumes that the Earth is perfectly spheric, but since the Earth is actually
            // ellipsoidal there will be a slight error in this calculation, which does not take into account the
            // flattening (with a slight reduction of radius for the best-fitting sphere passing at geographic
            // poles at average sea level). But this error is very slight: it is null on the reference Equator,
            // then grows to an absolute maximum of 0.3% at median latitudes, then shrinks back to zero at
            // high latitudes towards poles.
            //
            // The error also does not take into account additional differences caused by variation of the
            // altitude on ground, or by the irregular variations of the geographic polar axis, and other
            // errors caused by celestial tidal effects and climatic effects on the average sea level, or by
            // continent drifts, major earthquakes, and magmatic flows below the crust).

            const int scaleSizePixel = 100;
            double distanceMeters = S_pixel_m * scaleSizePixel; // distance of 100 pixels
            double scaleMeters = RoundScale(distanceMeters);
            double scaleMetersPixels = Math.Round(scaleMeters / S_pixel_m, 0);

            if (scaleMetersPixels > 2.5 * scaleSizePixel)
            {
                // We don't render legend if too big (too much zoom)
                return new OxySize(0, 0);
            }

            double scaleMetterInFeet = CartographyHelper.MetersToFeet(scaleMeters);

            double scaleFeet;
            if (scaleMetterInFeet > 5280) // miles
            {
                double scaleMiles = scaleMetterInFeet / 5280;
                scaleFeet = RoundScale(scaleMiles) * 5280;
            }
            else // feet
            {
                scaleFeet = RoundScale(scaleMetterInFeet);
            }

            double scaleFeetPixels = Math.Round(CartographyHelper.FeetToMeters(scaleFeet) / S_pixel_m, 0);

            double mainScalePixel = Math.Max(scaleMetersPixels, scaleFeetPixels);

            var size = new OxySize(mainScalePixel, (ActualFontSize + 2) * 2);

            if (measureOnly)
            {
                return size;
            }

            //rc.PushClip(rect); // There's an issue with rect so far, we don't clip

            double scaleY = (rect.Top + rect.Bottom) / 2.0; // Y
            double scaleStartX = rect.Left;                 // X
            double sign = 1;
            HorizontalAlignment textHorizontalAlignment = HorizontalAlignment.Left;
            if (this.LegendPosition == LegendPosition.TopRight || this.LegendPosition == LegendPosition.BottomRight)
            {
                scaleStartX = rect.Right;
                sign = -1;
                textHorizontalAlignment = HorizontalAlignment.Right;
            }

            ScreenPoint scaleStartPoint = new ScreenPoint(scaleStartX, scaleY);

            OxyColor background = OxyColor.FromAColor(200, OxyColors.White);
            OxyPen linePen = OxyPen.Create(OxyColors.Black, 1.0);

            // Draw meters scale background 
            double metersTickX = scaleStartX + sign * scaleMetersPixels;
            double metersTickStartY = scaleY - ActualFontSize - 2;
            rc.DrawRectangle(new OxyRect(scaleStartPoint, new ScreenPoint(metersTickX, metersTickStartY)),
                             background,
                             background,
                             0,
                             EdgeRenderingMode);

            // Draw feet scale background 
            double feetTickX = scaleStartX + sign * scaleFeetPixels;
            double feetTickStartY = scaleY + ActualFontSize + 2;
            rc.DrawRectangle(new OxyRect(scaleStartPoint, new ScreenPoint(feetTickX, feetTickStartY)),
                             background,
                             background,
                             0,
                             EdgeRenderingMode);

            // Draw scale main lines
            // Horizontal line
            rc.DrawLine(scaleStartX,
                        scaleY,
                        scaleStartX + sign * mainScalePixel,
                        scaleY,
                        linePen,
                        EdgeRenderingMode);

            // Vertical line
            rc.DrawLine(scaleStartX,
                        metersTickStartY,
                        scaleStartX,
                        feetTickStartY,
                        linePen,
                        EdgeRenderingMode);

            // Draw meters tick
            rc.DrawLine(metersTickX,
                        scaleY,
                        metersTickX,
                        metersTickStartY,
                        linePen,
                        EdgeRenderingMode);

            // Draw feet tick
            rc.DrawLine(feetTickX,
                        scaleY,
                        feetTickX,
                        feetTickStartY,
                        linePen,
                        EdgeRenderingMode);

            var scaleTextStartPoint = new ScreenPoint(scaleStartPoint.X + sign * 4, scaleStartPoint.Y);

            // Draw meters text
            rc.DrawMathText(scaleTextStartPoint,
                            CartographyScaleHelper.FormatStringMeters(scaleMeters),
                            OxyColors.Black,
                            ActualFont,
                            ActualFontSize,
                            ActualFontWeight,
                            0,
                            textHorizontalAlignment,
                            VerticalAlignment.Bottom);

            // Draw feet text
            rc.DrawMathText(scaleTextStartPoint,
                            CartographyScaleHelper.FormatStringFeet(scaleFeet),
                            OxyColors.Black,
                            ActualFont,
                            ActualFontSize,
                            ActualFontWeight,
                            0,
                            textHorizontalAlignment,
                            VerticalAlignment.Top);

#if DEBUG
            //var orange = OxyColor.FromAColor(200, OxyColors.Orange);
            //rc.DrawRectangle(rect, orange, OxyColors.Undefined, 1, EdgeRenderingMode.Adaptive);
            //rc.DrawCircle(scaleStartX, scaleY, 3, OxyColors.Red, OxyColors.Red, 0, EdgeRenderingMode.Adaptive);
#endif

            // There's an issue with rect so far, we don't clip
            //if (!measureOnly)
            //{
            //rc.PopClip();
            //}

            return size;
        }

        protected virtual double RoundScale(double scale)
        {
            //return scale;
            return CartographyScaleHelper.RoundScale2And5(scale);
        }

        /// <summary>
        /// Renders or measures the legends.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public override void RenderLegends(IRenderContext rc)
        {
            this.RenderOrMeasureLegends(rc, this.LegendArea);
        }
    }
}
