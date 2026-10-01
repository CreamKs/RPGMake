using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private Transform visual;

    private Rigidbody2D rb;
    private EnemyHealth health;

    private Vector3 originalVisualScale;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();

        originalVisualScale = visual.localScale;
    }

    private void FixedUpdate()
    {

        // 피격 중에는 속도를 변경하지 않아 넉백을 유지한다.
        if (health.IsHit)
        {
            return;
        }

        if (health.IsDead || target == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            (Vector2)target.position
        );

        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        float horizontalDistance =
            target.position.x - rb.position.x;

        if (Mathf.Abs(horizontalDistance) <= stopDistance)
        {
            StopMoving();
            return;
        }

        float direction = Mathf.Sign(horizontalDistance);
        UpdateFacingDirection(direction);

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }
    private void UpdateFacingDirection(float direction)
    {
        if (visual == null)
        {
            return;
        }

        visual.localScale = new Vector3(
            Mathf.Abs(originalVisualScale.x) * direction,
            originalVisualScale.y,
            originalVisualScale.z
        );
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}