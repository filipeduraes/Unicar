using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unicar.MapAPI.Models;
using UnityEngine;

namespace Unicar.MapAPI
{
    public static class MapManager
    {
        private const string APIKey = "AIzaSyDimHeI1LRuUK3BrweuRUdnmIa7pzhWy6c";
        private static HttpClient _client;
        private static GetSessionTokenResult _sessionToken;
        
        public static async UniTask<Texture2D> GetMapFromCoordinateAsync(Vector3 coordinate)
        {
            using HttpClient client = await GetClientAsync();
            HttpResponseMessage result = await client.GetAsync($"https://tile.googleapis.com/v1/2dtiles/{coordinate.z}/{coordinate.x}/{coordinate.y}?session={_sessionToken.Session}&key={APIKey}&orientation=0");
            string resultJson = await result.Content.ReadAsStringAsync();

            return null;
        }

        private static async UniTask<HttpClient> GetClientAsync()
        {
            if (_client == null)
                _sessionToken = await InitializeClientAsync();

            return _client;
        }

        private static async UniTask<GetSessionTokenResult> InitializeClientAsync()
        {
            _client = new HttpClient();
            
            GetSessionTokenModel model = new("roadmap", "en-US", "US");
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await _client.PostAsync(new Uri($"https://tile.googleapis.com/v1/createSession?key={APIKey}"), httpContent);
            string resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetSessionTokenResult>(resultJson);
        }
    }
}
