namespace OxyPlot.Axes
{
    /// <summary>
    /// Represents an axis with longitude scale.
    /// https://en.wikipedia.org/wiki/Web_Mercator_projection
    /// </summary>
    public class LongitudeAxis : LinearAxis
    {
        private const double _maxDefaultValue = 180;

        public LongitudeAxis() : base()
        {
            this.FilterMinValue = -_maxDefaultValue;
            this.FilterMaxValue = _maxDefaultValue;
            this.AbsoluteMinimum = -_maxDefaultValue;
            this.AbsoluteMaximum = _maxDefaultValue;
            Key = "Longitude";
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
