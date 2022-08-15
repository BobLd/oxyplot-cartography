using OxyPlot.Annotations;

namespace OxyPlot.Legends
{
    /// <summary>
    /// Represents a Legend.
    /// </summary>
    public partial class MapTileLegend : LegendBase
    {
        private OxyRect legendBox;

        /// <summary>
        /// Initializes a new insance of the Legend class.
        /// </summary>
        public MapTileLegend()
        {
            this.IsLegendVisible = true;
            this.legendBox = new OxyRect();
            this.Key = null;
            this.GroupNameFont = null;
            this.GroupNameFontWeight = FontWeights.Normal;
            this.GroupNameFontSize = double.NaN;

            this.LegendTitleFont = null;
            this.LegendTitleFontSize = double.NaN;
            this.LegendTitleFontWeight = FontWeights.Bold;
            this.LegendFont = null;
            this.LegendFontSize = double.NaN;
            this.LegendFontWeight = FontWeights.Normal;
            this.LegendSymbolLength = 16;
            this.LegendSymbolMargin = 4;
            this.LegendPadding = 8;
            this.LegendColumnSpacing = 8;
            this.LegendItemSpacing = 24;
            this.LegendLineSpacing = 0;
            this.LegendMargin = 8;

            this.LegendBackground = OxyColors.Undefined;
            this.LegendBorder = OxyColors.Undefined;
            this.LegendBorderThickness = 1;

            this.LegendTextColor = OxyColors.Automatic;
            this.LegendTitleColor = OxyColors.Automatic;

            this.LegendMaxWidth = double.NaN;
            this.LegendMaxHeight = double.NaN;
            this.LegendPlacement = LegendPlacement.Inside;
            this.LegendPosition = LegendPosition.RightTop;
            this.LegendOrientation = LegendOrientation.Vertical;
            this.LegendItemOrder = LegendItemOrder.Normal;
            this.LegendItemAlignment = HorizontalAlignment.Left;
            this.LegendSymbolPlacement = LegendSymbolPlacement.Left;

            this.ShowInvisibleSeries = true;

            this.AnnotationsInvisibleTextColor = OxyColor.FromAColor(64, this.LegendTextColor);

            this.SeriesPosMap = new Dictionary<IMapTileAnnotation, OxyRect>();

            this.Selectable = true;
            this.SelectionMode = SelectionMode.Single;
        }

        /// <summary>
        /// Override for legend hit test.
        /// </summary>
        /// <param name="args">Arguments passe to the hit test</param>
        /// <returns>The hit test results.</returns>
        protected override HitTestResult? LegendHitTest(HitTestArguments args)
        {
            ScreenPoint point = args.Point;
            if (this.IsPointInLegend(point) && this.SeriesPosMap?.Count > 0)
            {
                foreach (KeyValuePair<IMapTileAnnotation, OxyRect> kvp in this.SeriesPosMap)
                {
                    if (kvp.Value.Contains(point) && this.ShowInvisibleSeries)
                    {
                        kvp.Key.IsVisible = !kvp.Key.IsVisible;
                        this.PlotModel.InvalidatePlot(false);
                        break;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the group name font.
        /// </summary>
        public string? GroupNameFont { get; set; }

        /// <summary>
        /// Gets or sets the group name font size.
        /// </summary>
        public double GroupNameFontSize { get; set; }

        /// <summary>
        /// Gets or sets the group name font weight.
        /// </summary>
        public double GroupNameFontWeight { get; set; }

        private Dictionary<IMapTileAnnotation, OxyRect> SeriesPosMap { get; set; }

        /// <summary>
        /// Gets or sets the textcolor of invisible annotations.
        /// </summary>
        public OxyColor AnnotationsInvisibleTextColor { get; set; }

        /// <summary>
        /// Checks if a screen point is within the legend boundaries.
        /// </summary>
        /// <param name="point">A screen point.</param>
        /// <returns>A value indicating whether the point is inside legend boundaries or not.</returns>
        public bool IsPointInLegend(ScreenPoint point)
        {
            return this.legendBox.Contains(point);
        }
    }
}
