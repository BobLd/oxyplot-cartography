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
    }
}