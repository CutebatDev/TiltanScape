using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class EventsManager : MonoBehaviour
    {
        public static EventsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public UnityAction OnTogglePauseMenu;
        public UnityAction OnNextScene;
    }
}