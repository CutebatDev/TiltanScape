using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Save_System
{
    public class PlayerQuestData
    {
        public List<string> activeQuestIds;
        public List<float> activeQuestProgress;
        public List<string> completedQuestIds;

        public PlayerQuestData(Dictionary<string, Quest> activeQuests, HashSet<string> completedQuests)
        {
            activeQuestIds = activeQuests.Keys.ToList();
            activeQuestProgress = activeQuests.Values.Select(q => q.Progress).ToList();
            completedQuestIds = completedQuests.ToList();
        }

        public PlayerQuestData()
        {
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(activeQuestIds.Count);
            foreach (var id in activeQuestIds)
            {
                writer.Write(id);
            }

            writer.Write(activeQuestProgress.Count);
            foreach (var progress in activeQuestProgress)
            {
                writer.Write(progress);
            }

            writer.Write(completedQuestIds.Count);
            foreach (var id in completedQuestIds)
            {
                writer.Write(id);
            }
        }

        public static PlayerQuestData Read(BinaryReader reader)
        {
            PlayerQuestData data = new PlayerQuestData();

            int activeCount = reader.ReadInt32();
            data.activeQuestIds = new List<string>(activeCount);
            for (int i = 0; i < activeCount; i++)
            {
                data.activeQuestIds.Add(reader.ReadString());
            }

            int progressCount = reader.ReadInt32();
            data.activeQuestProgress = new List<float>(progressCount);
            for (int i = 0; i < progressCount; i++)
            {
                data.activeQuestProgress.Add(reader.ReadSingle());
            }

            int completedCount = reader.ReadInt32();
            data.completedQuestIds = new List<string>(completedCount);
            for (int i = 0; i < completedCount; i++)
            {
                data.completedQuestIds.Add(reader.ReadString());
            }

            return data;
        }
    }
}