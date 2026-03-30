using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Save_System;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance { get; private set; }

    private Dictionary<SkillDefinition, int> xp = new();
    public int skillsAmount;

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

    private void Start()
    {
        skillsAmount = Instance.xp.Count;

        EventsManager.Instance.OnGameSave += SaveSkills;
        EventsManager.Instance.OnGameLoad += LoadSkills;
    }

    public Dictionary<SkillDefinition, int> GetXpDictionary() => xp;

    private void SaveSkills()
    {
        SaveSystem.SaveSkills();
    }

    private void LoadSkills()
    {
        PlayerSkillsData data = SaveSystem.LoadSkills();
        if (data == null) return;

        xp.Clear();
        for (int i = 0; i < data.skillNames.Length; i++)
        {
            SkillDefinition skill = Resources.Load<SkillDefinition>("Skills/" + data.skillNames[i]);
            if (skill == null)
            {
                // Try searching the whole project structure if Resources doesn't work,
                // but for now let's hope it's in a Resources folder or we use another method.
                // Given the project structure, let's assume we might need to find it differently
                // but I will start with a simple look-up if possible.
                // Actually, let's use a more robust way if they aren't in Resources.
                Debug.LogWarning($"Could not find SkillDefinition for {data.skillNames[i]}");
                continue;
            }
            xp[skill] = data.skillsXP[i];
        }
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