# OxyPlot.Cartography
![example-openstreetmap](https://user-images.githubusercontent.com/38405645/184510801-c255316f-6dbb-4955-a064-c2b744c3cb28.png)

# Usage
The below example is implemented using Avalonia, but it will be very similar for other platforms.
```csharp
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
});

model.Axes.Add(new LatitudeWebMercatorAxis
{
	Position = AxisPosition.Left,
	Minimum = 51.42,
	Maximum = 51.62,
	Title = "Latitude"
});

var tileMapImageProvider = new HttpTileMapImageProvider(SynchronizationContext.Current)
{
	Url = "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}",
	MaxNumberOfDownloads = 2,
	UserAgent = "OxyPlot.Cartography",
	ImageConverter = new Func<byte[], byte[]>(input =>
	{
		// Only convert if file format is Jpeg
		if (input.Length >= 2 && input[0] == 0xFF && input[1] == 0xD8)
		{
			using (var msInput = new MemoryStream(input))
			using (var msOutput = new MemoryStream())
			{
				var bitmap = Bitmap.DecodeToWidth(msInput, 256);
				bitmap.Save(msOutput);
				return msOutput.ToArray();
			}
		}
		return input;
	})
};

var loadingImg = new Uri("avares://SimpleDemo/Assets/live-view.png");
var asset = AvaloniaLocator.Current.GetService<IAssetLoader>();
using (var streamImg = asset.Open(loadingImg))
{
	// Add the tile map annotation
	model.Annotations.Add(new MapTileAnnotation(streamImg, tileMapImageProvider)
	{
		CopyrightNotice = "OpenStreetMap",
		MinZoomLevel = 0,
		MaxZoomLevel = 19, // max OpenStreetMap value
		IsTileGridVisible = true,
		TileGridThickness = 3
	});
}
```

## Map Tiles
### MapTileAnnotation
![example-arcgisonline-sat](https://user-images.githubusercontent.com/38405645/184510820-e1616724-46e5-46a8-8169-fa408bb26d8d.png)


#### Usage
```csharp
model.Annotations.Add(new MapTileAnnotation(streamImg, tileMapImageProvider)
{
	CopyrightNotice = "OpenStreetMap",
	MinZoomLevel = 0,
	MaxZoomLevel = 19, // max OpenStreetMap value
	IsTileGridVisible = true,
	TileGridThickness = 3
});
```
#### Setting a tile loading image
![loading_images](https://user-images.githubusercontent.com/38405645/184518394-55c89ba6-2a36-4a0a-97f7-e1785dfb5b0f.png)

##### Usage (Avalonia)
```csharp
var loadingImg = new Uri("avares://SimpleDemo/Assets/live-view.png");
var asset = AvaloniaLocator.Current.GetService<IAssetLoader>();
using (var streamImg = asset.Open(loadingImg))
{
	// Add the tile map annotation
	model.Annotations.Add(new MapTileAnnotation(streamImg, tileMapImageProvider)
	{
		CopyrightNotice = "OpenStreetMap",
		MinZoomLevel = 0,
		MaxZoomLevel = 19, // max OpenStreetMap value
		IsTileGridVisible = true,
		TileGridThickness = 3
	});
}
```

## Axis
### LatitudeWebMercatorAxis
Map tiles are rendered as true squares, axis is not linear. You can use it in combination with `LongitudeAxis`, which is basically a `LinearAxis`.
> **Spherical Pseudo-Mercator projection**<br>
> Most of OSM, including the main tiling system, uses a Pseudo-Mercator projection where the Earth is modelized as if it was a perfect a sphere. Combined with the zoom level, the system is known as a Web Mercator on Wikipedia.<br>
> This produces a fast approximation to the truer, but heavier elliptical projection, where the Earth would be projected on a more accurate ellipsoid (flattened on poles). As a consequence, direct mesurements of distances in this projection will be approximative, except on the Equator, and the aspect ratios on the rendered map for true squares measured on the surface on Earth will slightly change with latitude and angles not so precisely preserved by this spherical projection.
https://wiki.openstreetmap.org/wiki/Mercator
![example-openstreetmap-latitude-mercator-axis](https://user-images.githubusercontent.com/38405645/184510844-ef039e92-198f-471c-bfd1-b7c141aa2995.png)


#### Usage
```csharp
model.Axes.Add(new LatitudeWebMercatorAxis
{
	Position = AxisPosition.Left,
	Minimum = 51.42,
	Maximum = 51.62,
	Title = "Latitude"
});
```

### LinearAxis
When using the basic Oxyplot `LinearAxis`, the map tiles are not rendered as true squares.
![example-openstreetmap-linear-axis](https://user-images.githubusercontent.com/38405645/184510852-5003c17e-4b7d-4de7-a248-1855a3fcb014.png)

## Data
### LocalTileMapImageProvider
#### Usage
```csharp
var tileMapImageProvider = new LocalTileMapImageProvider(pathToFolder);
```
### HttpTileMapImageProvider
#### Usage
```csharp
var tileMapImageProvider = new HttpTileMapImageProvider(SynchronizationContext.Current)
{
	Url = "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}",
	MaxNumberOfDownloads = 2,
	UserAgent = "OxyPlot.Cartography"
};
```

### Unsuported image format by OxyPlot (mainly Jpeg)
#### Usage (Avalonia)
```csharp
var tileMapImageProvider = new HttpTileMapImageProvider(SynchronizationContext.Current)
{
	Url = "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}",
	MaxNumberOfDownloads = 2,
	UserAgent = "OxyPlot.Cartography",
	ImageConverter = new Func<byte[], byte[]>(input =>
	{
		// Only convert if file format is Jpeg
		if (input.Length >= 2 && input[0] == 0xFF && input[1] == 0xD8)
		{
			using (var msInput = new MemoryStream(input))
			using (var msOutput = new MemoryStream())
			{
				var bitmap = Bitmap.DecodeToWidth(msInput, 256);
				bitmap.Save(msOutput);
				return msOutput.ToArray();
			}
		}
		return input;
	})
};
```

## Tile Images APIs
- http://tile.openstreetmap.org/{Z}/{X}/{Y}.png
- https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}
   - https://developers.arcgis.com/documentation/mapping-apis-and-services/data-hosting/services/image-tile-service/
- https://maptiles.finncdn.no/tileService/1.0.3/norortho/{Z}/{X}/{Y}.png
- https://maptiles.finncdn.no/tileService/1.0.3/normap/{Z}/{X}/{Y}.png

## References
- https://stackoverflow.com/questions/39101368/oxyplot-how-to-programmatically-get-the-scale-of-a-linearaxis-and-use-it-in-an
- https://gis.stackexchange.com/questions/110730/mercator-scale-factor-is-changed-along-the-meridians-as-a-function-of-latitude
- https://wiki.openstreetmap.org/wiki/Mercator
- https://en.wikipedia.org/wiki/Web_Mercator_projection
