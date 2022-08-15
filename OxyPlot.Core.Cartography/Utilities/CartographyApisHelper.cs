namespace OxyPlot
{
    public struct MapTileApis
    {
        /// <summary>
        /// 
        /// </summary>
        public MapTileApis()
        { }

        public readonly OpenStreetMap OpenStreetMap = new OpenStreetMap();

        public readonly ArcGIS ArcGIS = new ArcGIS();
    }

    public struct OpenStreetMap
    {
        /// <summary>
        /// 
        /// </summary>
        public OpenStreetMap()
        { }

        public readonly MapTileApi Standard { get; } = new MapTileApi("Standard", "http://tile.openstreetmap.org/{Z}/{X}/{Y}.png", "© OpenStreetMap contributors");

        public readonly MapTileApi PublicGpsTrace { get; } = new MapTileApi("Public GPS trace", "https://gps.tile.openstreetmap.org/lines/{Z}/{X}/{Y}.png", "© OpenStreetMap contributors");

        public readonly MapTileApi Humanitarian { get; } = new MapTileApi("Humanitarian", "https://tile-c.openstreetmap.fr/hot/{Z}/{X}/{Y}.png", "© OpenStreetMap contributors");
    }

    public struct FinnCdn
    {
        /// <summary>
        /// 
        /// </summary>
        public FinnCdn()
        { }

        public readonly MapTileApi norortho { get; } = new MapTileApi("norortho", "https://maptiles.finncdn.no/tileService/1.0.3/norortho/{Z}/{X}/{Y}.png", "TODO");

        public readonly MapTileApi normap { get; } = new MapTileApi("normap", "https://maptiles.finncdn.no/tileService/1.0.3/normap/{Z}/{X}/{Y}.png", "TODO");
    }

    public struct ArcGIS
    {
        /// <summary>
        /// 
        /// </summary>
        public ArcGIS()
        { }

        /// <summary>
        /// https://developers.arcgis.com/documentation/mapping-apis-and-services/data-hosting/services/image-tile-service/
        /// </summary>
        public readonly MapTileApi WorldImagery { get; } = new MapTileApi("World_Imagery", "https://server.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{Z}/{Y}/{X}", "TODO");
    }

    public struct MapTileApi
    {
        public MapTileApi(string name, string url, string copyrightNotice)
        {
            Name = name;
            Url = url;
            CopyrightNotice = copyrightNotice;
        }

        public string Name { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string CopyrightNotice { get; init; }
    }
}
