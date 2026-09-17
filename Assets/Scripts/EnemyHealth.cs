using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject damagePopupPrefab;
    public Transform damagePopupPoint;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // 이미 죽은 적이면 데미지를 받지 않는다.
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            $"{gameObject.name} 데미지: {damage}, 남은 HP: {currentHealth}"
        );

        GameObject popup = Instantiate(
            damagePopupPrefab,
            damagePopupPoint.position,
            Quaternion.identity
        );

        popup.GetComponent<DamagePopup>().Setup(damage);

        // HP가 0 이하라면 사망
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        isDead = true;

        Debug.Log($"{gameObject.name} 사망");

        Collider2D enemyCollider = GetComponent<Collider2D>();

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Destroy(gameObject, 1f);
    }
}