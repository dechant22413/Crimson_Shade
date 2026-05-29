using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance;

    [Header("References")]
    public Transform player;
    public Checkpoint[] checkpoints;

    private SaveData currentSave;
    private string savePath => Application.persistentDataPath + "/save.dat";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ActivateCheckpoint(Checkpoint checkpoint, int index)
    {
        currentSave = new SaveData
        {
            lastCheckpointIndex = index,
            checkpointPosition = new float[]
            {
                checkpoint.transform.position.x,
                checkpoint.transform.position.y,
                checkpoint.transform.position.z
            },
            currentLifePoints = PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints(),
            currentStamina = PlayerStatsAndUIPanel.Instance.GetStamina()
        };

        Save();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, currentSave);
        file.Close();
    }

    public SaveData Load()
    {
        if (!File.Exists(savePath)) return null;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(savePath, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();
        return data;
    }

    public bool HasSave() => File.Exists(savePath);

    public void RespawnAtLastCheckpoint()
    {
        SaveData data = Load();
        if (data == null) return;

        Vector3 pos = new Vector3(
            data.checkpointPosition[0],
            data.checkpointPosition[1],
            data.checkpointPosition[2]);

        player.position = pos;
        PlayerStatsAndUIPanel.Instance.ChangeLifePoints((int)(data.currentLifePoints - PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints()));
    }
}