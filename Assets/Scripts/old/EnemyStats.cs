using Collision;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    [Header("Events")] [SerializeField] private UnityEvent onDie;

    private void OnEnable() => CollisionSystem.OnPlayerTouched += OnPlayerContact;

    private void OnDisable() => CollisionSystem.OnPlayerTouched -= OnPlayerContact;

    private void OnPlayerContact(GameObject player)
    {
        if (player != null && player.CompareTag("Player") && gameObject.activeSelf)
        {
            onDie?.Invoke();
            Die();
        }
    }

    public void Die() => Destroy(gameObject);

    public float ContactDamage => damage;
}