using UnityEngine;

[ExecuteAlways]
public class ResponsiveSkillGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;

    [Header("Cell Size Limits")]
    [SerializeField] private float minCellSize = 60f;
    [SerializeField] private float maxCellSize = 140f;

    [Header("Spacing")]
    [SerializeField] private float minSpacingX = 10f;
    [SerializeField] private float minSpacingY = 10f;

    [Header("Padding")]
    [SerializeField] private float paddingLeft = 10f;
    [SerializeField] private float paddingRight = 10f;
    [SerializeField] private float paddingTop = 10f;
    [SerializeField] private float paddingBottom = 10f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateLayout();
    }

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateLayout();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
            UpdateLayout();
    }
#endif

    public void UpdateLayout()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        int childCount = Mathf.Min(transform.childCount, columns * rows);
        if (childCount == 0)
            return;

        float availableWidth = rectTransform.rect.width - paddingLeft - paddingRight;
        float availableHeight = rectTransform.rect.height - paddingTop - paddingBottom;

        if (availableWidth <= 0f || availableHeight <= 0f)
            return;

        // Maximum cell size that still fits in the padded area
        float cellSizeFromWidth = (availableWidth - minSpacingX * (columns - 1)) / columns;
        float cellSizeFromHeight = (availableHeight - minSpacingY * (rows - 1)) / rows;

        float targetCellSize = Mathf.Min(cellSizeFromWidth, cellSizeFromHeight);
        float finalCellSize = Mathf.Clamp(targetCellSize, minCellSize, maxCellSize);

        float usedWidth = columns * finalCellSize;
        float usedHeight = rows * finalCellSize;

        float extraWidth = availableWidth - usedWidth;
        float extraHeight = availableHeight - usedHeight;

        float spacingX = columns > 1 ? extraWidth / (columns - 1) : 0f;
        float spacingY = rows > 1 ? extraHeight / (rows - 1) : 0f;

        spacingX = Mathf.Max(minSpacingX, spacingX);
        spacingY = Mathf.Max(minSpacingY, spacingY);

        float totalGridWidth = columns * finalCellSize + (columns - 1) * spacingX;
        float totalGridHeight = rows * finalCellSize + (rows - 1) * spacingY;

        // Center inside the padded area
        float startX = paddingLeft + (availableWidth - totalGridWidth) * 0.5f;
        float startY = -paddingTop - (availableHeight - totalGridHeight) * 0.5f;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            int row = i / columns;
            int col = i % columns;

            child.anchorMin = new Vector2(0f, 1f);
            child.anchorMax = new Vector2(0f, 1f);
            child.pivot = new Vector2(0f, 1f);

            float x = startX + col * (finalCellSize + spacingX);
            float y = startY - row * (finalCellSize + spacingY);

            child.anchoredPosition = new Vector2(x, y);
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalCellSize);
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalCellSize);
        }
    }
}