using System;
using System.Collections.Generic;
using System.Linq;

namespace Save_System
{
    [Serializable]
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
    }
}
