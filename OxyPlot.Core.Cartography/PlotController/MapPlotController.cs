namespace OxyPlot
{
    /// <summary>
    /// Provides an <see cref="IPlotController" /> with a set of map plot bindings.
    /// </summary>
    public sealed class MapPlotController : PlotController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPlotController"/> class.
        /// </summary>
        public MapPlotController() : base()
        {
            this.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            this.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);

            this.BindMouseEnter(new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) =>
                controller.AddHoverManipulator(view, new MapTrackerManipulator(view), args)));

            this.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, PlotCommands.ZoomRectangle);

            this.UnbindMouseDown(OxyMouseButton.Middle);
            this.UnbindMouseDown(OxyMouseButton.Right);
            this.UnbindKeyDown(OxyKey.C, OxyModifierKeys.Control | OxyModifierKeys.Alt);
            this.UnbindKeyDown(OxyKey.R, OxyModifierKeys.Control | OxyModifierKeys.Alt);
            this.UnbindKeyDown(OxyKey.Up);
            this.UnbindKeyDown(OxyKey.Down);
            this.UnbindKeyDown(OxyKey.Left);
            this.UnbindKeyDown(OxyKey.Right);

            /*
            this.UnbindKeyDown(OxyKey.Up, OxyModifierKeys.Control);
            this.UnbindKeyDown(OxyKey.Down, OxyModifierKeys.Control);
            this.UnbindKeyDown(OxyKey.Left, OxyModifierKeys.Control);
            this.UnbindKeyDown(OxyKey.Right, OxyModifierKeys.Control);
            */
        }
    }
}
