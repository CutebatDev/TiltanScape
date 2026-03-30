using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ResponsivePlayerInfoLayout : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private RectTransform playerCardContainer;
    [SerializeField] private RectTransform avatarCard;

    [Header("Card Elements")]
    [SerializeField] private RectTransform frameImage;
    [SerializeField] private RectTransform avatarImage;
    [SerializeField] private RectTransform levelBox;
    [SerializeField] private RectTransform nameBox;

    [Header("Text References")]
    [SerializeField] private TMP_Text levelLabelText;
    [SerializeField] private TMP_Text levelValueText;
    [SerializeField] private TMP_Text nameText;

    [Header("Card Size")]
    [SerializeField] private float cardWidthPercent = 0.9f;
    [SerializeField] private float minCardSize = 100f;
    [SerializeField] private float maxCardSize = 260f;

    [Header("Container Padding")]
    [SerializeField] private float paddingPercent = 0.04f;
    [SerializeField] private float minPadding = 4f;
    [SerializeField] private float maxPadding = 20f;

    [Header("Internal Proportions")]
    [SerializeField] private float nameHeightPercent = 0.20f;
    [SerializeField] private float levelWidthPercent = 0.42f;
    [SerializeField] private float levelHeightPercent = 0.22f;

    [Header("Responsive Font Sizes")]
    [SerializeField] private float levelLabelFontPercent = 0.09f;
    [SerializeField] private float levelValueFontPercent = 0.12f;
    [SerializeField] private float nameFontPercent = 0.11f;

    [SerializeField] private float minLevelLabelFontSize = 8f;
    [SerializeField] private float maxLevelLabelFontSize = 18f;

    [SerializeField] private float minLevelValueFontSize = 10f;
    [SerializeField] private float maxLevelValueFontSize = 24f;

    [SerializeField] private float minNameFontSize = 10f;
    [SerializeField] private float maxNameFontSize = 24f;

    [Header("Frame Overscan")]
    [SerializeField] private float frameOverscan = 0f;
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

        if (playerCardContainer == null || avatarCard == null)
            return;

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        if (parentWidth <= 0f || parentHeight <= 0f)
            return;

        // PlayerCardContainer fills PlayerInfoArea
        playerCardContainer.anchorMin = new Vector2(0f, 0f);
        playerCardContainer.anchorMax = new Vector2(1f, 1f);
        playerCardContainer.pivot = new Vector2(1f, 0f);
        playerCardContainer.offsetMin = Vector2.zero;
        playerCardContainer.offsetMax = Vector2.zero;

        float containerWidth = playerCardContainer.rect.width;
        float containerHeight = playerCardContainer.rect.height;

        if (containerWidth <= 0f || containerHeight <= 0f)
            return;

        float padding = Mathf.Clamp(
            Mathf.Min(containerWidth, containerHeight) * paddingPercent,
            minPadding,
            maxPadding
        );

        float targetCardSizeByWidth = containerWidth * cardWidthPercent;
        float targetCardSizeByHeight = containerHeight - padding * 2f;

        float cardSize = Mathf.Min(targetCardSizeByWidth, targetCardSizeByHeight);
        cardSize = Mathf.Clamp(cardSize, minCardSize, maxCardSize);

        // AvatarCard bottom-right
        avatarCard.anchorMin = new Vector2(1f, 0f);
        avatarCard.anchorMax = new Vector2(1f, 0f);
        avatarCard.pivot = new Vector2(1f, 0f);
        avatarCard.anchoredPosition = new Vector2(-padding, padding);
        avatarCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardSize);
        avatarCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardSize);

        // Frame fills full card
        if (frameImage != null)
        {
            frameImage.anchorMin = new Vector2(0f, 0f);
            frameImage.anchorMax = new Vector2(1f, 1f);
            frameImage.pivot = new Vector2(0.5f, 0.5f);

            frameImage.offsetMin = new Vector2(-frameOverscan, -frameOverscan);
            frameImage.offsetMax = new Vector2(frameOverscan, frameOverscan);
        }

        // Avatar fills full card too
        if (avatarImage != null)
        {
            avatarImage.anchorMin = new Vector2(0f, 0f);
            avatarImage.anchorMax = new Vector2(1f, 1f);
            avatarImage.pivot = new Vector2(0.5f, 0.5f);
            avatarImage.offsetMin = Vector2.zero;
            avatarImage.offsetMax = Vector2.zero;
        }

        // Level box top-left
        if (levelBox != null)
        {
            float levelWidth = cardSize * levelWidthPercent;
            float levelHeight = cardSize * levelHeightPercent;

            levelBox.anchorMin = new Vector2(0f, 1f);
            levelBox.anchorMax = new Vector2(0f, 1f);
            levelBox.pivot = new Vector2(0f, 1f);
            levelBox.anchoredPosition = new Vector2(0f, 0f);
            levelBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, levelWidth);
            levelBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, levelHeight);
        }

        // Name box bottom full width
        if (nameBox != null)
        {
            float nameHeight = cardSize * nameHeightPercent;

            nameBox.anchorMin = new Vector2(0f, 0f);
            nameBox.anchorMax = new Vector2(1f, 0f);
            nameBox.pivot = new Vector2(0.5f, 0f);
            nameBox.offsetMin = new Vector2(0f, 0f);
            nameBox.offsetMax = new Vector2(0f, nameHeight);
        }

        // Responsive font sizes
        ApplyResponsiveText(levelLabelText, cardSize * levelLabelFontPercent, minLevelLabelFontSize, maxLevelLabelFontSize);
        ApplyResponsiveText(levelValueText, cardSize * levelValueFontPercent, minLevelValueFontSize, maxLevelValueFontSize);
        ApplyResponsiveText(nameText, cardSize * nameFontPercent, minNameFontSize, maxNameFontSize);
    }

    private void ApplyResponsiveText(TMP_Text textComponent, float targetSize, float minSize, float maxSize)
    {
        if (textComponent == null)
            return;

        textComponent.fontSize = Mathf.Clamp(targetSize, minSize, maxSize);
    }
}