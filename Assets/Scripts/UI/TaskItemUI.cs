using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text taskLabel;
    [SerializeField] private Image fillImage;

    [Header("Optional")]
    [SerializeField] private TMP_Text percentLabel;

    private float currentProgress = 0f;

    public string TaskId { get; private set; }
    public float CurrentProgress => currentProgress;
    public bool IsComplete => currentProgress >= 1f;

    private void Awake()
    {
        if (taskLabel == null)
            Debug.LogWarning($"{name}: Task label is not assigned.");

        if (fillImage == null)
            Debug.LogWarning($"{name}: Fill image is not assigned.");
    }

    public void Setup(string taskId, string description, float initialProgress = 0f)
    {
        TaskId = taskId;
        SetDescription(description);
        SetProgress(initialProgress);
    }

    public void SetDescription(string description)
    {
        if (taskLabel != null)
            taskLabel.text = description;
    }

    public void SetProgress(float normalizedProgress)
    {
        currentProgress = Mathf.Clamp01(normalizedProgress);

        if (fillImage != null)
            fillImage.fillAmount = currentProgress;

        if (percentLabel != null)
            percentLabel.text = Mathf.RoundToInt(currentProgress * 100f) + "%";
    }

    public void SetProgressPercent(float percent)
    {
        SetProgress(percent / 100f);
    }

    public float GetProgress()
    {
        return currentProgress;
    }
}