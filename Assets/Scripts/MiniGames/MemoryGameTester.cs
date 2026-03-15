using UnityEngine;

public class MemoryGameTester : MonoBehaviour
{
    [SerializeField] private MemoryGameMinigame memoryGame;

    private void Start()
    {
        if (memoryGame != null)
        {
            memoryGame.OpenMinigame();
        }
    }
}