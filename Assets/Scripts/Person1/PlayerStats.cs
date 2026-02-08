using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    public float MaxHealth;

    public event Action<float, float> OnHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void OnEnable()
    {
        CollisionSystem.OnEnemyTouched += OnEnemyTouched;
    }

    private void OnDisable()
    {
        CollisionSystem.OnEnemyTouched += OnEnemyTouched;
    }

    public void OnEnemyTouched(GameObject enemy)
    {
        float damage = enemy.GetComponent<EnemyStats>().ContactDamage;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
