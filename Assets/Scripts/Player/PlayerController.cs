using System;
using System.Collections;
using Dice;
using Map;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public int currentTileIndex = 0;
        public float moveSpeed = 8f;
        public float stepDelay = 0.2f;
        public float scaleSpeed = 20f;
        public float scaleMultiplier = 0.8f;
        public float jumpScaleMultiplier = 0.2f;

        private Vector3 _originalScale;
        private Vector3 _shrunkScale;
        
        public static Action OnTileReached; 

        private void Start()
        {
            _originalScale = transform.localScale;
            _shrunkScale = new Vector3(_originalScale.x, _originalScale.y * scaleMultiplier, _originalScale.z);
        }

        private void OnEnable()
        {
            MapLoader.OnMapLoaded += MoveToSavedPosition;
            DiceRoller.OnDiceRolled += MovePlayer;
        }

        private void OnDisable()
        {
            MapLoader.OnMapLoaded -= MoveToSavedPosition;
            DiceRoller.OnDiceRolled -= MovePlayer;
        }

        private void MoveToSavedPosition()
        {
            if (MapManager.instance != null && MapManager.instance.tiles.Count > 0)
            {
                int savedTileIndex = GameManager.instance.playerTileIndex;

                if (savedTileIndex >= 0 && savedTileIndex < MapManager.instance.tiles.Count)
                {
                    transform.position = MapManager.instance.tiles[savedTileIndex].position;
                    transform.rotation = GameManager.instance.playerRotation;
                    currentTileIndex = savedTileIndex;
                }
            }
        }

        public void MovePlayer(int steps)
        {
            StartCoroutine(MoveStepByStep(steps));
        }

        private IEnumerator MoveStepByStep(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                float shrinkTime = 0;
                while (shrinkTime < 1)
                {
                    shrinkTime += Time.deltaTime * scaleSpeed;
                    transform.localScale = Vector3.Lerp(_originalScale, _shrunkScale, shrinkTime);
                    yield return null;
                }

                yield return new WaitForSeconds(0.05f);
                
                float growTime = 0;
                while (growTime < 1)
                {
                    growTime += Time.deltaTime * scaleSpeed;
                    transform.localScale = Vector3.Lerp(_shrunkScale, _originalScale, growTime);
                    yield return null;
                }
                
                currentTileIndex = (currentTileIndex + 1) % MapManager.instance.tiles.Count;
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = MapManager.instance.tiles[currentTileIndex].position;

                transform.LookAt(targetPosition);

                float time = 0;
                while (time < 1)
                {
                    time += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, time);
                    
                    if (i < steps)
                    {
                        float yScaleMultiplier = 1 + Mathf.Sin(time * Mathf.PI) * jumpScaleMultiplier;
                        transform.localScale = new Vector3(_originalScale.x, _originalScale.y * yScaleMultiplier,
                            _originalScale.z);
                    }

                    yield return null;
                }

                transform.localScale = _originalScale;
                
                if (i < steps)
                {
                    Transform currentTile = MapManager.instance.tiles[currentTileIndex];
                    StartCoroutine(SinkAndRiseTile(currentTile));
                }

                yield return new WaitForSeconds(stepDelay);
            }

            GameManager.instance.playerTileIndex = currentTileIndex;

            GameManager.instance.SaveGameData();

            CheckAndCollectItem(currentTileIndex);
            
            OnTileReached?.Invoke();
        }

        private IEnumerator SinkAndRiseTile(Transform tile)
        {
            Vector3 originalPosition = tile.position;
            Vector3 sunkenPosition = originalPosition + new Vector3(0, -0.1f, 0);

            float sinkTime = 0;
            float sinkDuration = 0.2f;

            while (sinkTime < sinkDuration)
            {
                sinkTime += Time.deltaTime;
                tile.position = Vector3.Lerp(originalPosition, sunkenPosition, sinkTime / sinkDuration);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            sinkTime = 0;

            while (sinkTime < sinkDuration)
            {
                sinkTime += Time.deltaTime;
                tile.position = Vector3.Lerp(sunkenPosition, originalPosition, sinkTime / sinkDuration);
                yield return null;
            }

            tile.position = originalPosition;
        }

        private void CheckAndCollectItem(int tileIndex)
        {
            Transform tile = MapManager.instance.tiles[tileIndex];
            TextMeshPro textMesh = tile.GetComponentInChildren<TextMeshPro>();

            if (textMesh != null && textMesh.text != "empty")
            {
                string[] contentParts = textMesh.text.Split(' ');

                if (contentParts.Length >= 2)
                {
                    if (int.TryParse(contentParts[0], out int amount))
                    {
                        string itemName = contentParts[1];
                        AddToInventory(itemName, amount);
                    }
                }
            }
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
