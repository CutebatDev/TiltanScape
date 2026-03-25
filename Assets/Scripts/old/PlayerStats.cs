using System;
using Collision;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public event Action<float, float> OnHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void OnEnable()
    {
        CollisionSystem.OnEnemyTouched += OnEnemyContact;
    }

    private void OnDisable()
    {
        CollisionSystem.OnEnemyTouched += OnEnemyContact;
    }

    public void OnEnemyContact(GameObject enemy)
    {
        float damage = enemy.GetComponent<EnemyStats>().ContactDamage;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}