using System.Collections;
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
        
        [Header("Initial Map Setup")]
        [SerializeField] private double latitude;
        [SerializeField] private double longitude;
        [SerializeField] private int zoomLevel = 10;
        [SerializeField] private int gridSize = 3;
        
        [Header("Navigation")]
        [SerializeField] private float navigationVelocity;
        [SerializeField] private bool enableMap = false;
        [SerializeField] private Vector2 scaleRange = new(0.8f, 1.2f);

        private readonly Dictionary<TilePoint, Texture2D> _cacheTextures = new();
        private CancellationTokenSource _cancellationTokenSource;
        private MapCell[,] _tileImages;
        private Vector2Int _closestTile;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
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

        public void MoveMap(Vector2 inputDelta)
        {
            if (IsGoingOutsideBounds(inputDelta)) 
                return;

            foreach (MapCell tileImage in _tileImages)
                tileImage.Image.rectTransform.localPosition += (Vector3) inputDelta;

            Vector2Int newClosestTile = GetClosestTile();
            
            if (newClosestTile != _closestTile)
            {
                Vector2Int distance = newClosestTile - _closestTile;
                UpdateTiles(distance.x, distance.y);
            }
        }

        private bool IsGoingOutsideBounds(Vector2 inputDelta)
        {
            return inputDelta.x < 0f && _tileImages[0, 0].TilePoint.x == 0 ||
                   inputDelta.y < 0f && _tileImages[0, 0].TilePoint.y == 0 || 
                   inputDelta.x > 0f && _tileImages[gridSize - 1, 0].TilePoint.x == 1 << _tileImages[gridSize - 1, 0].TilePoint.zoom || 
                   inputDelta.y > 0f && _tileImages[0, gridSize - 1].TilePoint.y == 1 << _tileImages[0, gridSize - 1].TilePoint.zoom;
        }

        public void AddZoom(float zoomOffset)
        {
            mapCenter.localScale += (Vector3) (Vector2.one * zoomOffset);
            
            if(mapCenter.localScale.x > scaleRange.y)
                ChangeZoomLevel(1);
            else if(mapCenter.localScale.x < scaleRange.x)
                ChangeZoomLevel(-1);
        }

        private void ChangeZoomLevel(int zoomOffset)
        {
            mapCenter.localScale = (zoomOffset < 0 ? scaleRange.y : scaleRange.x) * Vector2.one;
            
            int centerIndex = gridSize / 2;
            MapCell centerCell = _tileImages[centerIndex, centerIndex];
            int initialZoom = centerCell.TilePoint.zoom;
            
            int newZoom = initialZoom + zoomOffset;
            newZoom = Mathf.Clamp(newZoom, 1, 18);

            if (newZoom - initialZoom != 0)
            {
                double initialLongitude = MapManager.ConvertTileXToLongitude(_tileImages[centerIndex, centerIndex].TilePoint.x, initialZoom);
                double initialLatitude = MapManager.ConvertTileYToLatitude(_tileImages[centerIndex, centerIndex].TilePoint.y, initialZoom);

                TilePoint newInitialTilePoint = MapManager.ConvertCoordinateToTile(initialLatitude, initialLongitude, newZoom);

                for (int x = 0; x < gridSize; x++)
                {
                    for (int y = 0; y < gridSize; y++)
                        _tileImages[x, y].TilePoint = newInitialTilePoint + (new Vector2Int(x, y) - Vector2Int.one * centerIndex);
                }

                UniTask populateTask = PopulateMapTexturesAsync();
                populateTask.AttachExternalCancellation(_cancellationTokenSource.Token);
            }
        }

        private void UpdateTiles(int deltaX, int deltaY)
        {
            float size = _tileImages[0, 0].Image.rectTransform.sizeDelta.x;
            
            if (deltaX > 0)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    _tileImages[0, y].Image.transform.localPosition = _tileImages[gridSize - 1, y].Image.transform.localPosition + new Vector3(deltaX, 0) * size;
                    MapCell temporaryInitial = _tileImages[0, y];

                    for (int x = 0; x < gridSize - 1; x++)
                        _tileImages[x, y] = _tileImages[x + 1, y];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, 0) * gridSize);
                    _tileImages[gridSize - 1, y] = temporaryInitial;
                }
            }
            else if(deltaX < 0)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    _tileImages[gridSize - 1, y].Image.transform.localPosition = _tileImages[0, y].Image.transform.localPosition + new Vector3(deltaX, 0) * size;
                    MapCell temporaryInitial = _tileImages[gridSize - 1, y];

                    for (int x = gridSize - 1; x > 0; x--)
                        _tileImages[x, y] = _tileImages[x - 1, y];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(deltaX, 0) * gridSize);
                    _tileImages[0, y] = temporaryInitial;
                }
            }
            
            if (deltaY > 0)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    _tileImages[x, 0].Image.transform.localPosition = _tileImages[x, gridSize - 1].Image.transform.localPosition + new Vector3(0, -deltaY) * size;
                    MapCell temporaryInitial = _tileImages[x, 0];

                    for (int y = 0; y < gridSize - 1; y++)
                        _tileImages[x, y] = _tileImages[x, y + 1];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(0, deltaY) * gridSize);
                    _tileImages[x, gridSize - 1] = temporaryInitial;
                }
            }
            else if(deltaY < 0)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    _tileImages[x, gridSize - 1].Image.transform.localPosition = _tileImages[x, 0].Image.transform.localPosition + new Vector3(0, -deltaY) * size;
                    MapCell temporaryInitial = _tileImages[x, gridSize - 1];

                    for (int y = gridSize - 1; y > 0; y--)
                        _tileImages[x, y] = _tileImages[x, y - 1];

                    temporaryInitial.SetTilePoint(temporaryInitial.TilePoint + new Vector2Int(0, deltaY) * gridSize);
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
            if (_cacheTextures.TryGetValue(tilePoint, out Texture2D mapTile))
                return mapTile;

            Texture2D map = await MapManager.GetMapFromCoordinate(tilePoint, tilePoint.zoom);
            
            _cacheTextures[tilePoint] = map;
            return map;
        }

        private void GenerateTileImages()
        {
            TilePoint initialTile = MapManager.ConvertCoordinateToTile(latitude, longitude, zoomLevel);
            _tileImages = new MapCell[gridSize, gridSize];
            
            int gridCenter = gridSize / 2;
            float size = Screen.width;

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
                    rawImage.transform.localScale = Vector2.one;
                    TilePoint tilePoint = initialTile + new TilePoint(offset.x, offset.y, initialTile.zoom);

                    _tileImages[x, y].Image = rawImage;
                    _tileImages[x, y].SetTilePoint(tilePoint);
                }
            }
        }
    }
}