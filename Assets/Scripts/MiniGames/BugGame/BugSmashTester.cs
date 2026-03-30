using UnityEngine;

public class BugSmashTester : MonoBehaviour
{
    [SerializeField] private BugSmashMinigame bugSmashGame;

    private void Start()
    {
        if (bugSmashGame != null)
        {
            bugSmashGame.OpenMinigame();
        }
    }
}