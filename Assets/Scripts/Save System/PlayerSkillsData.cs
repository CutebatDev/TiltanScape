using System;
using System.Collections.Generic;
using UnityEngine;

namespace Save_System
{
    [Serializable]
    public class PlayerSkillsData
    {
        public string[] skillNames;
        public int[] skillsXP;

        public PlayerSkillsData(Dictionary<SkillDefinition, int> xp)
        {
            skillNames = new string[xp.Count];
            skillsXP = new int[xp.Count];

            int i = 0;
            foreach (var kvp in xp)
            {
                skillNames[i] = kvp.Key.name;
                skillsXP[i] = kvp.Value;
                i++;
            }
        }
    }
}