using UnityEngine;

public class BalancingTester : MonoBehaviour
{
    [SerializeField] private BalancingMinigameUI balancingGame;

    private void Start()
    {
        if (balancingGame != null)
        {
            balancingGame.OpenMinigame();
        }
    }
}