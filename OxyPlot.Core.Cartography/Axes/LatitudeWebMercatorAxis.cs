namespace OxyPlot.Axes
{
    /// <summary>
    /// Represents an axis with Web Mercator latitude scale (Y axis).
    /// <para>
    /// Web Mercator, Google Web Mercator, Spherical Mercator, WGS 84 Web Mercator or WGS 84/Pseudo-Mercator
    /// is a variant of the Mercator map projection and is the de facto standard for Web mapping applications.
    /// It rose to prominence when Google Maps adopted it in 2005. It is used by virtually all major online
    /// map providers, including Google Maps, CARTO, Mapbox, Bing Maps, OpenStreetMap, Mapquest, Esri, and
    /// many others. Its official EPSG identifier is EPSG:3857, although others have been used historically.
    /// </para>
    /// <see href="https://en.wikipedia.org/wiki/Web_Mercator_projection"/>
    /// </summary>
    public class LatitudeWebMercatorAxis : Axis
    {
        private readonly System.Reflection.MethodInfo? _updateActualMaxMinMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="LatitudeWebMercatorAxis"/> class.
        /// <para>
        /// Represents an axis with Web Mercator latitude scale (Y axis).
        /// </para>
        /// </summary>
        public LatitudeWebMercatorAxis()
        {
            FilterMinValue = -CartographyHelper.MaxMercatorProjectionLatitude;
            FilterMaxValue = CartographyHelper.MaxMercatorProjectionLatitude;
            AbsoluteMinimum = -CartographyHelper.MaxMercatorProjectionLatitude;
            AbsoluteMaximum = CartographyHelper.MaxMercatorProjectionLatitude;
            Key = "Latitude";
            StringFormat = "00.0###°N;00.0###°S";

            // Hack as 'UpdateActualMaxMin' method is internal
            _updateActualMaxMinMethod = typeof(Axis).GetMethod("UpdateActualMaxMin",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ArgumentNullException.ThrowIfNull(_updateActualMaxMinMethod);
        }

        /// <summary>
        /// Updates the <see cref="Axis.ActualMaximum"/> and <see cref="Axis.ActualMinimum"/> values.
        /// </summary>
        /// <remarks>If the user has zoomed/panned the axis, the internal ViewMaximum/ViewMinimum
        /// values will be used. If Maximum or Minimum have been set, these values will be used. Otherwise the maximum and minimum values
        /// of the series will be used, including the 'padding'.</remarks>
        protected void UpdateActualMaxMin()
        {
            _updateActualMaxMinMethod!.Invoke(this, null);
        }

        /// <summary>
        /// Determines whether the axis is used for X/Y values.
        /// </summary>
        /// <returns><c>true</c> if it is an XY axis; otherwise, <c>false</c>.</returns>
        public override bool IsXyAxis()
        {
            return true;
        }

        /// <summary>
        /// Determines whether the axis is logarithmic.
        /// </summary>
        /// <returns><c>true</c> if it is a logarithmic axis; otherwise, <c>false</c>.</returns>
        public override bool IsLogarithmic()
        {
            return false;
        }

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="ppt">The previous point (screen coordinates).</param>
        /// <param name="cpt">The current point (screen coordinates).</param>
        public override void Pan(ScreenPoint ppt, ScreenPoint cpt)
        {
            if (!IsPanEnabled)
            {
                return;
            }

            bool isHorizontal = IsHorizontal();
            if (isHorizontal)
            {
                throw new InvalidOperationException("Latitude Axis must be vertical.");
            }

            if (Math.Abs(cpt.Y - ppt.Y) < double.Epsilon)
            {
                return;
            }

            var oldMinimum = ActualMinimum;
            var oldMaximum = ActualMaximum;

            double iCptY = InverseTransform(cpt.Y);
            double px = (iCptY - InverseTransform(ppt.Y)) * CartographyHelper.GetMercatorAdj(iCptY);

            var dx0 = PreTransform(ActualMinimum) - px;
            var dx1 = PreTransform(ActualMaximum) - px;
            var newMinimum = PostInverseTransform(dx0);
            var newMaximum = PostInverseTransform(dx1);

            if (newMinimum < AbsoluteMinimum)
            {
                newMinimum = AbsoluteMinimum;
                newMaximum = Math.Min(newMinimum + ActualMaximum - ActualMinimum, AbsoluteMaximum);
            }

            if (newMaximum > AbsoluteMaximum)
            {
                newMaximum = AbsoluteMaximum;
                newMinimum = Math.Max(newMaximum - (ActualMaximum - ActualMinimum), AbsoluteMinimum);
            }

            ViewMinimum = newMinimum;
            ViewMaximum = newMaximum;
            UpdateActualMaxMin();

            var deltaMinimum = ActualMinimum - oldMinimum;
            var deltaMaximum = ActualMaximum - oldMaximum;

            OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Pan, deltaMinimum, deltaMaximum));
        }

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public override void Pan(double delta)
        {
            if (!IsPanEnabled || Math.Abs(delta) < double.Epsilon)
            {
                return;
            }

            var midLat = (ScreenMax.Y + ScreenMin.Y) / 2.0;
            Pan(new ScreenPoint(0, midLat), new ScreenPoint(0, midLat + delta));
        }

        /// <summary>
        /// <inheritdoc/>
        /// Does nothing in <see cref="LatitudeWebMercatorAxis"/>.
        /// </summary>
        /// <param name="newScale"></param>
        public override void Zoom(double newScale)
        {
            // There is an issue when it's a Cartesian plot type
            // We don't do the zoom using this function
            // It seems there's no real impact appart from fixing the issue...
        }

        /// <summary>
        /// Inverse transforms the specified screen coordinate. This method can only be used with non-polar coordinate systems.
        /// </summary>
        /// <param name="sx">The screen coordinate.</param>
        /// <returns>The value.</returns>
        public override double InverseTransform(double sx)
        {
            return CartographyHelper.PseudoMercatorProjectionYToLatitude((sx / Scale) + Offset);
        }

        /// <summary>
        /// Transforms the specified coordinate to screen coordinates.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The transformed value (screen coordinate).</returns>
        public override double Transform(double x)
        {
            return (CartographyHelper.PseudoMercatorProjectionLatitudeToY(x) - Offset) * Scale;
        }

        /// <summary>
        /// Applies a transformation after the inverse transform of the value. This is used in latitude axis.
        /// </summary>
        /// <param name="x">The value to transform.</param>
        /// <returns>The transformed value.</returns>
        protected override double PostInverseTransform(double x)
        {
            return CartographyHelper.PseudoMercatorProjectionYToLatitude(x);
        }

        /// <summary>
        /// Applies a transformation before the transform the value. This is used in latitude axis.
        /// </summary>
        /// <param name="x">The value to transform.</param>
        /// <returns>The transformed value.</returns>
        protected override double PreTransform(double x)
        {
            return CartographyHelper.PseudoMercatorProjectionLatitudeToY(x);
        }

        /// <inheritdoc/>
        protected override void CoerceActualMaxMin()
        {
            if (double.IsNaN(ActualMinimum) || double.IsInfinity(ActualMinimum))
            {
                ActualMinimum = -CartographyHelper.MaxMercatorProjectionLatitude;
            }

            if (ActualMinimum <= -CartographyHelper.MaxMercatorProjectionLatitude)
            {
                ActualMinimum = -CartographyHelper.MaxMercatorProjectionLatitude;
            }

            if (double.IsNaN(ActualMaximum) || double.IsInfinity(ActualMaximum))
            {
                ActualMaximum = CartographyHelper.MaxMercatorProjectionLatitude;
            }

            if (ActualMaximum >= CartographyHelper.MaxMercatorProjectionLatitude)
            {
                ActualMaximum = CartographyHelper.MaxMercatorProjectionLatitude;
            }

            if (ActualMaximum <= ActualMinimum)
            {
                ActualMaximum = ActualMinimum * 100;
            }

            base.CoerceActualMaxMin();
        }
    }
}
