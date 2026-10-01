using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public int attackDamage = 20;

    public LayerMask enemyLayer;

    private CameraFollow cameraFollow;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void HitAttack()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        bool hitSuccess = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, (Vector2)transform.position);
                hitSuccess = true;
            }
        }

        if (hitSuccess)
        {
            HitStopManager.Instance.Stop(0.05f);

            cameraFollow.Shake(0.08f, 0.05f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}