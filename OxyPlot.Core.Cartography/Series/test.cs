using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlot.Series
{
    // https://gis.stackexchange.com/questions/7546/drawing-day-and-night-on-a-google-map
    // https://www.aa.quae.nl/en/antwoorden/zonpositie.html#v526
    // https://community.appinventor.mit.edu/t/how-to-plot-sunrise-sunset-line-on-a-map-and-learn-how-to-use-equations/25832
    // https://math.stackexchange.com/questions/947718/how-to-calculate-civil-twilight-timings
    // http://www.stargazing.net/kepler/sunrise.html
    // https://www.maplesoft.com/support/help/Maple/view.aspx?path=MathApps/DayAndNightTerminator&cid=926
    public class TerminatorHeatMapSeries : FunctionSeries
    {
        private DateTime _utcNow;
        // 4 Lines to draw
        // - Civil twilight
        // - Nautical twilight
        // - Astronomical twilight
        // - Astronomical darkess

        public TerminatorHeatMapSeries()
            : base((t) => ComputeTerminatorTestX(t), (t) => ComputeTerminatorTestY(t), 0, 360, 0.5, "terminator") //-180 + 65, 180 + 64.5, 0.5, "terminator")
        {
            _utcNow = DateTime.UtcNow;
        }

        private static double ComputeTerminatorTestX(double phi)
        {
            return ComputeTerminator(DateTime.UtcNow, phi).L;
        }

        private static double ComputeTerminatorTestY(double phi)
        {
            return ComputeTerminator(DateTime.UtcNow, phi).B * -1.0; // Why negative??
        }

        /// <summary>
        /// Compute Terminator line (day / night).
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="phi">The distance (in degrees) along the terminator from a particular one of its intersections with the equator, in degrees</param>
        /// <returns></returns>
        public static (double L, double B) ComputeTerminator(DateTime utcTime, double phi)
        {
            (double b, double l) = ComputeSunStraightUpPoint(utcTime);

            (double sinPhi, double cosPhi) = Math.SinCos(phi * Math.PI / 180.0);
            (double sinl, double cosl) = Math.SinCos(l * Math.PI / 180.0);
            (double sinb, double cosb) = Math.SinCos(b * Math.PI / 180.0);

            double B = Math.Asin(cosb * sinPhi) * 180.0 / Math.PI; // B is the latitude
            double x = (-cosl * sinb * sinPhi) - (sinl * cosPhi);
            double y = (-sinl * sinb * sinPhi) + (cosl * cosPhi);
            double L = Math.Atan2(y, x) * 180.0 / Math.PI; // L is the longitude

            return (L, B);
        }

        public static (double lat, double lon) ComputeSunStraightUpPoint(DateTime utcTime)
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
            return (lat, lon);
        }

        public static double ComputeSunDeclination(DateTime utcTime)
        {
            // d is the number of days since (the beginning of) the most recent December 31st
            // (i.e., d=1 for midnight at the beginning of January 1st, for January 2nd, and so on).
            int d = utcTime.DayOfYear;
            double M = -3.6 + 0.9856 * d; // M is the solar mean anomaly
            double nu = M + 1.9 * Math.Sin(M * Math.PI / 180.0); // (v)
            double lambda = nu + 102.9;
            double sinLambda = Math.Sin(lambda * Math.PI / 180.0);
            double delta = 22.8 * sinLambda + 0.6 * (sinLambda * sinLambda * sinLambda);
            return delta;
        }
    }
}
