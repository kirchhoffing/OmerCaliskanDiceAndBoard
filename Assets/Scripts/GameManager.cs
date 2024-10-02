using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton instance

    public GameObject applePrefab;
    public GameObject pearPrefab;
    public GameObject strawberryPrefab;
    public GameObject player;  // Karakterin referansı

    private string savePath;

    private void Awake()
    {
        // Singleton yapı
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // GameManager'ı sahne geçişlerinde koru
        }
        else
        {
            Destroy(gameObject);  // Zaten bir instance varsa, yeni objeyi yok et
        }
    }

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        Debug.Log("JSON dosyasının yolu: " + Application.persistentDataPath);

        if (File.Exists(savePath))
        {
            LoadGameData();  // Oyun verilerini yükle
        }
        else
        {
            SaveGameData();  // Oyun verilerini kaydet (ilk defa açıldıysa)
        }
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData();

        // Karakterin pozisyonunu kaydet
        gameData.playerPosition = player.transform.position;

        // Meyvelerin pozisyonlarını kaydet
        foreach (GameObject fruit in GameObject.FindGameObjectsWithTag("Collectible"))
        {
            FruitData fruitData = new FruitData();
            fruitData.name = fruit.name;
            fruitData.position = fruit.transform.position;
            gameData.fruits.Add(fruitData);
        }

        // JSON'a kaydet
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Oyun verileri kaydedildi.");
    }

    private void LoadGameData()
    {
        string json = File.ReadAllText(savePath);
        GameData gameData = JsonUtility.FromJson<GameData>(json);

        // Karakterin pozisyonunu yükle
        player.transform.position = gameData.playerPosition;
        Debug.Log($"Karakter pozisyonu yüklendi: {gameData.playerPosition}");

        // Meyveleri sahneye geri yükle
        foreach (FruitData fruit in gameData.fruits)
        {
            GameObject prefab = GetFruitPrefabByName(fruit.name);
            if (prefab != null)
            {
                Instantiate(prefab, fruit.position, Quaternion.identity);
            }
        }

        Debug.Log("Oyun verileri yüklendi.");
    }


    private GameObject GetFruitPrefabByName(string name)
    {
        switch (name)
        {
            case "apple":
                return applePrefab;
            case "pear":
                return pearPrefab;
            case "strawberry":
                return strawberryPrefab;
            default:
                return null;
        }
    }
}

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public List<FruitData> fruits = new List<FruitData>();
}

[System.Serializable]
public class FruitData
{
    public string name;
    public Vector3 position;
}
