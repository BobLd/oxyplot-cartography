using System.Runtime.CompilerServices;

namespace OxyPlot
{
    /// <summary>
    /// Provides functionalities useful with cartography and maps.
    /// </summary>
    public static class CartographyHelper
    {
        /// <summary>
        /// List of map tiles apis.
        /// </summary>
        public static readonly MapTileApis Apis = new MapTileApis();

        /// <summary>
        /// Top edge is 85.0511°N and bottom edge is 85.0511°S.
        /// For the curious, the number 85.0511 is the result of arctan(sinh(π)). By using this bound, the entire map becomes a (very large) square.
        /// <para>See <see href="https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y"/></para>
        /// See <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection#WKT_definition"/>
        /// </summary>
        public const double MaxMercatorProjectionLatitude = 85.0511;

        /// <summary>
        /// Left edge is 180°W and right edge is 180°E.
        /// <para>See <see href="https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y"/></para>
        /// See <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection#WKT_definition"/>
        /// </summary>
        public const double MaxLongitude = 180;

        /// <summary>
        /// Spherical Pseudo-Mercator projection. Y to Latitude.
        /// <para>
        /// <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
        /// </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PseudoMercatorProjectionYToLatitude(double y)
        {
            return (Math.Atan(Math.Exp(y / 180.0 * Math.PI)) / Math.PI * 360.0) - 90.0;
        }

        /// <summary>
        /// Spherical Pseudo-Mercator projection. Latitude to Y.
        /// <para>
        /// <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
        /// </para>
        /// </summary>
        /// <param name="latitude">Latitude, in degrees</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PseudoMercatorProjectionLatitudeToY(double latitude)
        {
            return Math.Log(Math.Tan((latitude + 90.0) / 360.0 * Math.PI)) / Math.PI * 180.0;
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
        /// Converts decimal degrees (DD) to degrees minutes seconds coordinates (DMS).
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
        /// </summary>
        /// <param name="decimalDegrees">The decimal degrees (DD) coordinate</param>
        /// <param name="isLatitude">True if latitude, false if longitude</param>
        /// <param name="secondsDecimal">Decimal roundings of seconds part</param>
        /// <returns>The degrees minutes seconds coordinates (DMS), e.g. 50°40′46.461″N</returns>
        public static string DecimalDegreesToDegreesMinutesSeconds(double decimalDegrees, bool isLatitude, int secondsDecimal)
        {
            // see https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)
            char card = isLatitude ? (decimalDegrees > 0 ? 'N' : 'S') : (decimalDegrees > 0 ? 'E' : 'W');
            double d = Math.Truncate(decimalDegrees);
            double delta = Math.Abs(decimalDegrees - d);
            double m = Math.Truncate(60 * delta);
            double s = Math.Round(3600.0 * delta - 60.0 * m, secondsDecimal);
            return $"{Math.Abs(d):00}\u00b0{m:00}\u2032{s:00}\u2033{card}";
        }

        /// <summary>
        /// Converts degrees minutes seconds (DMS) to decimal degrees (DD) coordinates.
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="cardinal"></param>
        /// <returns>The decimal degrees (DD) coordinates</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double DegreesMinutesSecondsToDecimalDegrees(int degrees, int minutes, double seconds, char cardinal)
        {
            double sign;
            if (cardinal.Equals('N') || cardinal.Equals('E'))
            {
                sign = 1;
            }
            else if (cardinal.Equals('S') || cardinal.Equals('W'))
            {
                sign = -1;
            }
            else
            {
                throw new ArgumentException($"Cardinal value should be either 'N', 'S', 'E' or 'W', but is '{cardinal}'", nameof(cardinal));
            }

            return (degrees + minutes / 60.0 + seconds / 3600.0) * sign;
        }

        /// <summary>
        /// Converts degrees minutes seconds (DMS) to decimal degrees (DD) coordinates.
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
        /// </summary>
        /// <param name="degreesMinutesSeconds">
        /// The degrees minutes seconds coordinate, formated as DD°MM′SS.SSS″C e.g. 38°53′23″N
        /// <para>
        /// ° = \u00b0, ′ = \u2032, ″ = \u2033
        /// </para>
        /// </param>
        /// <returns>The decimal degrees (DD) coordinates</returns>
        public static double DegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            var parts = degreesMinutesSeconds.Split('\u00b0', '\u2032', '\u2033');
            if (parts.Length != 4)
            {
                throw new ArgumentException("The coordinate should be in DMS format, i.e. DD°MM′SS.SSS″C", nameof(degreesMinutesSeconds));
            }

            if (!int.TryParse(parts[0], out int d))
            {
                throw new ArgumentException($"Could not parse degrees part, got {parts[0]}. The coordinate should be in DMS format, i.e. DD°MM′SS.SSS″C", nameof(degreesMinutesSeconds));
            }

            if (!int.TryParse(parts[1], out int m))
            {
                throw new ArgumentException($"Could not parse minutes part, got {parts[1]}. The coordinate should be in DMS format, i.e. DD°MM′SS.SSS″C", nameof(degreesMinutesSeconds));
            }

            if (!double.TryParse(parts[2], out double s))
            {
                throw new ArgumentException($"Could not parse seconds part, got {parts[2]}. The coordinate should be in DMS format, i.e. DD°MM′SS.SSS″C", nameof(degreesMinutesSeconds));
            }

            var card = parts[3];
            if (card.Length != 1)
            {
                throw new ArgumentException($"Could not parse cardinal part, got {parts[3]}. The coordinate should be in DMS format, i.e. DD°MM′SS.SSS″C", nameof(degreesMinutesSeconds));
            }

            return DegreesMinutesSecondsToDecimalDegrees(d, m, s, card[0]);
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
