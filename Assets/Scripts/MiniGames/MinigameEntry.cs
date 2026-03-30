using UnityEngine;

public enum MinigameType
{
    Memory,
    Bugsplat,
    Balance
}

[System.Serializable]
public struct MinigameEntry
{
    public MinigameType type;
    public MinigameBaseUI minigame;
}
