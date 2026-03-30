using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Save_System
{
    public class PlayerSkillsData
    {
        public string[] SkillNames;
        public int[] SkillsXp;
        public int[] SkillsLevel;

        public PlayerSkillsData(Dictionary<SkillDefinition, int> xp)
        {
            SkillNames = new string[xp.Count];
            SkillsXp = new int[xp.Count];
            SkillsLevel = new int[xp.Count];

            int i = 0;
            foreach (var kvp in xp)
            {
                SkillNames[i] = kvp.Key.name;
                SkillsXp[i] = kvp.Value;
                SkillsLevel[i] = SkillXP.GetLevelForXP(kvp.Value);
                i++;
            }
        }

        public PlayerSkillsData()
        {
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(SkillNames.Length);
            for (int i = 0; i < SkillNames.Length; i++)
            {
                writer.Write(SkillNames[i]);
                writer.Write(SkillsXp[i]);
                writer.Write(SkillsLevel[i]);
            }
        }

        public static PlayerSkillsData Read(BinaryReader reader)
        {
            PlayerSkillsData data = new PlayerSkillsData();
            int count = reader.ReadInt32();
            data.SkillNames = new string[count];
            data.SkillsXp = new int[count];
            data.SkillsLevel = new int[count];

            for (int i = 0; i < count; i++)
            {
                data.SkillNames[i] = reader.ReadString();
                data.SkillsXp[i] = reader.ReadInt32();
                data.SkillsLevel[i] = reader.ReadInt32();
            }

            return data;
        }
    }
}