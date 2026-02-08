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

    private void OnEnable()
    {
        playerStats.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        playerStats.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        healthFill.fillAmount = current / max;
    }

}
