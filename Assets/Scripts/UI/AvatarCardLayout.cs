using UnityEngine;

[ExecuteAlways]
public class AvatarCardLayout : MonoBehaviour
{
    [SerializeField] private RectTransform avatarImage;
    [SerializeField] private RectTransform nameBox;
    [SerializeField] private RectTransform levelBox;

    [Header("Proportions")]
    [SerializeField] private float avatarSizePercent = 0.65f;
    [SerializeField] private float nameBoxHeightPercent = 0.18f;
    [SerializeField] private float levelBoxWidthPercent = 0.42f;
    [SerializeField] private float levelBoxHeightPercent = 0.22f;

    private RectTransform rectTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        RefreshLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        RefreshLayout();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
            RefreshLayout();
    }
#endif

    public void RefreshLayout()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        float size = rectTransform.rect.width;

        if (avatarImage != null)
        {
            float avatarSize = size * avatarSizePercent;
            avatarImage.anchorMin = new Vector2(0.5f, 0.5f);
            avatarImage.anchorMax = new Vector2(0.5f, 0.5f);
            avatarImage.pivot = new Vector2(0.5f, 0.5f);
            avatarImage.anchoredPosition = new Vector2(0f, 0f);
            avatarImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, avatarSize);
            avatarImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, avatarSize);
        }

        if (nameBox != null)
        {
            float nameHeight = size * nameBoxHeightPercent;
            nameBox.anchorMin = new Vector2(0f, 0f);
            nameBox.anchorMax = new Vector2(1f, 0f);
            nameBox.pivot = new Vector2(0.5f, 0f);
            nameBox.offsetMin = new Vector2(0f, 0f);
            nameBox.offsetMax = new Vector2(0f, nameHeight);
        }

        if (levelBox != null)
        {
            float levelWidth = size * levelBoxWidthPercent;
            float levelHeight = size * levelBoxHeightPercent;

            levelBox.anchorMin = new Vector2(0f, 1f);
            levelBox.anchorMax = new Vector2(0f, 1f);
            levelBox.pivot = new Vector2(0f, 1f);
            levelBox.anchoredPosition = Vector2.zero;
            levelBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, levelWidth);
            levelBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, levelHeight);
        }
    }
}