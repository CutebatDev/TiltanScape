using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Save_System
{
    public static class SaveSystem
    {
        public static void SaveSkills()
        {
            string path = Application.persistentDataPath + "/playerSkillsData.bin";
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                PlayerSkillsData playerSkillsData = new PlayerSkillsData(PlayerSkills.Instance.GetXpDictionary());
                formatter.Serialize(stream, playerSkillsData);
            }
        }

        public static PlayerSkillsData LoadSkills()
        {
            string path = Application.persistentDataPath + "/playerSkillsData.bin";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    PlayerSkillsData playerSkillsData = formatter.Deserialize(stream) as PlayerSkillsData;
                    return playerSkillsData;
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
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                PlayerQuestData questData = QuestManager.Instance.GetQuestSaveData();
                formatter.Serialize(stream, questData);
            }
        }

        public static PlayerQuestData LoadQuests()
        {
            string path = Application.persistentDataPath + "/questData.bin";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    PlayerQuestData questData = formatter.Deserialize(stream) as PlayerQuestData;
                    return questData;
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