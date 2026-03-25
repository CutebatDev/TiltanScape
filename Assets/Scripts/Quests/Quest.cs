using UnityEngine;

public class Quest
{
    public QuestData Data { get; }

    public float Progress { get; private set; } // 0 - 1
    public bool IsCompleted => Progress >= 1f;
    public bool IsTurnedIn { get; private set; }

    public Quest(QuestData data)
    {
        Data = data;
        Progress = 0f;
    }

    public void AddProgress(float amount)
    {
        if (IsCompleted) return;

        Progress += amount;
        Progress = Mathf.Clamp01(Progress);
    }

    public void TurnIn()
    {
        if (!IsCompleted) return;

        IsTurnedIn = true;
    }
}
