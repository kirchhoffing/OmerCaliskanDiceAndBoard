using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int amount;
    }

    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem instance;

        [SerializeField]
        public List<InventoryItem> inventory = new List<InventoryItem>();

        private Dictionary<string, InventoryItem> _inventoryDictionary = new Dictionary<string, InventoryItem>();

        private string _savePath;

        public static Action OnInventoryDataLoaded;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _savePath = Path.Combine(Application.persistentDataPath, "inventoryData.json");
        }

        private void Start()
        {
            LoadInventoryData();
        }

        public void AddItem(string itemName, int amount)
        {
            InventoryItem itemInList = inventory.Find(i => i.itemName == itemName);

            if (itemInList != null)
            {
                itemInList.amount += amount;
            }
            else
            {
                inventory.Add(new InventoryItem { itemName = itemName, amount = amount });
            }

            SaveInventoryData();
            OnInventoryDataLoaded?.Invoke();
        }

        public void SaveInventoryData()
        {
            string json = JsonUtility.ToJson(new InventoryData(inventory), true);
            File.WriteAllText(_savePath, json);
        }

        public void LoadInventoryData()
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                InventoryData data = JsonUtility.FromJson<InventoryData>(json);
                inventory = data.items;

                OnInventoryDataLoaded?.Invoke();
            }
        }
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<InventoryItem> items;

        public InventoryData(List<InventoryItem> items)
        {
            this.items = items;
        }
    }
}
