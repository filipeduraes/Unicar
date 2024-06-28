using System;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.MapAPI
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        
        private const string ApiUrl = "https://api.openstreetmap.org/";
        
        public static async UniTask<Texture2D> GetMapFromCoordinate(TilePoint tile, int zoomLevel)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "Unicar / 0.1 development");

            string requestUri = $"http://tile.openstreetmap.org/{zoomLevel}/{tile.x}/{tile.y}.png";
            Debug.Log(requestUri);
            
            HttpResponseMessage response = await client.GetAsync(requestUri);
            byte[] image = await response.Content.ReadAsByteArrayAsync();

            Texture2D texture = new(256, 256); 
            texture.LoadImage(image);
            texture.Apply();

            return texture;
        }
        
        public static TilePoint ConvertCoordinateToTile(double latitude, double longitude, int zoomLevel)
        {
            long tileX = ConvertLongitudeToTileX(longitude, zoomLevel);
            long tileY = ConvertLatitudeToTileY(latitude, zoomLevel);

            return new TilePoint(tileX, tileY, zoomLevel);
        }

        public static long ConvertLongitudeToTileX(double longitude, int zoom)
        {
            return (long) Math.Floor((longitude + 180.0) / 360.0 * (1 << zoom));
        }

        public static long ConvertLatitudeToTileY(double latitude, int zoom)
        {
            double latitudeRadian = latitude * Math.PI / 180.0;
            long tileY = (long) Math.Floor((1 - Math.Log(Math.Tan(latitudeRadian) + 1 / Math.Cos(latitudeRadian)) / Math.PI) / 2 * (1 << zoom));
            
            return tileY;
        }

        public static double ConvertTileXToLongitude(long x, int zoom)
        {
            return (double) x / (1 << zoom) * 360.0 - 180.0;
        }

        public static double ConvertTileYToLatitude(long y, int zoom)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (1 << zoom);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }
    }

    public struct TilePoint
    {
        public readonly long x;
        public readonly long y;
        public readonly int zoom;

        public TilePoint(long x, long y, int zoom)
        {
            this.x = x;
            this.y = y;
            this.zoom = zoom;
        }

        public static TilePoint operator +(TilePoint aTilePoint, TilePoint bTilePoint)
        {
            return new TilePoint(aTilePoint.x + bTilePoint.x, aTilePoint.y + bTilePoint.y, aTilePoint.zoom);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(zoom, x, y);
        }
    }
}
