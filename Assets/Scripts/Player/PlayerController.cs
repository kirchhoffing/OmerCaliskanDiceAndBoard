using System.Collections;
using Dice;
using Map;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public int currentTileIndex = 0;
        public float moveSpeed = 2f;
        public float stepDelay = 0.2f;

        private bool mapLoaded = false;

        private void OnEnable()
        {
            MapLoader.OnMapLoaded += MoveToStartPosition;  // Olay abonesi ol
            DiceRoller.instance.OnDiceRolled += MovePlayer;
        }

        private void OnDisable()
        {
            MapLoader.OnMapLoaded -= MoveToStartPosition;  // Olaydan çık
            DiceRoller.instance.OnDiceRolled -= MovePlayer;
        }

        // Harita yüklendikten sonra bu fonksiyon çağrılır
        private void MoveToStartPosition()
        {
            // Haritanın yüklenip yüklenmediğini kontrol et
            if (MapManager.instance != null && MapManager.instance.tiles.Count > 0)
            {
                transform.position = MapManager.instance.tiles[0].position;
                currentTileIndex = 0;
                mapLoaded = true;
                Debug.Log("Oyuncu başlangıç pozisyonuna yerleşti: İlk kare.");
            }
            else
            {
                Debug.LogError("Harita yüklenmedi veya tile'lar eksik.");
            }
        }

        public void MovePlayer(int steps)
        {
            if (mapLoaded)
            {
                Debug.Log($"Zar sonucu: {steps}, oyuncu hareket edecek.");
                StartCoroutine(MoveStepByStep(steps));
            }
            else
            {
                Debug.LogWarning("Harita yüklenmeden oyuncu hareket edemez.");
            }
        }


        private IEnumerator MoveStepByStep(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                // Bir sonraki tile'a git
                currentTileIndex = (currentTileIndex + 1) % MapManager.instance.tiles.Count;
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = MapManager.instance.tiles[currentTileIndex].position;

                // Karakterin hedef yöne bakmasını sağla
                transform.LookAt(targetPosition);

                // Piyonu adım adım Lerp ile hareket ettir
                float time = 0;
                while (time < 1)
                {
                    time += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, time);
                    yield return null;
                }

                // Her adım sonrası bekleme
                yield return new WaitForSeconds(stepDelay);
                Debug.Log($"Piyon {i + 1}. adımda hareket etti.");
            }

            Debug.Log("Piyon son noktaya ulaştı.");
        }


        public void AddToInventory(string itemName, int amount)
        {
            if (InventorySystem.instance != null)
            {
                InventorySystem.instance.AddItem(itemName, amount);
                Debug.Log($"Envantere {itemName} eklendi.");
            }
            else
            {
                Debug.LogError("InventorySystem instance bulunamadı.");
            }
        }
    }
}
