using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual References")]
    [SerializeField] private Image mainIcon;
    [SerializeField] private TMP_Text levelText;

    [Header("Skill Data")]
    [SerializeField] private SkillDefinition skillDefinition;
    [SerializeField] private Sprite icon;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private TMP_Text tooltipText;

    private void Awake()
    {
        if (mainIcon && icon)
            mainIcon.sprite = icon;
    }

    public void Refresh()
    {
        if (!skillDefinition)
            return;

        if (mainIcon && icon)
            mainIcon.sprite = icon;

        int level = PlayerSkills.Instance.GetLevel(skillDefinition);

        if (levelText)
            levelText.text = level.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip)
            tooltip.SetActive(true);
        tooltipText.text = skillDefinition.skillName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip)
            tooltip.SetActive(false);
    }
}