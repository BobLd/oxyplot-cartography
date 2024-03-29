﻿using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Data;
using OxyPlot.Legends;
using OxyPlot.Series;
using ReactiveUI;
using System;
using System.IO;
using System.Threading;

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

            var tileMapImageProvider = new HttpTileMapImageProvider(SynchronizationContext.Current)
            {
                //Url = "https://www.metoffice.gov.uk/public/tiles/map/{Z}/{Y}/{X}.png",
                //Url = "https://tile-c.openstreetmap.fr/hot/{Z}/{X}/{Y}.png", // Humanitarian
                //Url = "https://gps.tile.openstreetmap.org/lines/{Z}/{X}/{Y}.png", // Public GPS trace
                Url = "http://tile.openstreetmap.org/{Z}/{X}/{Y}.png", // Standard
                //Url = "https://b.tile-cyclosm.openstreetmap.fr/cyclosm/{Z}/{X}/{Y}.png", // CyclOSM
                //Url = "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}", // https://developers.arcgis.com/documentation/mapping-apis-and-services/data-hosting/services/image-tile-service/
                //Url = "https://maptiles.finncdn.no/tileService/1.0.3/norortho/{Z}/{X}/{Y}.png",
                //Url = "https://maptiles.finncdn.no/tileService/1.0.3/normap/{Z}/{X}/{Y}.png",

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

            var loadingImg = new Uri("avares://SimpleDemo/Assets/live-view.png");
            var asset = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var streamImg = asset.Open(loadingImg))
            {
                // Add the tile map annotation
                model.Annotations.Add(new MapTileAnnotation(streamImg, tileMapImageProvider)
                {
                    CopyrightNotice = "© OpenStreetMap contributors",
                    MinZoomLevel = 0,
                    MaxZoomLevel = 19, // max OpenStreetMap value
                    IsTileGridVisible = true,
                    TileGridThickness = 3
                });
            }

            /*
            // Public GPS trace
            var tileMapImageProvider2 = new HttpTileMapImageProvider(SynchronizationContext.Current)
            {
                //Url = "https://www.metoffice.gov.uk/public/tiles/map/{Z}/{Y}/{X}.png",
                //Url = "https://tile-c.openstreetmap.fr/hot/{Z}/{X}/{Y}.png", // Humanitarian
                Url = "https://gps.tile.openstreetmap.org/lines/{Z}/{X}/{Y}.png", // Public GPS trace
                //Url = "http://tile.openstreetmap.org/{Z}/{X}/{Y}.png", // Standard
                //Url = "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}", // https://developers.arcgis.com/documentation/mapping-apis-and-services/data-hosting/services/image-tile-service/
                //Url = "https://maptiles.finncdn.no/tileService/1.0.3/norortho/{Z}/{X}/{Y}.png",
                //Url = "https://maptiles.finncdn.no/tileService/1.0.3/normap/{Z}/{X}/{Y}.png",

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
                CopyrightNotice = "© OpenStreetMap contributors",
                MinZoomLevel = 0,
                MaxZoomLevel = 19, // max OpenStreetMap value
            });
            */

            return model;
        }
    }
}
