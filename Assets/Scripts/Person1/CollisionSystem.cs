using System;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    [SerializeField] private string questTriggerTag = "QuestTrigger";

    private static CollisionSystem instance;

    public static event Action<GameObject> OnQuestTrigger;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static void HandleTrigger(GameObject other)
    {
        if (instance == null)
        {
            return;
        }

        if (other.CompareTag(instance.questTriggerTag))
        {
            OnQuestTrigger?.Invoke(other);
        }
    }
}
