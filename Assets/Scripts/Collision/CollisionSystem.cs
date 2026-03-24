using System;
using SceneChange;
using UnityEngine;

namespace Collision
{
    public class CollisionSystem : MonoBehaviour
    {
        [SerializeField] private string questTriggerTag = "QuestTrigger";
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string nextSceneTag = "NextScene";

        private static CollisionSystem _instance;

        public static event Action<GameObject> OnQuestTrigger;
        public static event Action<GameObject> OnEnemyTouched;
        public static event Action<GameObject> OnPlayerTouched;
        public static event Action<string> OnNextScene;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }

        public static void HandleTrigger(GameObject other, GameObject self)
        {
            if (_instance == null)
                return;

            if (other.CompareTag(_instance.questTriggerTag))
            {
                OnQuestTrigger?.Invoke(other);
            }

            if (other.CompareTag(_instance.enemyTag))
            {
                OnEnemyTouched?.Invoke(other);
                OnPlayerTouched?.Invoke(self);
            }

            if (other.CompareTag(_instance.nextSceneTag))
            {
                string gameObjectName = other.gameObject.name;
                Destroy(other);
                OnNextScene?.Invoke(gameObjectName);
            }
        }
    }
}