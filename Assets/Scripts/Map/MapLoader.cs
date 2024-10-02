using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapLoader : MonoBehaviour
    {
        public static event Action OnMapLoaded;
        private ITileFactory _tileFactory;
    
        public TextAsset jsonFile;
        public GameObject tilePrefab;

        void Start()
        {
            _tileFactory = new TileFactory(tilePrefab);
            LoadMapFromJson();
        }

        private void LoadMapFromJson()
        {
            GameMap map = JsonUtility.FromJson<GameMap>(jsonFile.text);
            List<Transform> tileTransforms = new List<Transform>();

            // Tüm tile'ları oluştur ve pozisyonlarını listeye ekle
            foreach (Tile tile in map.tiles)
            {
                Transform tileTransform = _tileFactory.CreateTile(tile);
                tileTransforms.Add(tileTransform);

                // Tile'ı MapManager'a ekle
                MapManager.instance.tiles.Add(tileTransform);  // Haritayı MapManager'a ekliyoruz
            }

            // Haritanın ortalamasını hesapla
            Vector3 mapCenter = CalculateMapCenter(tileTransforms);

            // Haritayı merkezle (ortayı 0,0,0 konumuna taşı)
            foreach (Transform tileTransform in tileTransforms)
            {
                tileTransform.position -= mapCenter;  // Her tile'ı haritanın ortasına göre kaydır
            }

            // Harita yüklendikten sonra olayı tetikleyelim
            OnMapLoaded?.Invoke();
        }

        // Haritanın ortalama merkezini hesaplar
        private Vector3 CalculateMapCenter(List<Transform> tileTransforms)
        {
            Vector3 sum = Vector3.zero;

            foreach (Transform tileTransform in tileTransforms)
            {
                sum += tileTransform.position;
            }

            return sum / tileTransforms.Count;
        }
    }


    public interface ITileFactory
    {
        Transform CreateTile(Tile tileData);
    }

    public class TileFactory : ITileFactory
    {
        private GameObject _tilePrefab;

        public TileFactory(GameObject tilePrefab)
        {
            _tilePrefab = tilePrefab;
        }

        public Transform CreateTile(Tile tileData)
        {
            GameObject tileObject = GameObject.Instantiate(_tilePrefab);
            tileObject.transform.position = tileData.position;
            tileObject.name = $"Tile_{tileData.id}";
            return tileObject.transform;
        }
    }

    [System.Serializable]
    public class Tile
    {
        public int id;
        public Vector3 position;
    }

    [System.Serializable]
    public class GameMap
    {
        public List<Tile> tiles;
    }
}