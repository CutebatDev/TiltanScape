using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls one bug instance inside the bug smash minigame.
/// Handles movement, facing direction, click smash, and sprite swap.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class BugActorUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private Image bugImage;

    private RectTransform rectTransform;
    private RectTransform areaRect;
    private BugSmashMinigame owner;

    private Sprite aliveSprite;
    private Sprite smashedSprite;

    private float moveSpeed;
    private bool isSmashed;
    private bool hasEnteredPlayableArea;

    private Vector2 targetPosition;
    private float targetReachThreshold = 10f;

    /// <summary>
    /// Public state.
    /// </summary>
    public bool IsSmashed => isSmashed;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Initializes this bug instance.
    /// </summary>
    public void Setup(
        BugSmashMinigame minigameOwner,
        RectTransform playableArea,
        Sprite alive,
        Sprite smashed,
        float speed,
        Vector2 spawnPosition,
        Vector2 firstTargetInsideArea)
    {
        owner = minigameOwner;
        areaRect = playableArea;
        aliveSprite = alive;
        smashedSprite = smashed;
        moveSpeed = speed;

        isSmashed = false;
        hasEnteredPlayableArea = false;

        rectTransform.anchoredPosition = spawnPosition;
        targetPosition = firstTargetInsideArea;

        if (bugImage != null)
        {
            bugImage.sprite = aliveSprite;
            bugImage.preserveAspect = true;
        }

        RotateToward(targetPosition);
    }

    private void Update()
    {
        if (isSmashed || owner == null || !owner.IsGameplayRunning)
            return;

        MoveBug();
    }

    private void MoveBug()
    {
        Vector2 current = rectTransform.anchoredPosition;
        Vector2 next = Vector2.MoveTowards(current, targetPosition, moveSpeed * Time.deltaTime);
        rectTransform.anchoredPosition = next;

        RotateToward(targetPosition);

        if (!hasEnteredPlayableArea && IsInsidePlayableBounds(next))
        {
            hasEnteredPlayableArea = true;
            targetPosition = GetRandomPointInsideArea();
            RotateToward(targetPosition);
            return;
        }

        if (Vector2.Distance(next, targetPosition) <= targetReachThreshold)
        {
            if (hasEnteredPlayableArea)
            {
                targetPosition = GetRandomPointInsideArea();
            }
            else
            {
                targetPosition = GetRandomPointInsideArea();
            }

            RotateToward(targetPosition);
        }

        // After the bug enters the playable area, keep it trapped inside.
        if (hasEnteredPlayableArea)
        {
            rectTransform.anchoredPosition = ClampInsideArea(rectTransform.anchoredPosition);
        }
    }

    /// <summary>
    /// Left click smashes the bug.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (isSmashed || owner == null)
            return;

        owner.TrySmashBug(this);
    }

    /// <summary>
    /// Called by the minigame when this bug has been successfully smashed.
    /// </summary>
    public void Smash()
    {
        if (isSmashed)
            return;

        isSmashed = true;

        if (bugImage != null)
        {
            bugImage.sprite = smashedSprite;
            bugImage.preserveAspect = true;
        }
    }

    private void RotateToward(Vector2 destination)
    {
        Vector2 direction = destination - rectTransform.anchoredPosition;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private bool IsInsidePlayableBounds(Vector2 position)
    {
        Rect rect = areaRect.rect;

        return position.x >= rect.xMin &&
               position.x <= rect.xMax &&
               position.y >= rect.yMin &&
               position.y <= rect.yMax;
    }

    private Vector2 ClampInsideArea(Vector2 position)
    {
        Rect rect = areaRect.rect;

        float halfWidth = rectTransform.rect.width * 0.5f;
        float halfHeight = rectTransform.rect.height * 0.5f;

        float clampedX = Mathf.Clamp(position.x, rect.xMin + halfWidth, rect.xMax - halfWidth);
        float clampedY = Mathf.Clamp(position.y, rect.yMin + halfHeight, rect.yMax - halfHeight);

        return new Vector2(clampedX, clampedY);
    }

    private Vector2 GetRandomPointInsideArea()
    {
        Rect rect = areaRect.rect;

        float halfWidth = rectTransform.rect.width * 0.5f;
        float halfHeight = rectTransform.rect.height * 0.5f;

        float randomX = Random.Range(rect.xMin + halfWidth, rect.xMax - halfWidth);
        float randomY = Random.Range(rect.yMin + halfHeight, rect.yMax - halfHeight);

        return new Vector2(randomX, randomY);
    }
}