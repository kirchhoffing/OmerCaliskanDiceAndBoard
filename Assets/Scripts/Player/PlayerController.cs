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
                int savedTileIndex = GameManager.instance.playerTileIndex;

                if (savedTileIndex >= 0 && savedTileIndex < MapManager.instance.tiles.Count)
                {
                    transform.position = MapManager.instance.tiles[savedTileIndex].position;
                    currentTileIndex = savedTileIndex;

                    // Harita tamamen yüklendiğinde mapLoaded true yapılıyor
                    mapLoaded = true;  
                }
            }
        }
        
        public void MovePlayer(int steps)
        {
            if (mapLoaded)
            {
                StartCoroutine(MoveStepByStep(steps));
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
            }

            // Piyon her hamleyi tamamladıktan sonra playerTileIndex'i güncelle
            GameManager.instance.playerTileIndex = currentTileIndex;

            // Güncellenen Tile indeksini JSON'a kaydet
            GameManager.instance.SaveGameData();
        }
        
        public void AddToInventory(string itemName, int amount)
        {
            if (InventorySystem.instance != null)
            {
                InventorySystem.instance.AddItem(itemName, amount);
            }
        }
    }
}
