using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName;  // Collectible'ın ismi (elma, armut, çilek)
    public int amount = 1;   // Toplandığında kaç adet eklenecek

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu bu collectible'a çarptığında
        if (other.CompareTag("Player"))
        {
            // Envantere ekle
            InventorySystem.instance.AddItem(itemName, amount);
            Debug.Log($"{amount} adet {itemName} envantere eklendi.");

            // Collectible'ı yok et
            Destroy(gameObject);
        }
    }
}