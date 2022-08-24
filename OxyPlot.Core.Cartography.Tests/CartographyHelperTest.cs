namespace OxyPlot.Core.Cartography.Tests
{
    public class CartographyHelperTest
    {
        [Fact]
        public void DecimalDegreesToDegreesMinutesSecondsTest()
        {
            var dms = CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(38.8897, true, 0);
            Assert.Equal("38\u00b053\u203223\u2033N", dms);

            var dms1 = CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(-77.0089, false, 0);
            Assert.Equal("77\u00b000\u203232\u2033W", dms1);
        }

        [Fact]
        public void DegreesMinutesSecondsToDecimalDegreesTests()
        {
            var dec = CartographyHelper.DegreesMinutesSecondsToDecimalDegrees(38, 53, 23, 'N');
            Assert.Equal(38.8897, dec, 4);

            var dec1 = CartographyHelper.DegreesMinutesSecondsToDecimalDegrees(77, 0, 32, 'W');
            Assert.Equal(-77.0089, dec1, 4);
        }

        [Fact]
        public void DegreesMinutesSecondsToDecimalDegreesTests2()
        {
            var dec = CartographyHelper.DegreesMinutesSecondsToDecimalDegrees("38\u00b053\u203223\u2033N");
            Assert.Equal(38.8897, dec, 4);

            var dec1 = CartographyHelper.DegreesMinutesSecondsToDecimalDegrees("77\u00b000\u203232\u2033W");
            Assert.Equal(-77.0089, dec1, 4);
        }

        [Fact]
        public void ComputeSunDeclinationTest()
        {
            var degree = CartographyHelper.ComputeSunDeclination(DateTime.UtcNow);
            var dms = CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(degree, true, 0);
        }

        [Fact]
        public void ComputeSunStraightUpPointTest()
        {
            var coord = CartographyHelper.ComputeSunStraightUpPoint(DateTime.UtcNow);
        }

        [Fact]
        public void ComputeDayNumberOriginTest()
        {
            var d0 = CartographyHelper.ComputeDayNumber(new DateTime(1999, 12, 31)); // 0
            Assert.Equal(0, d0);
        }

        [Fact]
        public void TutorialTest()
        {
            var refDate = new DateTime(1990, 04, 19);
            // http://www.stjarnhimlen.se/comp/tutorial.html#5
            var d = CartographyHelper.ComputeDayNumber(refDate);
            Assert.Equal(-3543, d);

            double M = CartographyHelper.ComputeSunMeanAnomaly(refDate);
            Assert.Equal(104.0653, M, 4);

            double M1 = CartographyHelper.ComputeSunMeanAnomaly(d);
            Assert.Equal(104.0653, M1, 4);

            double w = CartographyHelper.ComputeSunArgumentOfPerihelion(d);
            Assert.Equal(282.7735, w, 4);

            double e = CartographyHelper.ComputeSunEccentricity(refDate);
            Assert.Equal(0.016713, e, 6);

            double e1 = CartographyHelper.ComputeSunEccentricity(d);
            Assert.Equal(0.016713, e1, 6);

            double oblecl = CartographyHelper.ComputeEarthObliquityEcliptic(d);
            Assert.Equal(23.4406, oblecl, 4);

            double E = CartographyHelper.ComputeSunEccentricAnomaly(refDate);
            Assert.Equal(104.9904, E, 4);

            double E1 = CartographyHelper.ComputeSunEccentricAnomaly(d);
            Assert.Equal(104.9904, E1, 4);

            double E2 = CartographyHelper.ComputeSunEccentricAnomaly(M, e);
            Assert.Equal(104.9904, E2, 4);

            (double RA, double Dec) = CartographyHelper.ComputeSunPosition(refDate);
            Assert.Equal(26.6580, RA, 3);
            Assert.Equal(11.0084, Dec, 4);

            (double RA1, double Dec1) = CartographyHelper.ComputeSunPosition(d);
            Assert.Equal(26.6580, RA1, 3);
            Assert.Equal(11.0084, Dec1, 4);

            (double RA2, double Dec2) = CartographyHelper.ComputeSunPosition(M, e, w, oblecl);
            Assert.Equal(26.6580, RA2, 3);
            Assert.Equal(11.0084, Dec2, 4);

            var RightAsc = TimeSpan.FromHours(RA1 / 15.0); // 1h 46m 37.9s
            var decl = CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(Dec1, true, 0); // +11_deg 0' 30"
        }

        [Fact]
        public void SunNowTest()
        {
            (double RA, double Dec) = CartographyHelper.ComputeSunPosition(DateTime.UtcNow);

            var rightAsc = TimeSpan.FromHours(RA / 15.0);
            var decl = CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(Dec, true, 0);
            var sunPos = CartographyHelper.ComputeSunStraightUpPoint2(DateTime.UtcNow);
        }
    }
}