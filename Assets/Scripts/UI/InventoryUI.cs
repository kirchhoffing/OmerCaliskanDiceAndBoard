using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        public TextMeshProUGUI applesText;
        public TextMeshProUGUI pearsText;
        public TextMeshProUGUI strawberriesText;

        public Image secretBackground;
        public Button rollButton;
        private bool _isSecretShown = false;
        
        private void OnEnable()
        {
            Dice.DiceRoller.OnRollButtonClicked += ShowSecretBackgroundAndDisableRollButton;
            PlayerController.OnTileReached += HideSecretBackgroundAndEnableRollButton;
            InventorySystem.OnInventoryDataLoaded += UpdateUI;
        }

        private void OnDisable()
        {
            Dice.DiceRoller.OnRollButtonClicked -= ShowSecretBackgroundAndDisableRollButton;
            PlayerController.OnTileReached -= HideSecretBackgroundAndEnableRollButton;
            InventorySystem.OnInventoryDataLoaded -= UpdateUI;
        }

        private void ShowSecretBackgroundAndDisableRollButton()
        {
            if (!_isSecretShown)
            {
                secretBackground.gameObject.SetActive(true);
                _isSecretShown = true;
            }

            rollButton.interactable = false;
        }

        private void HideSecretBackgroundAndEnableRollButton()
        {
            if (_isSecretShown)
            {
                secretBackground.gameObject.SetActive(false);
                _isSecretShown = false;
            }

            rollButton.interactable = true;
        }
        
        private void UpdateUI()
        {
            if (InventorySystem.instance != null)
            {
                List<InventoryItem> inventoryList = InventorySystem.instance.inventory;

                InventoryItem appleItem = inventoryList.Find(item => item.itemName == "Apples");
                if (appleItem != null)
                {
                    applesText.text = appleItem.amount.ToString();
                }
                else
                {
                    applesText.text = "0";
                }

                InventoryItem pearItem = inventoryList.Find(item => item.itemName == "Pears");
                if (pearItem != null)
                {
                    pearsText.text = pearItem.amount.ToString();
                }
                else
                {
                    pearsText.text = "0";
                }

                InventoryItem strawberryItem = inventoryList.Find(item => item.itemName == "Strawberries");
                if (strawberryItem != null)
                {
                    strawberriesText.text = strawberryItem.amount.ToString();
                }
                else
                {
                    strawberriesText.text = "0";
                }
            }
        }
    }
}
