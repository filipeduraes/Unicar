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
        [SerializeField] private string x;
        [SerializeField] private string y;
        [SerializeField] private string zoom;
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

            HttpResponseMessage response = await client.GetAsync($"http://tile.openstreetmap.org/{zoom}/{x}/{y}.png");
            byte[] image = await response.Content.ReadAsByteArrayAsync();

            Texture2D texture = new(256, 256); 
            texture.LoadImage(image);
            texture.Apply();

            rawImage.texture = texture;
        }
    }
}
