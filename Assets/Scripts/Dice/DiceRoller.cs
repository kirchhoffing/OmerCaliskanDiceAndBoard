using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dice
{
    public class DiceRoller : MonoBehaviour
    {
        public static DiceRoller instance;
        public GameObject dicePrefab;
        public Transform diceSpawnPoint;
        public Button rollButton;

        private GameObject _currentDice;
        private Rigidbody _diceRb;
        
        public static Action OnRollButtonClicked;
        public static Action<int> OnDiceRolled;

        private const float StopThreshold = 0.05f;
        private const float StopCheckDuration = 0.5f;
        private float _stopTime = 0f;

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

        private void Start()
        {
            rollButton.onClick.AddListener(OnRollButtonPressed);
        }

        private void OnRollButtonPressed()
        {
            OnRollButtonClicked?.Invoke();
            StartCoroutine(RollDiceAndCheck());
        }

        private IEnumerator RollDiceAndCheck()
        {
            if (_currentDice != null)
            {
                Destroy(_currentDice);
            }

            _currentDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.Euler(
                UnityEngine.Random.Range(0f, 360f), 
                UnityEngine.Random.Range(0f, 360f), 
                UnityEngine.Random.Range(0f, 360f)));
            _diceRb = _currentDice.GetComponent<Rigidbody>();

            _diceRb.velocity = Vector3.zero;
            _diceRb.angularVelocity = Vector3.zero;

            Vector3 initialForce = new Vector3(0f, -2f, 0f);
            _diceRb.AddForce(initialForce, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3(
                UnityEngine.Random.Range(-100f, 100f), 
                UnityEngine.Random.Range(-100f, 100f), 
                UnityEngine.Random.Range(-100f, 100f));
            
            _diceRb.AddTorque(randomTorque, ForceMode.Impulse);

            _stopTime = 0f;

            yield return new WaitUntil(() => IsDiceStopped());

            int diceValue = GetDiceValue();

            yield return new WaitForSeconds(0.5f);

            OnDiceRolled?.Invoke(diceValue);
        }

        private bool IsDiceStopped()
        {
            if (_diceRb.velocity.magnitude < StopThreshold && _diceRb.angularVelocity.magnitude < StopThreshold)
            {
                _stopTime += Time.deltaTime;
            }
            else
            {
                _stopTime = 0f;
            }

            return _stopTime > StopCheckDuration;
        }

        private int GetDiceValue()
        {
            Vector3 upVector = _currentDice.transform.up;

            if (Mathf.Abs(upVector.y - 1f) < 0.1f) return 3;
            if (Mathf.Abs(upVector.y + 1f) < 0.1f) return 4;
            if (Mathf.Abs(_currentDice.transform.forward.y - 1f) < 0.1f) return 2;
            if (Mathf.Abs(_currentDice.transform.forward.y + 1f) < 0.1f) return 5;
            if (Mathf.Abs(_currentDice.transform.right.y - 1f) < 0.1f) return 6;
            if (Mathf.Abs(_currentDice.transform.right.y + 1f) < 0.1f) return 1;

            return 0;
        }
    }
}
