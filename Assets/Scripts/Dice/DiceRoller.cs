using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        public Action<int> OnDiceRolled;

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
            // Butona tıklanma olayına zar atma işlemini ekliyoruz
            rollButton.onClick.AddListener(() => StartCoroutine(RollDiceAndCheck()));
        }

        private IEnumerator RollDiceAndCheck()
        {
            if (_currentDice != null)
            {
                Destroy(_currentDice);  // Önceki zar varsa yok et
            }

            // Yeni bir zar oluştur ve rastgele bir rotasyon ver
            _currentDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            _diceRb = _currentDice.GetComponent<Rigidbody>();

            // Zarın hız ve dönme hızlarını sıfırla
            _diceRb.velocity = Vector3.zero;
            _diceRb.angularVelocity = Vector3.zero;

            // Zarın yüksekten bırakılması ve rastgele rotasyon verilmesi
            Vector3 initialForce = new Vector3(0f, -2f, 0f);  // Aşağıya doğru bir kuvvet
            _diceRb.AddForce(initialForce, ForceMode.Impulse);

            // Rastgele dönme hareketi ekliyoruz
            Vector3 randomTorque = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            _diceRb.AddTorque(randomTorque, ForceMode.Impulse);

            // Durma zamanını sıfırla
            _stopTime = 0f;

            // Zarın gerçekten durduğunu kontrol etmek için bekle
            yield return new WaitUntil(() => IsDiceStopped());

            // Zar durduktan sonra zarın yüzünü belirle
            int diceValue = GetDiceValue();
            Debug.Log($"Zar durdu, görünen yüz: {diceValue}");

            // Zar durduktan sonra biraz gecikme ekliyoruz (yarım saniye)
            yield return new WaitForSeconds(0.5f);

            // Action ile sonucu bildiriyoruz
            OnDiceRolled?.Invoke(diceValue);
        }


        // Zarın durduğunu kontrol et
        private bool IsDiceStopped()
        {
            // Hem lineer hız hem de açısal hızı kontrol ediyoruz
            if (_diceRb.velocity.magnitude < StopThreshold && _diceRb.angularVelocity.magnitude < StopThreshold)
            {
                _stopTime += Time.deltaTime;  // Eğer zar düşük hızda kalıyorsa süreyi artır
            }
            else
            {
                _stopTime = 0f;  // Eğer zar yeterince yavaşlamadıysa süreyi sıfırla
            }

            // Zarın en az 'stopCheckDuration' kadar süreyle durduğuna emin ol
            return _stopTime > StopCheckDuration;
        }

        private int GetDiceValue()
        {
            // Zarın global dönüş rotasyonunu alıyoruz
            Vector3 upVector = _currentDice.transform.up;

            // Yukarı ve aşağı bakan yüzleri kontrol et
            if (Mathf.Abs(upVector.y - 1f) < 0.1f) return 3;   // Üst yüz 3 (yukarı bakan yüz)
            if (Mathf.Abs(upVector.y + 1f) < 0.1f) return 4;   // Alt yüz 4 (aşağı bakan yüz)

            // Diğer yüzlerin dönüş açılarını kontrol et
            if (Mathf.Abs(_currentDice.transform.forward.y - 1f) < 0.1f) return 2;  // Ön yüz 2
            if (Mathf.Abs(_currentDice.transform.forward.y + 1f) < 0.1f) return 5;  // Arka yüz 5
            if (Mathf.Abs(_currentDice.transform.right.y - 1f) < 0.1f) return 6;    // Sağ yüz 6
            if (Mathf.Abs(_currentDice.transform.right.y + 1f) < 0.1f) return 1;    // Sol yüz 1

            return 0;
        }
    }
}
