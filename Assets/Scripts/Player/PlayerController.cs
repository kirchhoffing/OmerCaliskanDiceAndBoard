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
            MapLoader.OnMapLoaded += MoveToSavedPosition;
            DiceRoller.instance.OnDiceRolled += MovePlayer;
        }

        private void OnDisable()
        {
            MapLoader.OnMapLoaded -= MoveToSavedPosition;
            DiceRoller.instance.OnDiceRolled -= MovePlayer;
        }

        private void MoveToSavedPosition()
        {
            if (MapManager.instance != null && MapManager.instance.tiles.Count > 0)
            {
                transform.position = MapManager.instance.tiles[0].position;
                currentTileIndex = 0;
                mapLoaded = true;  // Harita yüklendiğinde mapLoaded'ı true yapıyoruz
                Debug.Log("Harita yüklendi ve karakter başlangıç pozisyonuna yerleşti.");
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
                currentTileIndex = (currentTileIndex + 1) % MapManager.instance.tiles.Count;
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = MapManager.instance.tiles[currentTileIndex].position;

                transform.LookAt(targetPosition);

                float time = 0;
                while (time < 1)
                {
                    time += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, time);
                    yield return null;
                }

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
