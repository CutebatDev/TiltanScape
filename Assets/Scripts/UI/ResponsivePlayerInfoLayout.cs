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

    [Header("Card Size")]
    [SerializeField] private float cardWidthPercent = 0.9f;
    [SerializeField] private float minCardSize = 100f;
    [SerializeField] private float maxCardSize = 260f;

    [Header("Container Padding")]
    [SerializeField] private float paddingPercent = 0.04f;
    [SerializeField] private float minPadding = 4f;
    [SerializeField] private float maxPadding = 20f;

    [Header("Internal Proportions")]
    [SerializeField] private float avatarSizePercent = 0.68f;
    [SerializeField] private float nameHeightPercent = 0.20f;
    [SerializeField] private float levelWidthPercent = 0.42f;
    [SerializeField] private float levelHeightPercent = 0.22f;

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

        // ----- 1) Container fills PlayerInfoArea -----
        playerCardContainer.anchorMin = new Vector2(0f, 0f);
        playerCardContainer.anchorMax = new Vector2(1f, 1f);
        playerCardContainer.pivot = new Vector2(1f, 0f);
        playerCardContainer.offsetMin = Vector2.zero;
        playerCardContainer.offsetMax = Vector2.zero;

        float containerWidth = playerCardContainer.rect.width;
        float containerHeight = playerCardContainer.rect.height;

        if (containerWidth <= 0f || containerHeight <= 0f)
            return;

        // ----- 2) Responsive padding -----
        float padding = Mathf.Clamp(Mathf.Min(containerWidth, containerHeight) * paddingPercent, minPadding, maxPadding);

        // ----- 3) Card size -----
        float targetCardSizeByWidth = containerWidth * cardWidthPercent;
        float targetCardSizeByHeight = containerHeight - padding * 2f;

        float cardSize = Mathf.Min(targetCardSizeByWidth, targetCardSizeByHeight);
        cardSize = Mathf.Clamp(cardSize, minCardSize, maxCardSize);

        // ----- 4) AvatarCard anchored bottom-right -----
        avatarCard.anchorMin = new Vector2(1f, 0f);
        avatarCard.anchorMax = new Vector2(1f, 0f);
        avatarCard.pivot = new Vector2(1f, 0f);
        avatarCard.anchoredPosition = new Vector2(-padding, padding);
        avatarCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardSize);
        avatarCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardSize);

        // ----- 5) Frame fills full card -----
        if (frameImage != null)
        {
            frameImage.anchorMin = new Vector2(0f, 0f);
            frameImage.anchorMax = new Vector2(1f, 1f);
            frameImage.pivot = new Vector2(0.5f, 0.5f);
            frameImage.offsetMin = Vector2.zero;
            frameImage.offsetMax = Vector2.zero;
        }

        // ----- 6) Avatar centered and smaller than frame -----
        if (avatarImage != null)
        {
            float avatarSize = cardSize * avatarSizePercent;

            avatarImage.anchorMin = new Vector2(0.5f, 0.5f);
            avatarImage.anchorMax = new Vector2(0.5f, 0.5f);
            avatarImage.pivot = new Vector2(0.5f, 0.5f);

            // small upward shift so it does not collide with the name bar
            float nameHeight = cardSize * nameHeightPercent;
            float verticalOffset = nameHeight * 0.15f;

            avatarImage.anchoredPosition = new Vector2(0f, verticalOffset);
            avatarImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, avatarSize);
            avatarImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, avatarSize);
        }

        // ----- 7) Level box top-left -----
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

        // ----- 8) Name box bottom full width -----
        if (nameBox != null)
        {
            float nameHeight = cardSize * nameHeightPercent;

            nameBox.anchorMin = new Vector2(0f, 0f);
            nameBox.anchorMax = new Vector2(1f, 0f);
            nameBox.pivot = new Vector2(0.5f, 0f);
            nameBox.offsetMin = new Vector2(0f, 0f);
            nameBox.offsetMax = new Vector2(0f, nameHeight);
        }
    }
}