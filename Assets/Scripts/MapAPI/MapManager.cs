using System;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.MapAPI
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        
        private const string ApiUrl = "https://api.openstreetmap.org/";
        
        public static async UniTask<Texture2D> GetMapFromCoordinate(int tileX, int tileY, int zoomLevel)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "Unicar / 0.1 development");
            
            HttpResponseMessage response = await client.GetAsync($"http://tile.openstreetmap.org/{zoomLevel}/{tileX}/{tileY}.png");
            byte[] image = await response.Content.ReadAsByteArrayAsync();

            Texture2D texture = new(512, 512); 
            texture.LoadImage(image);
            texture.Apply();

            return texture;
        }

        public static Vector2Int ConvertCoordinateToTile(double latitude, double longitude, int zoomLevel)
        {
            int tileX = ConvertLongitudeToTileX(longitude, zoomLevel);
            int tileY = ConvertLatitudeToTileY(latitude, zoomLevel);

            return new Vector2Int(tileX, tileY);
        }

        public static int ConvertLongitudeToTileX(double longitude, int zoom)
        {
            return (int) Math.Round((longitude + 180.0) / 360.0 * (1 << zoom));
        }

        public static int ConvertLatitudeToTileY(double latitude, int zoom)
        {
            double latRad = latitude / 180.0 * Math.PI;
            return (int) Math.Floor((1 - Math.Log(Math.Tan(latRad) + 1 / Math.Cos(latRad)) / Math.PI) / 2 * (1 << zoom));
        }

        public static double ConvertTileXToLongitude(int x, int zoom)
        {
            return (double) x / (1 << zoom) * 360.0 - 180.0;
        }

        public static double ConvertTileYToLatitude(int y, int zoom)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (1 << zoom);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }
    }
}
