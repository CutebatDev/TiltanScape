using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance { get; private set; }

    private Dictionary<SkillDefinition, int> xp = new();

    public delegate void SkillChanged(SkillDefinition skill);
    public event SkillChanged OnSkillLevelChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddXP(SkillDefinition skill, int amount)
    {
        if (!xp.ContainsKey(skill))
            xp[skill] = 0;

        int oldLevel = GetLevel(skill);
        xp[skill] += amount;
        int newLevel = GetLevel(skill);

        if (newLevel > oldLevel)
            OnSkillLevelChanged?.Invoke(skill);
    }

    public int GetXP(SkillDefinition skill) => xp.TryGetValue(skill, out int value) ? value : 0;
    public int GetLevel(SkillDefinition skill) => SkillXP.GetLevelForXP(GetXP(skill));
}
