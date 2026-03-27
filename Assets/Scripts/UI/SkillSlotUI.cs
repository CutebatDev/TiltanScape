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
        if (mainIcon != null && icon != null)
            mainIcon.sprite = icon;
    }

    public void Refresh(PlayerSkills playerSkills)
    {
        if (playerSkills == null || skillDefinition == null)
            return;

        if (mainIcon != null && icon != null)
            mainIcon.sprite = icon;

        int level = playerSkills.GetLevel(skillDefinition);

        if (levelText != null)
            levelText.text = level.ToString();
    }
}