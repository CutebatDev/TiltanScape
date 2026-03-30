using UnityEngine;
using System.IO;
using Player;

namespace Save_System
{
    public static class SaveSystem
    {
        public static void SaveSkills()
        {
            string path = Application.persistentDataPath + "/playerSkillsData.bin";

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    PlayerSkillsData playerSkillsData = new PlayerSkillsData(PlayerSkills.Instance.GetXpDictionary());
                    playerSkillsData.Write(writer);
                }
            }
        }

        public static PlayerSkillsData LoadSkills()
        {
            string path = Application.persistentDataPath + "/playerSkillsData.bin";
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        PlayerSkillsData playerSkillsData = PlayerSkillsData.Read(reader);
                        return playerSkillsData;
                    }
                }
            }
            else
            {
                Debug.Log("No save file found in" + path);
                return null;
            }
        }

        public static void SaveQuests()
        {
            string path = Application.persistentDataPath + "/questData.bin";

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    PlayerQuestData questData = QuestManager.Instance.GetQuestSaveData();
                    questData.Write(writer);
                }
            }
        }

        public static PlayerQuestData LoadQuests()
        {
            string path = Application.persistentDataPath + "/questData.bin";
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        PlayerQuestData questData = PlayerQuestData.Read(reader);
                        return questData;
                    }
                }
            }
            else
            {
                Debug.Log("No save file found in" + path);
                return null;
            }
        }
    }
}