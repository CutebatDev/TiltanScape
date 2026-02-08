using UnityEngine;
using UnityEngine.UI;

public class HealthBarUpdater : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private PlayerStats playerStats;

    private void Start()
    {
        healthFill.fillAmount = 1;
    }

    public void UpdateHealthBar()
    {
        healthFill.fillAmount = playerStats.CurrentHealth / playerStats.MaxHealth;
    }
}
