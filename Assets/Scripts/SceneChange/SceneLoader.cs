using System;
using System.Threading.Tasks;
using Collision;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace SceneChange
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image loadingBar;
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Camera loadingCamera;
        [SerializeField] private SceneGroup[] sceneGroups;

        private float _targetProgress;
        private bool _isLoading;

        private readonly SceneGroupManager _sceneGroupManager = new SceneGroupManager();

        private void Awake()
        {
            _sceneGroupManager.OnSceneLoaded += sceneName => Debug.Log("Scene loaded: " + sceneName);
            _sceneGroupManager.OnSceneUnloaded += sceneName => Debug.Log("Scene unloaded: " + sceneName);
            _sceneGroupManager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        async void Start()
        {
            await LoadSceneGroup(0);

            CollisionSystem.OnNextScene += o => FindSceneIndex(o);
            EventsManager.Instance.OnNextScene += () => FindSceneIndex("Level_SecondFloor");
        }

        void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFullSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount =
                Mathf.MoveTowards(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFullSpeed);
        }

        private async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            _targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.OnProgressed += target => _targetProgress = Mathf.Max(target, _targetProgress);

            EnableLoadingCanvas();
            await _sceneGroupManager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }

        async void FindSceneIndex(string sceneName)
        {
            foreach (var sceneGroup in sceneGroups)
            {
                if (sceneName == sceneGroup.groupName)
                {
                    int index = Array.IndexOf(sceneGroups, sceneGroup);
                    await LoadSceneGroup(index);
                }
            }
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgressed;

        private const float Ratio = 1f;

        public void Report(float value)
        {
            OnProgressed?.Invoke(value / Ratio);
        }
    }
}