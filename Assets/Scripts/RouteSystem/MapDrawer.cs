using Cysharp.Threading.Tasks;
using Unicar.MapAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.RouteSystem
{
    public class MapDrawer : MonoBehaviour
    {
        [SerializeField] private RawImage mapImage;
        [SerializeField] private Vector3 coordinate;

        private void Awake()
        {
            _ = RenderMapAsync();
            
        }

        private async UniTask RenderMapAsync()
        {
            Texture2D mapTexture = await MapManager.GetMapFromCoordinateAsync(coordinate);
            mapImage.texture = mapTexture;
        }
    }
}
