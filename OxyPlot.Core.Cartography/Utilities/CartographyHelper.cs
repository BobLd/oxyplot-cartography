using System.Runtime.CompilerServices;
using OxyPlot.Series;

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
        /// <para>
        /// See <see href="https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y"/>
        /// </para>
        /// See <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection#WKT_definition"/>
        /// </summary>
        public const double MaxMercatorProjectionLatitude = 85.0511;

        /// <summary>
        /// Left edge is 180°W and right edge is 180°E.
        /// <para>
        /// See <see href="https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y"/>
        /// </para>
        /// See <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection#WKT_definition"/>
        /// </summary>
        public const double MaxLongitude = 180;

        /// <summary>
        /// Spherical Pseudo-Mercator projection. Y to Latitude.
        /// <para>
        /// See <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
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
        /// See <see href="https://wiki.openstreetmap.org/wiki/Mercator#C#"/>
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
        /// See <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
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
        /// Transforms a position to a tile coordinate (x,y).
        /// <para>
        /// See <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
        /// </para>
        /// </summary>
        /// <param name="coordinate">The map coordinate</param>
        /// <param name="zoom">The zoom</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        public static void LatLonToTile(MapCoordinate coordinate, int zoom, out double x, out double y)
        {
            LatLonToTile(coordinate.Latitude, coordinate.Longitude, zoom, out x, out y);
        }

        /// <summary>
        /// Transforms a tile coordinate (x,y) to a position.
        /// </summary>
        /// <para>
        /// See <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
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
        /// Transforms a tile coordinate (x,y) to a position.
        /// </summary>
        /// <para>
        /// See <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames"/>
        /// </para>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="zoom">The zoom</param>
        public static MapCoordinate TileToLatLon(double x, double y, int zoom)
        {
            int n = 1 << zoom;
            double latitude = Math.Atan(Math.Sinh(Math.PI * (1 - (2 * y / n)))) * 180.0 / Math.PI;
            double longitude = (x / n * 360.0) - 180.0;
            return new MapCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Converts decimal degrees (DD) to degrees minutes seconds coordinates (DMS).
        /// <para>
        /// See <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// See <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
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
        /// See <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// See <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="cardinal">N, S, E or W</param>
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
        /// See <see href="https://en.wikipedia.org/wiki/Decimal_degrees"/>
        /// </para>
        /// See <see href="https://en.wikipedia.org/wiki/ISO_6709#Representation_at_the_human_interface_(Annex_D)"/>
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

        /// <summary>
        /// Gets the <see cref="DataPoint"/> with X coordinate being the longitude and Y coordinate beeing the latitude.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns>The corresponding <see cref="DataPoint"/> with X coordinate being the longitude and Y coordinate beeing the latitude</returns>
        public static DataPoint ToDataPoint(this MapCoordinate coordinate)
        {
            return new DataPoint(coordinate.Longitude, coordinate.Latitude);
        }

        #region Terminator lines
        /// <summary>
        /// Compute Terminator line (day / night).
        /// <para>
        /// See <see href="https://www.aa.quae.nl/en/antwoorden/zonpositie.html#v526"/>
        /// </para>
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="phi">The distance (in degrees) along the terminator from a particular one of its intersections with the equator, in degrees</param>
        /// <returns>B is the latitude, L is the longitude</returns>
        public static MapCoordinate ComputeTerminator(DateTime utcTime, double phi)
        {
            var coordinate = ComputeSunStraightUpPoint(utcTime); // b, l

            (double sinPhi, double cosPhi) = Math.SinCos(phi * Math.PI / 180.0);
            (double sinl, double cosl) = Math.SinCos(coordinate.Longitude * Math.PI / 180.0);
            (double sinb, double cosb) = Math.SinCos(coordinate.Latitude * Math.PI / 180.0);

            double B = Math.Asin(cosb * sinPhi) * 180.0 / Math.PI; // B is the latitude
            double x = (-cosl * sinb * sinPhi) - (sinl * cosPhi);
            double y = (-sinl * sinb * sinPhi) + (cosl * cosPhi);
            double L = Math.Atan2(y, x) * 180.0 / Math.PI; // L is the longitude

            //return (L, B);
            return new MapCoordinate(B, L);
        }

        /// <summary>
        /// 
        /// <para>
        /// See <see href="https://www.aa.quae.nl/en/antwoorden/zonpositie.html#v526"/>
        /// </para>
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns>latitude = b, longitude = l</returns>
        public static MapCoordinate ComputeSunStraightUpPoint(DateTime utcTime)
        {
            // At t hours UTC on that day, the Sun is straight up from a location at a latitude equal
            // to b=δ (northern latitudes are positive and southern latitudes are negative), and a longitude
            // equal to l = 180 - 15 * t degrees. Add or subtract 360° if the result is not between −180° and +180°.
            // Then eastern longitudes are positive, and western longitudes are negative.

            double lat = ComputeSunDeclination(utcTime);
            double t = utcTime.TimeOfDay.TotalHours;
            double lon = 180.0 - 15.0 * t;
            //lon %= 360; // 180?

            while (lon < -180)
            {
                lon += 360;
            }

            while (lon > +180)
            {
                lon -= 360;
            }

            return new MapCoordinate(lat, lon);
        }

        /// <summary>
        /// 
        /// <para>
        /// See <see href="https://www.aa.quae.nl/en/antwoorden/zonpositie.html#v526"/>
        /// </para>
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns>Latitude</returns>
        public static double ComputeSunDeclination(DateTime utcTime)
        {
            // d is the number of days since (the beginning of) the most recent December 31st
            // (i.e., d=1 for midnight at the beginning of January 1st, for January 2nd, and so on).
            int d = utcTime.DayOfYear;
            double M = -3.6 + 0.9856 * d; // M is the solar mean anomaly
            double nu = M + 1.9 * Math.Sin(M * Math.PI / 180.0); // (v)
            double lambda = nu + 102.9;
            double sinLambda = Math.Sin(lambda * Math.PI / 180.0);
            return (double)(22.8 * sinLambda + 0.6 * (sinLambda * sinLambda * sinLambda)); // delta
        }

        /// <summary>
        /// http://www.stjarnhimlen.se/comp/ppcomp.html#5
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ComputeDayNumber(DateTime dateTime)
        {
            int y = dateTime.Year;
            int m = dateTime.Month;
            int D = dateTime.Day;

            // INTEGER divisions
            var d = 367 * y - 7 * (y + (m + 9) / 12) / 4 - 3 * ((y + (m - 9) / 7) / 100 + 1) / 4 + 275 * m / 9 + D - 730515;

            // floating-point division
            return d + (dateTime.TimeOfDay.TotalHours / 24.0); // TODO or HOURS instead of TotalHours
        }

        /// <summary>
        /// Revolution function, normalizes an angle to between 0 and 360 degrees by adding
        /// or subtracting even multiples of 360.
        /// </summary>
        private static double Rev(double x)
        {
            return x - Math.Floor(x / 360.0) * 360.0;
        }

        /// <summary>
        /// M
        /// <para>
        /// This angle increases uniformly over time, by 360 degrees per orbital period. It's zero at perihelion. It's easily computed from the orbital period and the time since last perihelion.
        /// </para>
        /// See <see href="http://www.stjarnhimlen.se/comp/ppcomp.html#5"/>
        /// </summary>
        /// <param name="d">Day number</param>
        /// <returns>The angle in degrees, between 0 and 360</returns>
        public static double ComputeSunMeanAnomaly(double d)
        {
            double M = 356.0470 + 0.9856002585 * d;

            // http://www.stjarnhimlen.se/comp/ppcomp.html#5
            /*
             * When computing M (and, for the Moon, when computing N and w as well),
             * one will quite often get a result that is larger than 360 degrees,
             * or negative (all angles are here computed in degrees). If negative,
             * add 360 degrees until positive. If larger than 360 degrees, subtract
             * 360 degrees until the value is less than 360 degrees. Note that, in
             * most programming languages, one must then multiply these angles with
             * pi/180 to convert them to radians, before taking the sine or cosine of them.
             */

            return Rev(M);
        }

        /// <summary>
        /// M
        /// <para>
        /// This angle increases uniformly over time, by 360 degrees per orbital period. It's zero at perihelion. It's easily computed from the orbital period and the time since last perihelion.
        /// </para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ComputeSunMeanAnomaly(DateTime dateTime)
        {
            double d = ComputeDayNumber(dateTime);
            return ComputeSunMeanAnomaly(d);
        }

        /// <summary>
        /// e
        /// <para>
        /// (0=circle, 0-1=ellipse, 1=parabola)
        /// </para>
        /// </summary>
        /// <param name="d">Day number</param>
        /// <returns></returns>
        public static double ComputeSunEccentricity(double d)
        {
            return 0.016709 - (1.151E-9 * d); // e
        }

        /// <summary>
        /// e
        /// <para>
        /// (0=circle, 0-1=ellipse, 1=parabola)
        /// </para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ComputeSunEccentricity(DateTime dateTime)
        {
            double d = ComputeDayNumber(dateTime);
            return ComputeSunEccentricity(d); // e
        }

        /// <summary>
        /// E
        /// </summary>
        /// <param name="M">Sun mean anomaly</param>
        /// <param name="e">Sun eccentricity</param>
        /// <returns></returns>
        public static double ComputeSunEccentricAnomaly(double M, double e)
        {
            (double sinM, double cosM) = Math.SinCos(M * Math.PI / 180.0);
            return M + e * (180.0 / Math.PI) * sinM * (1.0 + e * cosM); // E
        }

        /// <summary>
        /// E
        /// </summary>
        /// <param name="d">Day number</param>
        /// <returns></returns>
        public static double ComputeSunEccentricAnomaly(double d)
        {
            double M = ComputeSunMeanAnomaly(d);
            double e = ComputeSunEccentricity(d);
            return ComputeSunEccentricAnomaly(M, e);
        }

        /// <summary>
        /// E
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ComputeSunEccentricAnomaly(DateTime dateTime)
        {
            double d = ComputeDayNumber(dateTime);
            return ComputeSunEccentricAnomaly(d);
        }

        /// <summary>
        /// w
        /// </summary>
        /// <param name="d">Day number</param>
        /// <returns></returns>
        public static double ComputeSunArgumentOfPerihelion(double d)
        {
            return 282.9404 + 4.70935E-5 * d;
        }

        /// <summary>
        /// ecl
        /// <para>
        ///  the obliquity of the ecliptic, i.e. the "tilt" of the Earth's axis of rotation (currently 23.4
        ///  degrees and slowly decreasing). First, compute the "d" of the moment of interest (section 3).
        /// </para>
        /// See <see href="http://www.stjarnhimlen.se/comp/ppcomp.html#5"/>
        /// </summary>
        /// <param name="d">Day number</param>
        /// <returns></returns>
        public static double ComputeEarthObliquityEcliptic(double d)
        {
            return 23.4393 - 3.563E-7 * d; // ecl
        }

        public static (double RA, double Dec) ComputeSunPosition(double M, double e, double w, double ecl)
        {
            double E = ComputeSunEccentricAnomaly(M, e);

            (double sinE, double cosE) = Math.SinCos(E * Math.PI / 180.0);
            double xv = cosE - e;
            double yv = Math.Sqrt(1.0 - e * e) * sinE;

            double v = Math.Atan2(yv, xv) * 180.0 / Math.PI;    // Sun true anomaly
            double r = Math.Sqrt(xv * xv + yv * yv);            // Sun distance

            double lonsun = v + w; // Sun true longitude

            // Convert lonsun, r to ecliptic rectangular geocentric coordinates xs,ys:
            (double sinLonsun, double cosLonsun) = Math.SinCos(lonsun * Math.PI / 180.0);
            double xs = r * cosLonsun;
            double ys = r * sinLonsun;

            // (since the Sun always is in the ecliptic plane, zs is of course zero).
            // xs,ys is the Sun's position in a coordinate system in the plane of the ecliptic.
            // To convert this to equatorial, rectangular, geocentric coordinates, compute:
            (double sinEcl, double cosEcl) = Math.SinCos(ecl * Math.PI / 180.0);
            double xe = xs;
            double ye = ys * cosEcl;
            double ze = ys * sinEcl;

            // Finally, compute the Sun's Right Ascension (RA) and Declination (Dec):
            double RA = Math.Atan2(ye, xe) * 180.0 / Math.PI;
            double Dec = Math.Atan2(ze, Math.Sqrt(xe * xe + ye * ye)) * 180.0 / Math.PI;
            return (RA, Dec);
        }

        public static (double RA, double Dec) ComputeSunPosition(double d)
        {
            double M = ComputeSunMeanAnomaly(d);
            double e = ComputeSunEccentricity(d);
            double w = ComputeSunArgumentOfPerihelion(d);
            double ecl = ComputeEarthObliquityEcliptic(d);
            return ComputeSunPosition(M, e, w, ecl);
        }

        public static (double RA, double Dec) ComputeSunPosition(DateTime dateTime)
        {
            double d = ComputeDayNumber(dateTime);
            return ComputeSunPosition(d);
        }

        public static MapCoordinate ComputeSunStraightUpPoint2(DateTime utcTime)
        {
            // At t hours UTC on that day, the Sun is straight up from a location at a latitude equal
            // to b=δ (northern latitudes are positive and southern latitudes are negative), and a longitude
            // equal to l = 180 - 15 * t degrees. Add or subtract 360° if the result is not between −180° and +180°.
            // Then eastern longitudes are positive, and western longitudes are negative.

            double lat = ComputeSunPosition(utcTime).Dec;
            double t = utcTime.TimeOfDay.TotalHours;
            double lon = 180.0 - 15.0 * t;
            //lon %= 360; // 180?

            while (lon < -180)
            {
                lon += 360;
            }

            while (lon > +180)
            {
                lon -= 360;
            }

            return new MapCoordinate(lat, lon);
        }

        /// <summary>
        /// Compute Terminator line (day / night).
        /// <para>
        /// See <see href="https://www.aa.quae.nl/en/antwoorden/zonpositie.html#v526"/>
        /// </para>
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="phi">The distance (in degrees) along the terminator from a particular one of its intersections with the equator, in degrees</param>
        /// <returns>B is the latitude, L is the longitude</returns>
        public static MapCoordinate ComputeTerminator2(DateTime utcTime, double phi)
        {
            var coordinate = ComputeSunStraightUpPoint2(utcTime); // b, l

            (double sinPhi, double cosPhi) = Math.SinCos(phi * Math.PI / 180.0);
            (double sinl, double cosl) = Math.SinCos(coordinate.Longitude * Math.PI / 180.0);
            (double sinb, double cosb) = Math.SinCos(coordinate.Latitude * Math.PI / 180.0);

            double B = Math.Asin(cosb * sinPhi) * 180.0 / Math.PI; // B is the latitude
            double x = (-cosl * sinb * sinPhi) - (sinl * cosPhi);
            double y = (-sinl * sinb * sinPhi) + (cosl * cosPhi);
            double L = Math.Atan2(y, x) * 180.0 / Math.PI; // L is the longitude

            //return (L, B);
            return new MapCoordinate(B, L);
        }

        public static double ComputeSolarElevation(DateTime dateTime, double latitude)
        {
            // https://solarsena.com/solar-elevation-angle-altitude/
            (double sinLatitude, double cosLatitude) = Math.SinCos(latitude * Math.PI / 180.0);
            double declinationAngle = ComputeSunPosition(dateTime).Dec;
            (double sinDeclinationAngle, double cosDeclinationAngle) = Math.SinCos(latitude * Math.PI / 180.0);
            double h = 0; // solar hour angle
            double sinSolarElevation = sinLatitude * sinDeclinationAngle + cosLatitude * cosDeclinationAngle * Math.Cos(h * Math.PI / 180.0);
            return Math.Asin(sinSolarElevation) * 180.0 / Math.PI;
        }

        public static double ComputeSolarHourAngle(DateTime dateTime)
        {
            // https://solarsena.com/solar-hour-angle-calculator-formula/
            // https://gml.noaa.gov/grad/solcalc/solareqns.PDF

            throw new NotImplementedException("ComputeSolarHourAngle");
        }
        #endregion
    }
}
