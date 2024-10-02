using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton instance
    public GameObject player;  // Karakterin referansı
    public int playerTileIndex;  // Piyonun bulunduğu Tile'ın indeksi
    public Quaternion playerRotation;  // Karakterin rotasyonu

    private string savePath;

    private void Awake()
    {
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

        // Karakterin bulunduğu tile indeksini ve rotasyonunu kaydet
        gameData.playerTileIndex = playerTileIndex;
        gameData.playerRotation = player.transform.rotation;

        // JSON'a kaydet
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            // Karakterin bulunduğu tile indeksini ve rotasyonunu yükle
            playerTileIndex = gameData.playerTileIndex;
            player.transform.rotation = gameData.playerRotation;
        }
    }
}

[System.Serializable]
public class GameData
{
    public int playerTileIndex;  // Karakterin bulunduğu Tile'ın indeksi
    public Quaternion playerRotation;  // Karakterin rotasyonu
}
