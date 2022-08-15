using System;
using System.IO;
using System.Threading;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Data;
using OxyPlot.Legends;
using OxyPlot.Series;
using ReactiveUI;

namespace SimpleDemo.ViewModels
{
    public sealed class MapViewModel : ViewModelBase
    {
        private PlotModel _mapPlotView;
        public PlotModel MapPlotView
        {
            get
            {
                return _mapPlotView;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _mapPlotView, value);
            }
        }

        public MapViewModel()
        {
            MapPlotView = CreateMapPlotView();
        }

        private PlotModel CreateMapPlotView()
        {
            var model = new PlotModel
            {
                IsLegendVisible = true,
                PlotType = PlotType.Cartesian
            };

            model.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.RightTop,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black,
                IsLegendVisible = true
            });

            model.Legends.Add(new MapTileLegend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.RightMiddle,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black,
                IsLegendVisible = true
            });

            model.Axes.Add(new LongitudeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -0.24,
                Maximum = 0.04,
                Title = "Longitude",
                //LabelFormatter = (decDegrees) => CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(decDegrees, false, 3)
            });

            model.Axes.Add(new LatitudeWebMercatorAxis
            {
                Position = AxisPosition.Left,
                Minimum = 51.42,
                Maximum = 51.62,
                Title = "Latitude",
                //LabelFormatter = (decDegrees) => CartographyHelper.DecimalDegreesToDegreesMinutesSeconds(decDegrees, true, 3)
            });

            MapTileApi standardApi = CartographyHelper.Apis.OpenStreetMap.Standard;
            var tileMapImageProvider = new HttpTileMapImageProvider(SynchronizationContext.Current!)
            {
                Url = standardApi.Url,
                MaxNumberOfDownloads = 2,
                UserAgent = "OxyPlot.Cartography",
                ImageConverter = new Func<byte[], byte[]>(bytes =>
                {
                    // https://github.com/oxyplot/oxyplot/blob/205e968870c292ecaeab2cb9e7f34904897126cb/Source/OxyPlot/Imaging/OxyImage.cs#L221
                    if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
                    {
                        return bytes; // Bmp
                    }

                    if (bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                    {
                        return bytes; //Png
                    }

                    using (var msInput = new MemoryStream(bytes))
                    using (var msOutput = new MemoryStream())
                    {
                        var bitmap = Bitmap.DecodeToWidth(msInput, 256);
                        bitmap.Save(msOutput);
                        return msOutput.ToArray();
                    }
                })
            };

            var loadingImg = new Uri("avares://SimpleDemo/Assets/live-view.png");
            var asset = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var streamImg = asset.Open(loadingImg))
            {
                // Add the tile map annotation
                model.Annotations.Add(new MapTileAnnotation(streamImg, tileMapImageProvider)
                {
                    Title = standardApi.Name,
                    AnnotationGroupName = "Background",
                    CopyrightNotice = standardApi.CopyrightNotice,
                    MinZoomLevel = 0,
                    MaxZoomLevel = 19, // max OpenStreetMap value
                    IsTileGridVisible = true,
                    TileGridThickness = 3,
                    // Layer Wrong documentation in base class
                });
            }

            // Public GPS trace
            MapTileApi gpsTraceApi = CartographyHelper.Apis.OpenStreetMap.PublicGpsTrace;
            var tileMapImageProvider2 = new HttpTileMapImageProvider(SynchronizationContext.Current)
            {
                Url = gpsTraceApi.Url,
                MaxNumberOfDownloads = 2,
                UserAgent = "OxyPlot.Cartography",
                ImageConverter = new Func<byte[], byte[]>(bytes =>
                {
                    // https://github.com/oxyplot/oxyplot/blob/205e968870c292ecaeab2cb9e7f34904897126cb/Source/OxyPlot/Imaging/OxyImage.cs#L221
                    if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
                    {
                        return bytes; // Bmp
                    }

                    if (bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                    {
                        return bytes; //Png
                    }

                    using (var msInput = new MemoryStream(bytes))
                    using (var msOutput = new MemoryStream())
                    {
                        var bitmap = Bitmap.DecodeToWidth(msInput, 256);
                        bitmap.Save(msOutput);
                        return msOutput.ToArray();
                    }
                    return bytes;
                })
            };

            // Add the tile map annotation
            model.Annotations.Add(new MapTileAnnotation(tileMapImageProvider2)
            {
                Title = gpsTraceApi.Name,
                AnnotationGroupName = "Overlays",
                CopyrightNotice = gpsTraceApi.CopyrightNotice,
                MinZoomLevel = 0,
                MaxZoomLevel = 19, // max OpenStreetMap value
            });

            ScatterSeries scatterSeries = new ScatterSeries()
            {
                Title = "Some data",
                RenderInLegend = true,
                SeriesGroupName = "Group 1"
            };
            for (int x = -10; x <= 10; x++)
            {
                for (int y = 510; y <= 520; y++)
                {
                    scatterSeries.Points.Add(new ScatterPoint(x / 10.0, y / 10.0));
                }
            }
            model.Series.Add(scatterSeries);

            ScatterSeries scatterSeries2 = new ScatterSeries()
            {
                Title = "Some data 2",
                RenderInLegend = true,
                SeriesGroupName = "Group 1"
            };
            for (int x = -11; x <= 11; x++)
            {
                for (int y = 510; y <= 520; y++)
                {
                    scatterSeries2.Points.Add(new ScatterPoint(x / 11.0, y / 10.0));
                }
            }

            model.Series.Add(scatterSeries2);

            ScatterSeries scatterSeries3 = new ScatterSeries()
            {
                Title = "Some data 3",
                RenderInLegend = true,
                SeriesGroupName = "Group 2",
                IsVisible = true
            };
            for (int x = -12; x <= 12; x++)
            {
                for (int y = 520; y <= 530; y++)
                {
                    scatterSeries3.Points.Add(new ScatterPoint(x / 10.0, y / 10.0));
                }
            }

            model.Series.Add(scatterSeries3);

            return model;
        }
    }
}
