using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unicar.MapAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Unicar.UI.RidesScreen.View
{
    public class MapDrawer : MonoBehaviour
    {
        [SerializeField] private RectTransform mapCenter;
        [SerializeField] private double latitude;
        [SerializeField] private double longitude;
        [SerializeField] private int zoomLevel = 10;

        private Dictionary<TilePoint, Texture2D> _cacheTextures = new();
        private readonly RawImage[,] _tileImages = new RawImage[3, 3];
        private CancellationTokenSource _cancellationTokenSource;
        private TilePoint _initialTile;

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
            UniTask[] populateTasks = new UniTask[_tileImages.GetLength(0) * _tileImages.GetLength(1)];
            _initialTile = MapManager.ConvertCoordinateToTile(latitude, longitude, zoomLevel);

            int taskIndex = 0;
            int y = 1;

            for (int j = 0; j < _tileImages.GetLength(1); j++)
            {
                int x = -1;
                
                for (int i = 0; i < _tileImages.GetLength(0); i++)
                {
                    TilePoint tile = _initialTile + new TilePoint(x, y, _initialTile.zoom);
                    populateTasks[taskIndex] = PopulateTile(tile, new Vector2Int(i, j));
                    
                    x++;
                    taskIndex++;
                }

                y--;
            }

            await UniTask.WhenAll(populateTasks);
        }

        private async UniTask PopulateTile(TilePoint tilePoint, Vector2Int index)
        {
            Texture2D map = await SpawnAndCacheMapTileAsync(tilePoint);
            _tileImages[index.x, index.y].texture = map;
        }

        private async UniTask<Texture2D> SpawnAndCacheMapTileAsync(TilePoint tilePoint)
        {
            if (_cacheTextures.TryGetValue(tilePoint, out Texture2D mapTile))
                return mapTile;
            
            Texture2D map = await MapManager.GetMapFromCoordinate(tilePoint, zoomLevel);
            _cacheTextures[tilePoint] = map;
            return map;
        }

        private void GenerateTileImages()
        {
            int initialY = -1;
            float size = mapCenter.rect.width * 2f;
            
            for (int j = 0; j < _tileImages.GetLength(1); j++)
            {
                int initialX = -1;
                
                for (int i = 0; i < _tileImages.GetLength(0); i++)
                {
                    RawImage rawImage = new GameObject($"MapTile: {i}, {j}").AddComponent<RawImage>();
                    rawImage.transform.SetParent(transform);

                    Vector2 position = (Vector2) mapCenter.position + size * new Vector2(initialX, initialY);
                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

                    rawImage.rectTransform.position = position;

                    _tileImages[i, j] = rawImage;
                    initialX++;
                }

                initialY++;
            }
        }
    }
}