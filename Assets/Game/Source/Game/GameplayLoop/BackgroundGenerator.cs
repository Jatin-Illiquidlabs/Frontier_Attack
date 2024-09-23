using UnityEngine;
using UnityEngine.Tilemaps;

namespace WerewolfBearer {
    public class BackgroundGenerator : MonoBehaviour {
        [SerializeField]
        private Grid _grid;

        [SerializeField]
        private Tilemap _tilemap;

        [SerializeField]
        private Tile[] _tiles;

        [SerializeField]
        private float _perlinScale = 0.1f;

        [SerializeField]
        private float _perlinDarkening = 0.6f;

        [SerializeField]
        private float _tileSize = 1;

        [SerializeField]
        private Camera _camera;

        private RectInt _gridRect;

        public Vector2 PerlinSeed { get; set; }

        public void Clear() {
            _tilemap.ClearAllTiles();

            _gridRect = new(new Vector2Int(int.MaxValue, int.MaxValue), new Vector2Int(int.MaxValue, int.MaxValue));
        }

        public void Initialize() {
            if (_camera == null) {
                _camera = Camera.main;
            }

            _grid.transform.localScale = Vector3.one * _tileSize;
        }

        private void OnEnable() {
            Clear();
            Initialize();
        }

        private void OnDisable() {
            Clear();
        }

        private void Update() {
            if (UpdateGridStart()) {
                UpdateTiles();
            }
        }

        private bool UpdateGridStart() {
            Vector2 cameraViewportSize = CameraUtility.GetCameraViewportSize(_camera);
            Vector2 viewLeftBottom = (Vector2) _camera.transform.position - (cameraViewportSize * 0.5f);
            Vector2Int gridStart = Vector2Int.FloorToInt(viewLeftBottom / _tileSize);
            Vector2Int gridSize = GetTileCountXY();
            RectInt gridRect = new(gridStart, gridSize);

            if (!_gridRect.Equals(gridRect)) {
                _gridRect = gridRect;
                return true;
            }

            return false;
        }

        private void UpdateTiles() {
            _tilemap.transform.position = (Vector2) _gridRect.position * _tileSize;
            Vector2Int tileCountXY = GetTileCountXY();

            TileChangeData tileChangeData = new() {
                transform = Matrix4x4.identity
            };
            for (int gridX = 0; gridX < tileCountXY.x; gridX++) {
                for (int gridY = 0; gridY < tileCountXY.y; gridY++) {
                    Vector2Int gridPosition = new(gridX, gridY);
                    Vector2Int gridWorldPosition = gridPosition + _gridRect.position;

                    TileBase tile = GetTileForGridPosition(gridWorldPosition, out float alpha);

                    alpha = 1f - alpha;
                    alpha *= alpha;
                    float baseValue = 1f - alpha * _perlinDarkening;
                    Color color = Color.white * baseValue;
                    color.a = 1;

                    tileChangeData.position = (Vector3Int) gridPosition;
                    tileChangeData.tile = tile;
                    tileChangeData.color = color;
                    _tilemap.SetTile(tileChangeData, false);
                }
            }
        }

        private Vector2Int GetTileCountXY() {
            Vector2 cameraViewportSize = CameraUtility.GetCameraViewportSize(_camera);
            Vector2Int tileCountXY = Vector2Int.CeilToInt(cameraViewportSize / _tileSize);
            tileCountXY += Vector2Int.one;
            return tileCountXY;
        }

        private TileBase GetTileForGridPosition(Vector2Int gridPosition, out float noiseValue) {
            Vector2 scaledNoisePosition = (gridPosition + PerlinSeed) * _perlinScale;
            noiseValue = Mathf.PerlinNoise(scaledNoisePosition.x, scaledNoisePosition.y);
            noiseValue = Mathf.Clamp01(noiseValue);

            int tilesMaxIndex = _tiles.Length;
            Tile tile = _tiles[Mathf.Max(Mathf.CeilToInt(noiseValue * tilesMaxIndex) - 1, 0)];

            return tile;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere((Vector2) _gridRect.position * _tileSize, 0.3f);
        }
    }
}
