using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestButtonUI : MonoBehaviour
{
    public TMP_Text text;
    public LayoutElement layoutElement;

    public float horizontalPadding = 24f;
    public float minWidth = 120f;
    public float maxWidth = 500f;

    public void SetText(string questName)
    {
        text.text = questName;
        UpdateWidth();
    }

    void UpdateWidth()
    {
        float width = text.preferredWidth + horizontalPadding;
        width = Mathf.Clamp(width, minWidth, maxWidth);

        layoutElement.preferredWidth = width;
    }
}