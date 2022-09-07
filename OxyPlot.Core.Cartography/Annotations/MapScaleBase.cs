namespace OxyPlot.Annotations
{
    public abstract class MapScaleBase : IMapScale
    {
        public string? Font { get; set; }

        public double FontSize { get; set; } = double.NaN;

        public double FontWeight { get; set; } = double.NaN;

        public OxyColor? TextColor { get; set; }

        public EdgeRenderingMode EdgeRenderingMode { get; set; } = EdgeRenderingMode.Automatic;

        public bool IsMapScaleVisible { get; set; } = true;

        public Annotation Parent { get; internal set; }

        /// <summary>
        /// Gets the actual font.
        /// </summary>
        protected internal string ActualFont
        {
            get
            {
                return this.Font ?? this.Parent.PlotModel.DefaultFont;
            }
        }

        /// <summary>
        /// Gets the actual size of the font.
        /// </summary>
        /// <value>The actual size of the font.</value>
        protected internal double ActualFontSize
        {
            get
            {
                return !double.IsNaN(this.FontSize) ? this.FontSize : this.Parent.PlotModel.DefaultFontSize;
            }
        }

        /// <summary>
        /// Gets the actual font weight.
        /// </summary>
        protected internal double ActualFontWeight
        {
            get
            {
                return this.FontWeight;
            }
        }

        /// <summary>
        /// Gets the actual color of the text.
        /// </summary>
        /// <value>The actual color of the text.</value>
        protected internal OxyColor ActualTextColor
        {
            get
            {
                return this.TextColor ?? this.Parent.PlotModel.TextColor;
            }
        }

        public abstract OxyRect GetClippingRect();

        public abstract void Render(IRenderContext rc, int zoom, double latitude);
    }
}
