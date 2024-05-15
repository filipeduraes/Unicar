namespace Unicar.MapAPI.Models
{
    public record GetSessionTokenModel(string MapType, string Language, string Region)
    {
        public string MapType { get; } = MapType;
        public string Language { get; } = Language;
        public string Region { get; } = Region;
    }

    public record GetSessionTokenResult(string Session, string Expiry, string ImageFormat, int TileWidth, int TileHeight)
    {
        public string Session { get; } = Session;
        public string Expiry { get; } = Expiry;
        public string ImageFormat { get; } = ImageFormat;
        public int TileWidth { get; } = TileWidth;
        public int TileHeight { get; } = TileHeight;
    }
}