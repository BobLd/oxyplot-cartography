namespace OxyPlot.Annotations
{
    public class MapScale : MapScaleBase
    {
        public override OxyRect GetClippingRect()
        {
            throw new NotImplementedException();
        }

        public override void Render(IRenderContext rc, int zoom, double latitude)
        {
            if (Parent is null)
            {
                throw new NullReferenceException();
            }

            // Transform from tile coordinates to lat/lon
            CartographyHelper.TileToLatLon(0, 0, zoom, out double latitude0, out double longitude0);
            CartographyHelper.TileToLatLon(1, 1, zoom, out double latitude1, out double longitude1);

            // Transform from lat/lon to screen coordinates
            var s00 = Parent.Transform(longitude0, latitude0);
            var s11 = Parent.Transform(longitude1, latitude1);

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
            double scaleMeters = CartographyScaleHelper.RoundScale2And5(distanceMeters);
            double scaleMetersPixels = Math.Round(scaleMeters / S_pixel_m, 0);

            //string scale = $"z={zoom}, m/pixel={S_pixel:0.000}, tile width={actualTileWidth:0.0}, legend stick={rounded}m ({roundedPixels}px)";

            if (scaleMetersPixels > 2.5 * scaleSizePixel)
            {
                // We don't render legend if too big (too much zoom)
                return;
            }

            double scaleFeet = CartographyScaleHelper.RoundScale2And5(CartographyHelper.MetersToFeet(distanceMeters));
            double scaleFeetPixels = Math.Round(CartographyHelper.FeetToMeters(scaleFeet) / S_pixel_m, 0);

            double mainScale = Math.Max(scaleMetersPixels, scaleFeetPixels);

            var clippingRectangle = Parent.GetClippingRect();
            double scaleY = clippingRectangle.Bottom - ActualFontSize - 5;
            double scaleStartX = clippingRectangle.Left + 5;
            var scaleStartPoint = new ScreenPoint(scaleStartX, scaleY);

            OxyColor background = OxyColor.FromAColor(200, OxyColors.White);
            OxyPen linePen = OxyPen.Create(OxyColors.Black, 1.0);

            // Draw meters scale background 
            double metersTickX = scaleStartX + scaleMetersPixels;
            double metersTickStartY = scaleY - ActualFontSize - 2;
            rc.DrawRectangle(new OxyRect(scaleStartPoint, new ScreenPoint(metersTickX, metersTickStartY)), background, background, 0, EdgeRenderingMode);

            // Draw feet scale background 
            double feetTickX = scaleStartX + scaleFeetPixels;
            double feetTickStartY = scaleY + ActualFontSize + 2;
            rc.DrawRectangle(new OxyRect(scaleStartPoint, new ScreenPoint(feetTickX, feetTickStartY)), background, background, 0, EdgeRenderingMode);

            // Draw scale main lines
            rc.DrawLine(scaleStartX,
                        scaleY,
                        scaleStartX + mainScale,
                        scaleY,
                        linePen,
                        EdgeRenderingMode);

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

            ScreenPoint scaleTextStartPoint = new ScreenPoint(scaleStartPoint.X + 4, scaleStartPoint.Y);
            // Draw meters text
            rc.DrawText(scaleTextStartPoint, CartographyScaleHelper.FormatStringMeters(scaleMeters), OxyColors.Black, ActualFont, ActualFontSize, ActualFontWeight,
                        0, HorizontalAlignment.Left, VerticalAlignment.Bottom);

            // Draw feet text
            rc.DrawText(scaleTextStartPoint, CartographyScaleHelper.FormatStringFeet(scaleFeet), OxyColors.Black, ActualFont, ActualFontSize, ActualFontWeight,
                        0, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }
}
