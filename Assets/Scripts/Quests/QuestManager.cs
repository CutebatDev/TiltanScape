using Collision;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    void OnEnable()
    {
        CollisionSystem.OnQuestTrigger += LogQuest;
    }

    void OnDisable()
    {
        CollisionSystem.OnQuestTrigger -= LogQuest;
    }

    void LogQuest(GameObject obj)
    {
        Debug.Log($"Quest Triggered: {obj.name}");
    }
}