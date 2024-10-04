using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Map
{
    public class MapLoader : MonoBehaviour
    {
        public static Action OnMapLoaded;
        public TextAsset jsonFile;
        public GameObject tilePrefab;

        private void Start()
        {
            LoadMapFromJson();
        }

        private void LoadMapFromJson()
        {
            GameMap mapData = JsonUtility.FromJson<GameMap>(jsonFile.text);

            foreach (Tile tile in mapData.tiles)
            {
                Vector3 tilePosition = new Vector3(tile.position.x, tile.position.y, tile.position.z);
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                TextMeshPro textMesh = tileObject.GetComponentInChildren<TextMeshPro>();

                if (tile.content != "empty" && textMesh != null)
                {
                    textMesh.text = tile.content;
                }

                MapManager.instance.tiles.Add(tileObject.transform);
            }
            
            OnMapLoaded?.Invoke();
        }
    }

    [Serializable]
    public class GameMap
    {
        public List<Tile> tiles;
    }

    [Serializable]
    public class Tile
    {
        public int id;
        public string content;
        public TilePosition position;
    }

    [Serializable]
    public class TilePosition
    {
        public float x;
        public float y;
        public float z;
    }
}