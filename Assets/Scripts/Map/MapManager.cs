using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;

        public List<Transform> tiles = new List<Transform>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}