using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject damagePopupPrefab;
    public Transform damagePopupPoint;

    public int maxHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
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
    }

}