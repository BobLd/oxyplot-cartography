using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Annotations;

namespace OxyPlot.Legends
{
    public partial class MapScaleLegend : LegendBase
    {
        /// <summary>
        /// Initializes a new insance of the Legend class.
        /// </summary>
        public MapScaleLegend()
        {
            this.IsLegendVisible = true;
            this.Key = null;

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

            this.Selectable = true;
            this.SelectionMode = SelectionMode.Single;
        }

        protected override HitTestResult LegendHitTest(HitTestArguments args)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
