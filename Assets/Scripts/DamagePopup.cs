using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifeTime = 0.8f;

    public TextMeshProUGUI damageText;

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(int damage)
    {
        damageText.text = damage.ToString();
    }
}