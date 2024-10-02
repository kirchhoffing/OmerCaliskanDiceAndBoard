using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        public Text appleCountText;
        public Text pearCountText;
        public Text strawberryCountText;

        private void Start()
        {
            // Envanter güncelleme event'ine abone ol
            InventorySystem.instance.OnInventoryUpdated += UpdateInventoryUI;
        }

        private void OnDestroy()
        {
            // Event'ten çık
            InventorySystem.instance.OnInventoryUpdated -= UpdateInventoryUI;
        }

        // UI'ı envantere göre günceller
        private void UpdateInventoryUI()
        {
            int appleCount = InventorySystem.instance.GetItemCount("elma");
            int pearCount = InventorySystem.instance.GetItemCount("armut");
            int strawberryCount = InventorySystem.instance.GetItemCount("çilek");

            appleCountText.text = $"Elma: {appleCount}";
            pearCountText.text = $"Armut: {pearCount}";
            strawberryCountText.text = $"Çilek: {strawberryCount}";
        }
    }
}