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
        private const double _maxDefaultValue = 180;

        /// <summary>
        /// Initializes a new instance of the <see cref="LongitudeAxis"/> class.
        /// <para>
        /// Represents an axis with longitude scale (X axis).
        /// </para>
        /// </summary>
        public LongitudeAxis() : base()
        {
            this.FilterMinValue = -_maxDefaultValue;
            this.FilterMaxValue = _maxDefaultValue;
            this.AbsoluteMinimum = -_maxDefaultValue;
            this.AbsoluteMaximum = _maxDefaultValue;
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
                this.ActualMinimum = -_maxDefaultValue;
            }

            if (this.ActualMinimum <= -_maxDefaultValue)
            {
                this.ActualMinimum = -_maxDefaultValue;
            }

            if (double.IsNaN(this.ActualMaximum) || double.IsInfinity(this.ActualMaximum))
            {
                this.ActualMaximum = _maxDefaultValue;
            }

            if (this.ActualMaximum >= _maxDefaultValue)
            {
                this.ActualMaximum = _maxDefaultValue;
            }

            if (this.ActualMaximum <= this.ActualMinimum)
            {
                this.ActualMaximum = this.ActualMinimum * 100;
            }

            base.CoerceActualMaxMin();
        }
    }
}
