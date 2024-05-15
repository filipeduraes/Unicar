namespace Unicar.MapAPI.Models
{
    public record GetSessionTokenModel(string mapType, string language, string region, string scale = "scaleFactor1x")
    {
        public string mapType { get; } = mapType;
        public string language { get; } = language;
        public string region { get; } = region;
        public string scale { get; } = scale;
    }

    public record GetSessionTokenResult(string session, string expiry, string imageFormat, int tileWidth, int tileHeight)
    {
        public string session { get; } = session;
        public string expiry { get; } = expiry;
        public string imageFormat { get; } = imageFormat;
        public int tileWidth { get; } = tileWidth;
        public int tileHeight { get; } = tileHeight;
    }
    
    //public record 
}