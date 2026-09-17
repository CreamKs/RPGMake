using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private PlayerInputAction inputActions;
    private Vector2 moveInput;
    private PlayerCombat playerCombat;

    private Transform visual;
    private Animator animator;

    public Transform attackPoint;
    private float attackPointX;
    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        inputActions = new PlayerInputAction();
        playerCombat = GetComponent<PlayerCombat>();

        animator = GetComponentInChildren<Animator>();
        visual = transform.Find("Visual");
        attackPointX = Mathf.Abs(attackPoint.localPosition.x);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        // 좌우 이동 입력을 받아옴
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        
        UpdateFacingDirection();

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));

        // 땅에 닿아 있는지 확인
        bool isGrounded = Physics2D.OverlapCircle(
           groundCheck.position,
           groundCheckRadius,
           groundLayer
        ) && rb.linearVelocity.y <= 0;
        animator.SetBool("IsGround", isGrounded);
        animator.SetFloat("JumpSpeed", rb.linearVelocity.y);

        bool isPressingDown = moveInput.y < -0.5f;

        // 공격 입력 처리
        if (inputActions.Player.Attack.WasPressedThisFrame() && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }

        // ↓를 누르고 있는 경우
        if (isPressingDown)
        {
            // ↓ + Alt를 처음 눌렀을 때만 아래 점프 시도
            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                TryDropThroughPlatform();
            }

            return;
        }

        // ↓를 누르고 있지 않을 때만 일반 점프
        if (inputActions.Player.Jump.IsPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpPower
            );
        }

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void TryDropThroughPlatform()
    {
        Collider2D platform = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            platformLayer
        );

        if (platform == null)
        {
            return;
        }

        StartCoroutine(DropThroughPlatform(platform));
    }
    private IEnumerator DropThroughPlatform(Collider2D platform)
    {
        Physics2D.IgnoreCollision(playerCollider, platform, true);

        yield return new WaitForSeconds(0.6f);

        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }

    private void UpdateFacingDirection()
    {
        if (moveInput.x > 0.01f)
        {
            // 캐릭터 그림
            visual.localScale = new Vector3(
                1f,
                visual.localScale.y,
                visual.localScale.z
            );

            // 공격 위치
            attackPoint.localPosition = new Vector3(
                attackPointX,
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
        else if (moveInput.x < -0.01f)
        {
            // 캐릭터 그림
            visual.localScale = new Vector3(
                -1f,
                visual.localScale.y,
                visual.localScale.z
            );

            // 공격 위치
            attackPoint.localPosition = new Vector3(
                -attackPointX,
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
    }
    public void EndAttack()
    {
        isAttacking = false;
    }
}