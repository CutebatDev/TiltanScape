using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    [Header("Events")]
    [SerializeField] private UnityEvent onDie;

    private void OnEnable() => CollisionSystem.OnEnemyTouched += OnPlayerContact;

    private void OnDisable() => CollisionSystem.OnEnemyTouched -= OnPlayerContact;

    private void OnPlayerContact(GameObject player)
    {
        if (player != null && player.CompareTag("Player") && gameObject.activeSelf)
        {
            onDie?.Invoke();
        }
    }

    public void Die() => Destroy(gameObject);

    public float ContactDamage => damage;
}
