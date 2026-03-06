using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private Dictionary<SkillDefinition, int> xp = new();

    public void AddXP(SkillDefinition skill, int amount)
    {
        if (!xp.ContainsKey(skill))
            xp[skill] = 0;

        int oldLevel = GetLevel(skill);
        xp[skill] += amount;
        int newLevel = GetLevel(skill);

        if (newLevel > oldLevel)
            OnLevelUp(skill, oldLevel, newLevel);
    }

    public int GetXP(SkillDefinition skill) => xp.TryGetValue(skill, out int value) ? value : 0;
    public int GetLevel(SkillDefinition skill) => SkillXP.GetLevelForXP(GetXP(skill));

    private void OnLevelUp(SkillDefinition skill, int oldLevel, int newLevel)
    {
        Debug.Log($"{skill.skillName} leveled up from {oldLevel} to {newLevel}!");
    }
}
