using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneChange
{
    public class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async void Init()
        {
            Debug.Log("Bootstrapper...");
            await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
        }
    }
}