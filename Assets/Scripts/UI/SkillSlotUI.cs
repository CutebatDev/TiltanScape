using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Image mainIcon;
    [SerializeField] private TMP_Text levelText;

    [Header("Skill Data")]
    [SerializeField] private SkillDefinition skillDefinition;
    [SerializeField] private Sprite icon;

    private void Awake()
    {
        if (mainIcon && icon)
            mainIcon.sprite = icon;
    }

    public void Refresh(PlayerSkills playerSkills)
    {
        if (!playerSkills || !skillDefinition)
            return;

        if (mainIcon && icon)
            mainIcon.sprite = icon;

        int level = playerSkills.GetLevel(skillDefinition);

        if (levelText)
            levelText.text = level.ToString();
    }
}