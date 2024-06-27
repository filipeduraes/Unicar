using System.Threading;
using Cysharp.Threading.Tasks;
using Unicar.MapAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.UI.RidesScreen.View
{
    public class MapDrawer : MonoBehaviour
    {
        [SerializeField] private RectTransform mapCenter;
        [SerializeField] private double longitude;
        [SerializeField] private double latitude;
        [SerializeField] private int zoomLevel = 10;
        
        private readonly RawImage[,] _tileImages = new RawImage[3, 3];
        private CancellationTokenSource _cancellationTokenSource;
        private Vector2Int _initialTile;

        private void Awake()
        {
            GenerateTileImages();

            _cancellationTokenSource = new CancellationTokenSource();
            
            UniTask populateTask = PopulateMapTexturesAsync();
            populateTask.AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async UniTask PopulateMapTexturesAsync()
        {
            _initialTile = MapManager.ConvertCoordinateToTile(latitude, longitude, zoomLevel);
            int y = -1;

            for (int j = 0; j < _tileImages.GetLength(1); j++)
            {
                int x = -1;
                
                for (int i = 0; i < _tileImages.GetLength(0); i++)
                {
                    Texture2D map = await MapManager.GetMapFromCoordinate(_initialTile.x + x, _initialTile.y + y, zoomLevel);
                    _tileImages[i, j].texture = map;
                    x++;
                }

                y++;
            }
        }

        private void GenerateTileImages()
        {
            int initialX = -1;
            Vector2 size = mapCenter.rect.size * 2f;
            
            for (int j = 0; j < _tileImages.GetLength(1); j++)
            {
                int initialY = -1;
                
                for (int i = 0; i < _tileImages.GetLength(0); i++)
                {
                    RawImage rawImage = new GameObject($"MapTile: {i}, {j}").AddComponent<RawImage>();
                    rawImage.transform.SetParent(transform);

                    Vector2 position = (Vector2) mapCenter.position + size * new Vector2(initialX, initialY);
                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

                    rawImage.rectTransform.position = position;

                    _tileImages[i, j] = rawImage;
                    initialY++;
                }

                initialX++;
            }
        }
    }
}