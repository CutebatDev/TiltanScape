using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerSkills playerSkills;
    [SerializeField] private SkillSlotUI[] skillSlots;

    private void Start()
    {
        RefreshSkillsUI();
    }

    private void RefreshSkillsUI()
    {
        if (!playerSkills) return;

        foreach (SkillSlotUI slot in skillSlots)
        {
            if (!slot) continue;
            slot.Refresh(playerSkills);
        }
    }
}