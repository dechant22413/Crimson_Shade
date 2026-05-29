using System;

[Serializable]
public class SaveData
{
    public int lastCheckpointIndex;
    public float[] checkpointPosition; // x, y, z
    public float currentLifePoints;
    public float currentStamina;
}
