using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 10;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemyHealth == null || enemyHealth.IsDead)
        {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(contactDamage, (Vector2)enemyHealth.transform.position);
    }
}