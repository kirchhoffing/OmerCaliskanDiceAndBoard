using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    public delegate void InventoryUpdated();
    public event InventoryUpdated OnInventoryUpdated;

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

    // Envantere item ekler
    public void AddItem(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
        }
        else
        {
            inventory[itemName] = amount;
        }

        // Envanter güncellendiğinde event'i tetikle
        OnInventoryUpdated?.Invoke();
    }

    // İstenen item'in kaç tane olduğunu döner
    public int GetItemCount(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            return inventory[itemName];
        }
        return 0;
    }
}