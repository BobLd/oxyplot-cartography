using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;

namespace SimpleDemo
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            // Below is for https://github.com/AvaloniaUI/Avalonia/issues/6481
            GC.KeepAlive(typeof(OxyPlot.MapTrackerManipulator));

            return AppBuilder.Configure<App>()
                   .UsePlatformDetect()
                   .LogToTrace()
                   .UseReactiveUI();
        }
    }
}
