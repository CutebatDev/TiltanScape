using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerSkills playerSkills;
    [SerializeField] private SkillSlotUI[] skillSlots;

    private void Start()
    {
        RefreshSkillsUI();
    }

    public void RefreshSkillsUI()
    {
        if (playerSkills == null) return;

        foreach (SkillSlotUI slot in skillSlots)
        {
            if (slot == null) continue;
            slot.Refresh(playerSkills);
        }
    }
}