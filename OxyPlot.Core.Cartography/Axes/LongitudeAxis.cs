namespace OxyPlot.Axes
{
    /// <summary>
    /// Represents an axis with longitude scale (X axis).
    /// <para>
    /// <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection"/>
    /// </para>
    /// </summary>
    public class LongitudeAxis : LinearAxis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LongitudeAxis"/> class.
        /// <para>
        /// Represents an axis with longitude scale (X axis).
        /// </para>
        /// </summary>
        public LongitudeAxis() : base()
        {
            this.FilterMinValue = -CartographyHelper.MaxLongitude;
            this.FilterMaxValue = CartographyHelper.MaxLongitude;
            this.AbsoluteMinimum = -CartographyHelper.MaxLongitude;
            this.AbsoluteMaximum = CartographyHelper.MaxLongitude;
            Key = "Longitude";
            StringFormat = "00.0###°E;00.0###°W";
        }

        /// <summary>
        /// Coerces the actual maximum and minimum values.
        /// </summary>
        protected override void CoerceActualMaxMin()
        {
            if (double.IsNaN(this.ActualMinimum) || double.IsInfinity(this.ActualMinimum))
            {
                this.ActualMinimum = -CartographyHelper.MaxLongitude;
            }

            if (this.ActualMinimum <= -CartographyHelper.MaxLongitude)
            {
                this.ActualMinimum = -CartographyHelper.MaxLongitude;
            }

            if (double.IsNaN(this.ActualMaximum) || double.IsInfinity(this.ActualMaximum))
            {
                this.ActualMaximum = CartographyHelper.MaxLongitude;
            }

            if (this.ActualMaximum >= CartographyHelper.MaxLongitude)
            {
                this.ActualMaximum = CartographyHelper.MaxLongitude;
            }

            if (this.ActualMaximum <= this.ActualMinimum)
            {
                this.ActualMaximum = this.ActualMinimum * 100;
            }

            base.CoerceActualMaxMin();
        }
    }
}
