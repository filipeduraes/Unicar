using Cysharp.Threading.Tasks;
using Unicar.MapAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.RouteSystem
{
    public class MapDrawer : MonoBehaviour
    {
        [SerializeField] private RawImage mapImage;

        private void Awake()
        {
            Input.location.Start();
            

            _ = RenderMapAsync();
            
            Input.location.Stop();
        }

        private async UniTask RenderMapAsync()
        {
            LocationInfo locationInfo = Input.location.lastData;
            Vector3 coordinate = new(locationInfo.latitude, locationInfo.altitude, locationInfo.longitude);

            await UniTask.WaitUntil(() => Input.location.status != LocationServiceStatus.Initializing);
            
            Texture2D mapTexture = await MapManager.GetMapFromCoordinateAsync(coordinate);
            
            mapImage.texture = mapTexture;
        }
    }
}
