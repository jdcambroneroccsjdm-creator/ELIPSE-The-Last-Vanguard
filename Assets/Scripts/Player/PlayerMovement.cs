
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 0.5f;
    public float runSpeed = 1f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("First Person Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float cameraVerticalLimit = 80f;

    [Header("Cursor Settings")]
    public KeyCode cursorToggleKey = KeyCode.LeftAlt;
    public bool cursorVisible = false;

    [Header("Dash")]
    public KeyCode dashKey = KeyCode.Space;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;
    public float dashStaminaCost = 25f;

    [Header("Attack Animation Speed")]
    [Range(0.1f, 3f)]
    public float attackAnimationSpeed = 1f;

    [Header("References")]
    public Animator animator;
    public TargetLockOn targetLockOn;
    public PlayerAttack playerAttack;
    public PlayerStamina playerStamina;

    private CharacterController controller;

    private float verticalVelocity;
    private float dashTimer;
    private float dashCooldownTimer;
    private float cameraPitch;

    private Vector3 dashDirection;

    private bool isDashing;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (targetLockOn == null)
            targetLockOn = GetComponent<TargetLockOn>();

        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();

        if (playerStamina == null)
            playerStamina = GetComponent<PlayerStamina>();
    }

    private void Start()
    {
        if (playerCamera != null)
        {
            cameraPitch = 0f;
            playerCamera.localRotation = Quaternion.identity;
        }

        SetCursorState(cursorVisible);
    }

    private void Update()
    {
        HandleCursorToggle();

        HandleMouseLook();

        if (playerStamina != null)
            playerStamina.SetRegenerationPaused(isDashing);

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        UpdateAttackAnimationSpeed();

        if (isDashing)
        {
            HandleDashMovement();
            return;
        }

        if (playerAttack != null && playerAttack.IsAttacking)
        {
            FaceLockedTarget();
            HandleVerticalMovement();
            SetMovementAnimation(false, false);
            return;
        }

        if (Input.GetKeyDown(dashKey) &&
            dashCooldownTimer <= 0f)
        {
            if (StartDash())
            {
                HandleDashMovement();
                return;
            }
        }

        HandleMovement();

        if (playerAttack != null)
            playerAttack.HandleAttackInput();
    }

    // ALT KEY CURSOR TOGGLE
    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(cursorToggleKey) ||
            Input.GetKeyDown(KeyCode.RightAlt))
        {
            cursorVisible = !cursorVisible;
            SetCursorState(cursorVisible);
        }
    }

    private void SetCursorState(bool visible)
    {
        cursorVisible = visible;

        Cursor.visible = visible;

        Cursor.lockState = visible
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
            return;

        // Do not rotate camera when cursor is visible
        if (cursorVisible)
            return;

        float mouseX =
            Input.GetAxis("Mouse X") * mouseSensitivity;

        float mouseY =
            Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(
            Vector3.up * mouseX,
            Space.World
        );

        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            -cameraVerticalLimit,
            cameraVerticalLimit
        );

        playerCamera.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void UpdateAttackAnimationSpeed()
    {
        if (animator == null)
            return;

        if (playerAttack != null && playerAttack.IsAttacking)
            animator.speed = attackAnimationSpeed;
        else
            animator.speed = 1f;
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            transform.right * x +
            transform.forward * z;

        direction = Vector3.ClampMagnitude(direction, 1f);

        bool isMoving = direction.sqrMagnitude > 0.01f;

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) && isMoving;

        float speed = isRunning ? runSpeed : walkSpeed;

        HandleGravity();

        Vector3 movement = direction * speed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        SetMovementAnimation(isMoving, isRunning);
    }

    private bool StartDash()
    {
        if (playerStamina != null &&
            !playerStamina.TryUseStamina(dashStaminaCost))
        {
            return false;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            transform.right * x +
            transform.forward * z;

        direction = Vector3.ClampMagnitude(direction, 1f);

        Transform target = GetLockedTarget();

        if (target != null)
        {
            if (direction.sqrMagnitude < 0.01f)
                direction = target.position - transform.position;

            direction.y = 0f;
            direction.Normalize();
        }
        else if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        dashDirection = direction;

        if (dashDirection.sqrMagnitude < 0.001f)
            dashDirection = transform.forward;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        SetMovementAnimation(false, false);

        return true;
    }

    private void HandleDashMovement()
    {
        FaceLockedTarget();

        HandleGravity();

        Vector3 movement = dashDirection * dashSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
            isDashing = false;

        SetMovementAnimation(false, false);
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleVerticalMovement()
    {
        HandleGravity();

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

    private Transform GetLockedTarget()
    {
        if (targetLockOn == null)
            return null;

        return targetLockOn.CurrentTarget;
    }

    private void FaceLockedTarget()
    {
        Transform target = GetLockedTarget();

        if (target == null)
            return;

        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void SetMovementAnimation(bool moving, bool running)
    {
        if (animator == null)
            return;

        animator.SetBool("IsMoving", moving);
        animator.SetBool("IsRunning", running);
    }

    public void ResetForRespawn()
    {
        isDashing = false;

        dashTimer = 0f;
        dashCooldownTimer = 0f;
        verticalVelocity = 0f;

        if (playerAttack != null)
            playerAttack.ResetForRespawn();

        if (playerStamina != null)
            playerStamina.RestoreFullStamina();

        if (animator != null)
        {
            animator.speed = 1f;

            animator.Rebind();
            animator.Update(0f);

            animator.SetBool("IsMoving", false);
            animator.SetBool("IsRunning", false);
        }

        cameraPitch = 0f;

        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        if (animator != null)
            animator.speed = 1f;
    }

    private void OnDestroy()
    {
    }
}
