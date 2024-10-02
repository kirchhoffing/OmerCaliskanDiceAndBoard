using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Map
{
    public class MapLoader : MonoBehaviour
    {
        public static Action OnMapLoaded;  // Harita yüklendiğinde tetiklenecek Action

        public TextAsset jsonFile;  // JSON dosyasını referans alıyoruz
        public GameObject tilePrefab;  // Tile prefabı (altında TextMeshPro olacak)
        
        private void Start()
        {
            LoadMapFromJson();
        }

        private void LoadMapFromJson()
        {
            // JSON dosyasından verileri okuyoruz
            GameMap mapData = JsonUtility.FromJson<GameMap>(jsonFile.text);

            // Her bir tile için prefab oluşturuyoruz
            foreach (Tile tile in mapData.tiles)
            {
                // Tile pozisyonu
                Vector3 tilePosition = new Vector3(tile.position.x, tile.position.y, tile.position.z);

                // Tile prefabını oluştur
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                // Tile prefabının altındaki TextMeshPro'yu bul
                TextMeshPro textMesh = tileObject.GetComponentInChildren<TextMeshPro>();

                // Eğer content 'empty' değilse TextMeshPro'yu güncelle
                if (tile.content != "empty" && textMesh != null)
                {
                    textMesh.text = tile.content;  // JSON'daki içerik ile TextMeshPro'yu güncelle
                }

                // Tile'ı MapManager'daki tiles listesine ekle
                MapManager.instance.tiles.Add(tileObject.transform);
            }
            
            OnMapLoaded?.Invoke();
        }
        
    }

    [System.Serializable]
    public class GameMap
    {
        public List<Tile> tiles;
    }

    [System.Serializable]
    public class Tile
    {
        public int id;
        public string content;
        public TilePosition position;
    }

    [System.Serializable]
    public class TilePosition
    {
        public float x;
        public float y;
        public float z;
    }
}
