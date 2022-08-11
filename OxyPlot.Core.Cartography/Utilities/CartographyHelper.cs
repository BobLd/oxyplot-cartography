using System.Runtime.CompilerServices;

namespace OxyPlot
{
    /// <summary>
    /// Provides functionalities useful with cartography and maps
    /// </summary>
    public static class CartographyHelper
    {
        /// <summary>
        /// Y to Latitude.
        /// <para>
        /// <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
        /// </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double YToLatitude(double y)
        {
            return (Math.Atan(Math.Exp(y / 180 * Math.PI)) / Math.PI * 360) - 90;
        }

        /// <summary>
        /// Latitude to Y.
        /// <para>
        /// <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
        /// </para>
        /// </summary>
        /// <param name="latitude">Latitude, in degrees</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LatitudeToY(double latitude)
        {
            return Math.Log(Math.Tan((latitude + 90) / 360 * Math.PI)) / Math.PI * 180;
        }

        /// <summary>
        /// Gets the Mercator adjustement factor, the secant of the latitude.
        /// <para>secθ = 1 / cosθ</para>
        /// </summary>
        /// <param name="latitude">Latitude, in degrees</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetMercatorAdj(double latitude)
        {
            return 1.0 / Math.Cos(Math.Abs(latitude) * Math.PI / 180.0);
        }

        /// <summary>
        /// Transforms a position to a tile coordinate (x,y).
        /// <para>
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
        /// </para>
        /// </summary>
        /// <param name="latitude">The latitude</param>
        /// <param name="longitude">The longitude</param>
        /// <param name="zoom">The zoom</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        public static void LatLonToTile(double latitude, double longitude, int zoom, out double x, out double y)
        {
            int n = 1 << zoom;
            x = (longitude + 180.0) / 360.0 * n;
            double lat = latitude / 180 * Math.PI;
            y = (1.0 - (Math.Log(Math.Tan(lat) + (1.0 / Math.Cos(lat))) / Math.PI)) / 2.0 * n;
        }

        /// <summary>
        /// Transforms a tile coordinate (x,y) to a position.
        /// </summary>
        /// <para>
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
        /// </para>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="zoom">The zoom</param>
        /// <param name="latitude">The latitude</param>
        /// <param name="longitude">The longitude</param>
        public static void TileToLatLon(double x, double y, int zoom, out double latitude, out double longitude)
        {
            int n = 1 << zoom;
            longitude = (x / n * 360.0) - 180.0;
            double lat = Math.Atan(Math.Sinh(Math.PI * (1 - (2 * y / n))));
            latitude = lat * 180.0 / Math.PI;
        }

        /// <summary>
        /// Generate the storage path from a uri.
        /// </summary>
        /// <param name="uri">The uri to generate the storage path</param>
        /// <returns>The storage path</returns>
        public static string GetStoragePathFromUri(Uri uri)
        {
            var invalids = Path.GetInvalidFileNameChars();
            string path = uri.Host;

            foreach (var segment in uri.Segments)
            {
                var cleanedSegment = segment.Replace("/", "").Replace("\\", "");
                // https://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
                cleanedSegment = string.Join("_", cleanedSegment.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

                if (string.IsNullOrEmpty(cleanedSegment))
                {
                    continue;
                }

                if (cleanedSegment.Contains("%7BZ%7D") || cleanedSegment.Contains("%7BY%7D") || cleanedSegment.Contains("%7BX%7D"))
                {
                    break;
                }
                path = Path.Combine(path, cleanedSegment);
            }

            return path;
        }
    }
}
