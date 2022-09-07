namespace OxyPlot
{
    public static class CartographyScaleHelper
    {
        public static string FormatStringMeters(double meters)
        {
            return meters > 1000 ? $"{meters / 1000:0} km" : $"{meters:0} m";
        }

        public static string FormatStringFeet(double feet)
        {
            return feet > 5280 ? $"{feet / 5280:0} mi" : $"{feet:0} ft";
        }

        public static double RoundScale(double distance)
        {
            double nearestDistance = Math.Max(1, Math.Round(distance, 0)); // Minimum 1
            double orderMagn = Math.Floor(Math.Log10(nearestDistance) + 1);
            double roundFactor = Math.Pow(10, orderMagn - 1);
            return Math.Round(nearestDistance / roundFactor, 0) * roundFactor;
        }

        public static double RoundScale2And5(double distance)
        {
            double nearestDistance = Math.Max(1, Math.Round(distance, 0)); // Minimum 1
            double orderMagn = Math.Floor(Math.Log10(nearestDistance) + 1);
            double roundFactor = Math.Pow(10, orderMagn - 1);
            int round = (int)Math.Round(nearestDistance / roundFactor, 0); // * roundFactor;

            if (round <= 2)
            {
                return round * roundFactor;
            }
            else if (round > 2 && round <= 5)
            {
                return 2 * roundFactor;
            }
            else
            {
                return 5 * roundFactor;
            }
        }
    }
}
