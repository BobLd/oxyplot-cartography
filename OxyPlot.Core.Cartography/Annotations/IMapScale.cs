namespace OxyPlot.Annotations
{
    public interface IMapScale
    {
        /// <summary>
        /// Gets or sets the font. The default is <c>null</c> (use <see cref="PlotModel.DefaultFont"/>.
        /// </summary>
        /// <value>The font.</value>
        /// <remarks>If the value is <c>null</c>, the DefaultFont of the parent MapTileAnnotation will be used.</remarks>
        string? Font { get; set; }

        /// <summary>
        /// Gets or sets the size of the font. The default is <c>double.NaN</c> (use <see cref="PlotModel.DefaultFontSize"/>).
        /// </summary>
        /// <value>The size of the font.</value>
        /// <remarks>If the value is <c>NaN</c>, the DefaultFontSize of the parent MapTileAnnotation will be used.</remarks>
        double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font weight. The default is <c>FontWeights.Normal</c>.
        /// </summary>
        /// <value>The font weight.</value>
        double FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the color of the text. The default is <c>OxyColors.Automatic</c> (use <see cref="PlotModel.TextColor"/>).
        /// </summary>
        /// <value>The color of the text.</value>
        /// <remarks>If the value is <c>OxyColors.Automatic</c>, the TextColor of the parent MapTileAnnotation will be used.</remarks>
        OxyColor? TextColor { get; set; }

        /// <summary>
        /// Gets or sets the edge rendering mode that is used for rendering the plot element.
        /// </summary>
        /// <value>The edge rendering mode. The default is <see cref="EdgeRenderingMode.Automatic"/>.</value>
        EdgeRenderingMode EdgeRenderingMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map scale is visible.
        /// </summary>
        bool IsMapScaleVisible { get; set; }

        Annotation Parent { get; }

        void Render(IRenderContext rc, int zoom, double latitude);

        /// <summary>
        /// Gets the clipping rectangle.
        /// </summary>
        /// <returns>The clipping rectangle.</returns>
        OxyRect GetClippingRect();
    }
}
