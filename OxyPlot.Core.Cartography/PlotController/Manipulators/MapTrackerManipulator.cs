// --------------------------------------------------------------------------------------------------------------------
// Base on TrackerManipulator.cs
// https://github.com/oxyplot/oxyplot/blob/release/v2.1.0-Preview1/Source/OxyPlot/PlotController/Manipulators/TrackerManipulator.cs
// <summary>
//   Provides a plot manipulator for tracker functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot.Legends;

namespace OxyPlot
{
    /// <summary>
    /// Provides a plot manipulator for tracker functionality.
    /// </summary>
    public class MapTrackerManipulator : MouseManipulator
    {
        /// <summary>
        /// The current series.
        /// </summary>
        private Series.Series? _currentSeries;

        private ScreenPoint _previousResultPosition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTrackerManipulator" /> class.
        /// </summary>
        /// <param name="plotView">The plot view.</param>
        public MapTrackerManipulator(IPlotView plotView)
            : base(plotView)
        {
            Snap = true;
            PointsOnly = true;
            LockToInitialSeries = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show tracker on points only (not interpolating).
        /// </summary>
        public bool PointsOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to snap to the nearest point.
        /// </summary>
        public bool Snap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to lock the tracker to the initial series.
        /// </summary>
        /// <value><c>true</c> if the tracker should be locked; otherwise, <c>false</c>.</value>
        public bool LockToInitialSeries { get; set; }

        /// <summary>
        /// Occurs when a manipulation is complete.
        /// </summary>
        /// <param name="e">The <see cref="OxyMouseEventArgs" /> instance containing the event data.</param>
        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            e.Handled = true;

            _currentSeries = null;
            PlotView.HideTracker();
            PlotView.ActualModel?.RaiseTrackerChanged(null);
        }

        /// <summary>
        /// Occurs when the input device changes position during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="OxyPlot.OxyMouseEventArgs" /> instance containing the event data.</param>
        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);
            e.Handled = true;

            if (_currentSeries == null || !LockToInitialSeries)
            {
                // get the nearest
                _currentSeries = (PlotView.ActualModel?.GetSeriesFromPoint(e.Position, 20));
            }

            if (_currentSeries == null)
            {
                if (!LockToInitialSeries)
                {
                    PlotView.HideTracker();
                }

                return;
            }

            var actualModel = PlotView.ActualModel;
            if (actualModel == null)
            {
                return;
            }

            if (!actualModel.PlotArea.Contains(e.Position.X, e.Position.Y))
            {
                return;
            }

            if (actualModel.IsLegendVisible && actualModel.Legends?.Count > 0)
            {
                foreach (var legendBase in actualModel.Legends)
                {
                    if (legendBase?.LegendArea.Contains(e.Position) == true)
                    {
                        // Do not show tracker if point is in legend
                        // TODO - issue as sometime the position is NOT seen
                        // as inside the legend but actually is
                        return;
                    }
                }
            }

            var result = GetNearestHit(_currentSeries, e.Position, Snap, PointsOnly);
            if (result != null && !_previousResultPosition.Equals(result.Position))
            {
                result.PlotModel = PlotView.ActualModel;
                PlotView.ShowTracker(result);
                PlotView.ActualModel?.RaiseTrackerChanged(result);
                _previousResultPosition = result.Position;
            }
        }

        /// <summary>
        /// Occurs when an input device begins a manipulation on the plot.
        /// </summary>
        /// <param name="e">The <see cref="OxyPlot.OxyMouseEventArgs" /> instance containing the event data.</param>
        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);
            _currentSeries = (PlotView.ActualModel?.GetSeriesFromPoint(e.Position));
            Delta(e);
            _previousResultPosition = new ScreenPoint();
        }

        /// <summary>
        /// Gets the nearest tracker hit.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        /// <param name="snap">Snap to points.</param>
        /// <param name="pointsOnly">Check points only (no interpolation).</param>
        /// <returns>A tracker hit result.</returns>
        private static TrackerHitResult? GetNearestHit(Series.Series series, ScreenPoint point, bool snap, bool pointsOnly)
        {
            if (series == null)
            {
                return null;
            }

            // Check data points only
            if (snap || pointsOnly)
            {
                var result = series.GetNearestPoint(point, false);
                if (result?.Position.DistanceTo(point) < 20)
                {
                    return result;
                }
            }

            // Check between data points (if possible)
            if (!pointsOnly)
            {
                return series.GetNearestPoint(point, true);
            }

            return null;
        }
    }
}
