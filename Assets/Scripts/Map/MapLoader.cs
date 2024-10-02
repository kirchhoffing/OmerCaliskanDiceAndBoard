using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapLoader : MonoBehaviour
    {
        public static Action OnMapLoaded;  // Harita yüklendiğinde tetiklenecek Action
        public GameObject tilePrefab;      // Tile prefab'ı
        public TextAsset jsonFile;         // JSON dosyasını buraya ekleyeceğiz

        void Start()
        {
            LoadMapFromJson();  // JSON'dan map yükle
        }

        private void LoadMapFromJson()
        {
            // JSON verisini çöz
            GameMap gameMap = JsonUtility.FromJson<GameMap>(jsonFile.text);

            // Her tile üzerinde işlem yap
            foreach (TileData tile in gameMap.tiles)
            {
                // Tile'ın pozisyonunu JSON'dan al ve sahneye ekle
                Vector3 tilePosition = tile.position;

                // Tile prefab'ını belirtilen pozisyonda oluştur
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tileObject.name = $"Tile_{tile.id}";

                // Tile'ı MapManager tiles listesine ekle
                MapManager.instance.tiles.Add(tileObject.transform);
            }

            // Harita yüklendikten sonra Action'ı tetikleyelim
            OnMapLoaded?.Invoke();
        }
    }

    [System.Serializable]
    public class GameMap
    {
        public List<TileData> tiles;  // Tile'ları tutan liste
    }

    [System.Serializable]
    public class TileData
    {
        public int id;  // Tile'ın ID'si
        public string content;  // İçerik (örneğin "5 apples")
        public Vector3 position;  // Tile'ın pozisyonu
    }
}