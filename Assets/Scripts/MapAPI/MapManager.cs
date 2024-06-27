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
        private const string ApiUrl = "https://api.openstreetmap.org/";
        [SerializeField] private double latitude;
        [SerializeField] private double longitude;
        [SerializeField, Range(0, 18)] private int zoom;
        [SerializeField] private RawImage rawImage;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            UniTask getMapTask = GetMapFromCoordinate();
            getMapTask.AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public async UniTask GetMapFromCoordinate()
        {
            using HttpClient client = new();
            
            client.DefaultRequestHeaders.Add("User-Agent", "Unicar / 0.1 development");

            int tileX = ConvertLongitudeToTileX(longitude, zoom);
            int tileY = ConvertLatitudeToTileY(latitude, zoom);
            
            HttpResponseMessage response = await client.GetAsync($"http://tile.openstreetmap.org/{zoom}/{tileX}/{tileY}.png");
            byte[] image = await response.Content.ReadAsByteArrayAsync();

            Texture2D texture = new(512, 512); 
            texture.LoadImage(image);
            texture.Apply();

            rawImage.texture = texture;
        }

        private int ConvertLongitudeToTileX(double longitude, int zoom)
        {
            return (int) Math.Round((longitude + 180.0) / 360.0 * (1 << zoom));
        }

        private int ConvertLatitudeToTileY(double latitude, int zoom)
        {
            double latRad = latitude / 180.0 * Math.PI;
            return (int) Math.Floor((1 - Math.Log(Math.Tan(latRad) + 1 / Math.Cos(latRad)) / Math.PI) / 2 * (1 << zoom));
        }

        private double ConvertTileXToLongitude(int x, int zoom)
        {
            return (double) x / (1 << zoom) * 360.0 - 180.0;
        }

        private double ConvertTileYToLatitude(int y, int zoom)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (1 << zoom);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }
    }
}
