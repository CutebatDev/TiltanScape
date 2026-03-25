using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace SceneChange
{
    [Serializable]
    public class SceneGroup
    {
        public string groupName = "New Scene Group";
        public List<SceneData> scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            return scenes.FirstOrDefault(scene => scene.sceneType == sceneType)?.reference.Name;
        }
    }

    [Serializable]
    public class SceneData
    {
        public SceneReference reference;
        public string Name => reference.Name;
        public SceneType sceneType;
    }

    public enum SceneType
    {
        Level,
        ActiveScene,
        UserInterface,
        MainMenu,
        Cinematic,
        Tooling
    }
}