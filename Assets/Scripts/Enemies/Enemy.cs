
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float detectionRange = 8f;
    public float stopDistance = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;

    [Header("Knockback")]
    public float normalKnockback = 3f;
    public float heavyKnockback = 7f;
    public float knockbackDuration = 0.25f;

    private CharacterController controller;
    private Animator animator;
    private float verticalVelocity;
    private float nextAttackTime;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject found =
                GameObject.FindGameObjectWithTag("Player");

            if (found != null)
                player = found.transform;
        }

        if (player == null)
        {
            Debug.LogError(
                "EnemyAI: Assign the player or set its Tag to Player!",
                this
            );
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;
        bool inRange = distance <= stopDistance;
        bool chasing = distance <= detectionRange && !inRange;

        Vector3 movement = Vector3.zero;

        // Face the player.
        if (distance > 0.01f && (chasing || inRange))
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move toward the player unless knocked back.
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            movement += knockbackVelocity;
            knockbackVelocity = Vector3.Lerp(
                knockbackVelocity,
                Vector3.zero,
                Time.deltaTime / Mathf.Max(knockbackDuration, 0.01f)
            );

            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
        else
        {
            if (chasing)
            {
                movement.x = direction.normalized.x * moveSpeed;
                movement.z = direction.normalized.z * moveSpeed;
            }

            if (animator != null)
                animator.SetFloat("Speed", chasing ? 1f : 0f);

            if (inRange && Time.time >= nextAttackTime)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                nextAttackTime = Time.time + attackCooldown;
            }
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        knockbackVelocity = direction.normalized * force;
        knockbackTimer = knockbackDuration;
    }

    public void ReceiveKnockback(Vector3 direction, bool heavyAttack)
    {
        float force = heavyAttack
            ? heavyKnockback
            : normalKnockback;

        ApplyKnockback(direction, force);
    }
}
