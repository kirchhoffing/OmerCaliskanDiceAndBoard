using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Map;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance;  // Singleton instance
    public GameObject applePrefab;
    public GameObject pearPrefab;
    public GameObject strawberryPrefab;
    public float yOffset = 1.0f;  // Y ekseninde offset

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Sahne geçişlerinde kalıcı olmasını sağla
        }
        else
        {
            Destroy(gameObject);  // Zaten bir instance varsa yeni objeyi yok et
        }
    }

    private void Start()
    {
        MapLoader.OnMapLoaded += GenerateRandomFruits;  // Harita yüklendiğinde meyveleri oluştur
    }

    private void OnDestroy()
    {
        MapLoader.OnMapLoaded -= GenerateRandomFruits;  // Abonelikten çık
    }

    public void SaveCollectibleData()
    {
        CollectibleData collectibleData = new CollectibleData();

        // Toplanabilir meyvelerin pozisyonlarını kaydet
        foreach (GameObject fruit in GameObject.FindGameObjectsWithTag("Collectible"))
        {
            FruitData fruitData = new FruitData();
            fruitData.name = fruit.name;
            fruitData.position = fruit.transform.position;
            collectibleData.fruits.Add(fruitData);
        }

        // JSON'a kaydet
        string json = JsonUtility.ToJson(collectibleData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "collectiblesData.json"), json);
        Debug.Log("Meyve verileri JSON dosyasına kaydedildi.");
    }

    private void LoadCollectibleData()
    {
        string path = Path.Combine(Application.persistentDataPath, "collectiblesData.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CollectibleData collectibleData = JsonUtility.FromJson<CollectibleData>(json);

            // JSON dosyasından meyveleri sahneye yükle
            foreach (FruitData fruit in collectibleData.fruits)
            {
                GameObject prefab = GetFruitPrefabByName(fruit.name);
                if (prefab != null)
                {
                    Instantiate(prefab, fruit.position, Quaternion.identity);
                }
            }

            Debug.Log("Meyve verileri JSON dosyasından yüklendi.");
        }
    }

    private void GenerateRandomFruits()
    {
        // Tile'ların üzerine Y ekseni offset ile meyve yerleştir
        List<Transform> tiles = MapManager.instance.tiles;

        // Rastgele tile'lar seçerek meyve yerleştir
        for (int i = 0; i < 5; i++) 
        {
            Transform randomTile = GetRandomTile(tiles);
            Vector3 fruitPosition = randomTile.position + new Vector3(0, yOffset, 0);  // Y ekseni offset ekle
            Instantiate(applePrefab, fruitPosition, Quaternion.identity);
        }
        for (int i = 0; i < 3; i++) 
        {
            Transform randomTile = GetRandomTile(tiles);
            Vector3 fruitPosition = randomTile.position + new Vector3(0, yOffset, 0);  // Y ekseni offset ekle
            Instantiate(pearPrefab, fruitPosition, Quaternion.identity);
        }
        for (int i = 0; i < 7; i++) 
        {
            Transform randomTile = GetRandomTile(tiles);
            Vector3 fruitPosition = randomTile.position + new Vector3(0, yOffset, 0);  // Y ekseni offset ekle
            Instantiate(strawberryPrefab, fruitPosition, Quaternion.identity);
        }

        Debug.Log("Rastgele tile'ların üzerine meyveler yerleştirildi.");
    }

    private Transform GetRandomTile(List<Transform> tiles)
    {
        int randomIndex = Random.Range(0, tiles.Count);
        return tiles[randomIndex];
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
public class CollectibleData
{
    public List<FruitData> fruits = new List<FruitData>();
}

[System.Serializable]
public class FruitData
{
    public string name;
    public Vector3 position;
}
