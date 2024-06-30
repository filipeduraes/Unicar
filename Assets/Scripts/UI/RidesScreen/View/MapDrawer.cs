using System.Collections.Generic;
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
        [SerializeField] private double latitude;
        [SerializeField] private double longitude;
        [SerializeField] private int zoomLevel = 10;
        [SerializeField] private int gridSize = 3;
        [SerializeField] private float navigationVelocity;
        [SerializeField] private bool enableMap = false;

        private readonly Dictionary<int, Dictionary<TilePoint, Texture2D>> _cacheTextures = new();
        private MapCell[,] _tileImages;
        private CancellationTokenSource _cancellationTokenSource;
        private TilePoint _initialTile;
        private Vector2Int _closestTile;

        private void Awake()
        {
            GenerateTileImages();
            _closestTile = GetClosestTile();

            _cancellationTokenSource = new CancellationTokenSource();

            if (enableMap)
            {
                UniTask populateTask = PopulateMapTexturesAsync();
                populateTask.AttachExternalCancellation(_cancellationTokenSource.Token);
            }
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            bool zoomIn = Input.GetMouseButtonDown(0);
            bool zoomOut = Input.GetMouseButtonDown(1);
            
            Vector3 velocity = new Vector3(horizontal, vertical) * navigationVelocity;

            if (velocity.sqrMagnitude >= 0.01f)
            {
                foreach (MapCell tileImage in _tileImages)
                    tileImage.Image.rectTransform.localPosition += velocity;

                Vector2Int newClosestTile = GetClosestTile();
                
                if (newClosestTile != _closestTile)
                {
                    Vector2Int distance = newClosestTile - _closestTile;
                    UpdateTiles(distance.x, distance.y);
                }
            }

            if (zoomIn || zoomOut)
            {
                int centerIndex = gridSize / 2;
                int initialZoom = _tileImages[centerIndex, centerIndex].TilePoint.zoom;
                int newZoom = initialZoom + (zoomIn ? 1 : -1);
                newZoom = Mathf.Clamp(newZoom, 1, 18);

                if (newZoom - initialZoom != 0)
                {
                    double initialLongitude = MapManager.ConvertTileXToLongitude(_tileImages[centerIndex, centerIndex].TilePoint.x, initialZoom);
                    double initialLatitude = MapManager.ConvertTileYToLatitude(_tileImages[centerIndex, centerIndex].TilePoint.y, initialZoom);

                    TilePoint newInitialTilePoint = MapManager.ConvertCoordinateToTile(initialLatitude, initialLongitude, newZoom);

                    for (int x = 0; x < gridSize; x++)
                    {
                        for (int y = 0; y < gridSize; y++)
                        {
                            _tileImages[x, y].TilePoint = newInitialTilePoint + (new Vector2Int(x, y) - Vector2Int.one * centerIndex);
                        }
                    }

                    UniTask populateTask = PopulateMapTexturesAsync();
                    populateTask.AttachExternalCancellation(_cancellationTokenSource.Token);
                }
            }
        }

        void UpdateTiles(int deltaX, int deltaY)
        {
            float size = _tileImages[0, 0].Image.rectTransform.sizeDelta.x;
            
            if (deltaX > 0)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    _tileImages[0, y].Image.transform.localPosition = _tileImages[gridSize - 1, y].Image.transform.localPosition + new Vector3(deltaX, deltaY) * size;
                    MapCell temporaryInitial = _tileImages[0, y];

                    for (int x = 0; x < gridSize - 1; x++)
                        _tileImages[x, y] = _tileImages[x + 1, y];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, deltaY) * gridSize);
                    _tileImages[gridSize - 1, y] = temporaryInitial;
                }
            }
            else if(deltaX < 0)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    _tileImages[gridSize - 1, y].Image.transform.localPosition = _tileImages[0, y].Image.transform.localPosition + new Vector3(deltaX, deltaY) * size;
                    MapCell temporaryInitial = _tileImages[gridSize - 1, y];

                    for (int x = gridSize - 1; x > 0; x--)
                        _tileImages[x, y] = _tileImages[x - 1, y];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, deltaY) * gridSize);
                    _tileImages[0, y] = temporaryInitial;
                }
            }
            
            if (deltaY > 0)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    _tileImages[x, 0].Image.transform.localPosition = _tileImages[x, gridSize - 1].Image.transform.localPosition + new Vector3(deltaX, -deltaY) * size;
                    MapCell temporaryInitial = _tileImages[x, 0];

                    for (int y = 0; y < gridSize - 1; y++)
                        _tileImages[x, y] = _tileImages[x, y + 1];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, deltaY) * gridSize);
                    _tileImages[x, gridSize - 1] = temporaryInitial;
                }
            }
            else if(deltaY < 0)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    _tileImages[x, gridSize - 1].Image.transform.localPosition = _tileImages[x, 0].Image.transform.localPosition + new Vector3(deltaX, -deltaY) * size;
                    MapCell temporaryInitial = _tileImages[x, gridSize - 1];

                    for (int y = gridSize - 1; y > 0; y--)
                        _tileImages[x, y] = _tileImages[x, y - 1];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, deltaY) * gridSize);
                    _tileImages[x, 0] = temporaryInitial;
                }
            }

            UniTask populateMapTask = PopulateMapTexturesAsync();
            populateMapTask.AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private Vector2Int GetClosestTile()
        {
            Vector2Int smallerIndex = Vector2Int.zero;
            float smallerSqrMagnitude = float.MaxValue;

            for (int x = 0; x < _tileImages.GetLength(0); x++)
            {
                for (int y = 0; y < _tileImages.GetLength(1); y++)
                {
                    MapCell tileImage = _tileImages[x, y];
                    float sqrMagnitude = (tileImage.Image.rectTransform.position - mapCenter.position).sqrMagnitude;
                        
                    if (sqrMagnitude < smallerSqrMagnitude)
                    {
                        smallerIndex = new Vector2Int(x, y);
                        smallerSqrMagnitude = sqrMagnitude;
                    }
                }
            }

            return smallerIndex;
        }

        private async UniTask PopulateMapTexturesAsync()
        {
            if(!enableMap)
                return;
            
            UniTask[] populateTasks = new UniTask[_tileImages.GetLength(0) * _tileImages.GetLength(1)];
            int taskIndex = 0;

            for (int j = 0; j < _tileImages.GetLength(1); j++)
            {
                for (int i = 0; i < _tileImages.GetLength(0); i++)
                {
                    Vector2Int tileIndex = new(i, j);
                    populateTasks[taskIndex] = PopulateTile(tileIndex);

                    taskIndex++;
                }
            }

            await UniTask.WhenAll(populateTasks);
        }

        private async UniTask PopulateTile(Vector2Int index)
        {
            Texture2D map = await SpawnAndCacheMapTileAsync(_tileImages[index.x, index.y].TilePoint);
            _tileImages[index.x, index.y].Image.texture = map;
        }

        private async UniTask<Texture2D> SpawnAndCacheMapTileAsync(TilePoint tilePoint)
        {
            if (_cacheTextures.TryGetValue(tilePoint.zoom, out Dictionary<TilePoint, Texture2D> textures))
            {
                if(textures.TryGetValue(tilePoint, out Texture2D mapTile))
                    return mapTile;
            }
            
            Texture2D map = await MapManager.GetMapFromCoordinate(tilePoint, tilePoint.zoom);
            
            if (!_cacheTextures.ContainsKey(tilePoint.zoom))
                _cacheTextures[tilePoint.zoom] = new Dictionary<TilePoint, Texture2D>();
            
            _cacheTextures[tilePoint.zoom][tilePoint] = map;
            return map;
        }

        private void GenerateTileImages()
        {
            _initialTile = MapManager.ConvertCoordinateToTile(latitude, longitude, zoomLevel);
            _tileImages = new MapCell[gridSize, gridSize];
            
            int gridCenter = gridSize / 2;
            float size = mapCenter.rect.width * 2f;

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    RawImage rawImage = new GameObject($"MapTile: {x}, {y}").AddComponent<RawImage>();
                    rawImage.transform.SetParent(transform);

                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                    rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                    
                    Vector2Int offset = new Vector2Int(x, y) - Vector2Int.one * gridCenter;
                    rawImage.rectTransform.anchoredPosition = new Vector2(offset.x, -offset.y) * size;
                    
                    TilePoint tilePoint = _initialTile + new TilePoint(offset.x, offset.y, _initialTile.zoom);

                    _tileImages[x, y].Image = rawImage;
                    _tileImages[x, y].SetTilePoint(tilePoint);
                }
            }
        }
    }
}