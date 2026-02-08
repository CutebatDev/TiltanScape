using System;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    [SerializeField] private string questTriggerTag = "QuestTrigger";
    [SerializeField] private string enemyTag = "Enemy";

    private static CollisionSystem instance;

    public static event Action<GameObject> OnQuestTrigger;
    public static event Action<GameObject> OnEnemyTouched;

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
            return;

        if (other.CompareTag(instance.questTriggerTag))
        {
            OnQuestTrigger?.Invoke(other);
        }

        if (other.CompareTag(instance.enemyTag))
        {
            OnEnemyTouched?.Invoke(other);
        }
    }
}
