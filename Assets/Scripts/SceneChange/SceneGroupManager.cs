using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneChange
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup _activeSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            var loadedScenes = new List<string>();

            await UnloadScenes(group);

            _activeSceneGroup = group;

            int sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = _activeSceneGroup.scenes.Count;

            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

                var operation = SceneManager.LoadSceneAsync(sceneData.reference.Path, LoadSceneMode.Additive);
                await Task.Delay(TimeSpan.FromSeconds(0.1f)); // TODO: Delete

                operationGroup.operations.Add(operation);

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.InProgress);
                await Task.Delay(100);
            }

            Scene activeScene =
                SceneManager.GetSceneByName(_activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }

        private async Task UnloadScenes(SceneGroup nextGroup)
        {
            List<string> scenes = new List<string>();

            List<string> nextSceneNames = nextGroup.scenes.Select(scene => scene.Name).ToList();
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i <= sceneCount - 1; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                string sceneName = sceneAt.name;
                if (sceneName == "Bootstrapper") continue;
                if (nextSceneNames.Contains(sceneName)) continue;

                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            await Resources.UnloadUnusedAssets();
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> operations;

        public float InProgress => operations.Count == 0 ? 1f : operations.Average(operation => operation.progress);
        public bool IsDone => operations.All(operation => operation.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}