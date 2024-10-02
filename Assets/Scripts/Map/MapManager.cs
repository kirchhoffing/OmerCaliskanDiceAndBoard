using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;  // Singleton erişimi için

        public List<Transform> tiles = new List<Transform>();  // Tüm Tile'ların listesi

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;  // Singleton instance oluştur
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}