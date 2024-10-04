using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerTileIndex;
    public GameObject player;
    public Quaternion playerRotation;

    private string savePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        LoadGameData();
    }

    private void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            playerTileIndex = gameData.playerTileIndex;
            playerRotation = new Quaternion(gameData.rotationX, gameData.rotationY, gameData.rotationZ, gameData.rotationW);
        }
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData();
        gameData.playerTileIndex = playerTileIndex;

        var rotation = player.transform.rotation;
        gameData.rotationX = rotation.x;
        gameData.rotationY = rotation.y;
        gameData.rotationZ = rotation.z;
        gameData.rotationW = rotation.w;

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
    }
}

[System.Serializable]
public class GameData
{
    public int playerTileIndex;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
}