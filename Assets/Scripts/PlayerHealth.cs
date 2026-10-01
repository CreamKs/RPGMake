using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibleDuration = 0.7f;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private float knockbackSpeed = 4f;
    [SerializeField] private float hitDuration = 0.2f;

    [SerializeField] private float restartDelay = 2f;

    private int currentHealth;
    private float nextDamageTime;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;
    
    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine;

    private Animator animator;
    private PlayerController playerController;

    public bool IsHit { get; private set; }

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;

            rb = GetComponent<Rigidbody2D>();
        }
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (IsDead || damage <= 0)
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        currentHealth = Mathf.Max(
            0,
            currentHealth - damage
        );

        nextDamageTime = Time.time + invincibleDuration;
        PlayHitFlash();
        ApplyKnockback(attackerPosition);

        Debug.Log(
            $"플레이어 피격: {damage}, 남은 체력: {currentHealth}"
        );

        if (IsDead)
        {
            Die();
        }
        else if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }
    private void PlayHitFlash()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitFlashDuration);

        spriteRenderer.color = originalColor;

        hitFlashCoroutine = null;
    }
    private void ApplyKnockback(Vector2 attackerPosition)
    {
        if (rb == null)
        {
            return;
        }

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        float direction =
            transform.position.x >= attackerPosition.x ? 1f : -1f;

        knockbackCoroutine =
            StartCoroutine(Knockback(direction));
    }

    private IEnumerator Knockback(float direction)
    {
        IsHit = true;

        rb.linearVelocity = new Vector2(
            direction * knockbackSpeed,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(hitDuration);

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        IsHit = false;
        knockbackCoroutine = null;
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Attack");
            animator.SetBool("IsDead", true);
        }

        Debug.Log("플레이어 사망");

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }

        IsHit = false;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}