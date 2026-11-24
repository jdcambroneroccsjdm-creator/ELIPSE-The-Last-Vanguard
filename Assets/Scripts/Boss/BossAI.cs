
using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public BossAttackHitbox attackHitbox;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float returnRange = 15f;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float rotationSpeed = 10f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDamage = 25f;

    [Header("Hitbox Timing")]
    public float attackHitboxDelay = 0.5f;
    public float hitboxDuration = 0.35f;

    private Vector3 startPosition;
    private float attackTimer;

    private bool isDead;
    private bool isAggro;

    private Coroutine attackRoutine;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Detect player
        if (distance <= detectionRange)
            isAggro = true;

        // Stop chasing if player moves too far
        if (isAggro && distance > returnRange)
            isAggro = false;

        if (isAggro)
        {
            if (distance <= attackRange)
                Attack();
            else
                ChasePlayer();
        }
        else
        {
            ReturnToStart();
        }
    }

    // CHASE PLAYER
    void ChasePlayer()
    {
        if (attackRoutine != null)
            return;

        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

        MoveTowards(targetPosition);

        animator.SetBool("Walk", true);
    }

    // MOVE WITHOUT NAVMESH
    void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            walkSpeed * Time.deltaTime
        );
    }

    // ATTACK PLAYER
    void Attack()
    {
        animator.SetBool("Walk", false);

        FacePlayer();

        if (attackTimer <= 0f && attackRoutine == null)
        {
            animator.SetTrigger("Attack");

            attackTimer = attackCooldown;

            attackRoutine = StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(attackHitboxDelay);

        if (!isDead && attackHitbox != null)
        {
            attackHitbox.Activate(
                attackDamage,
                hitboxDuration
            );
        }

        yield return new WaitForSeconds(hitboxDuration);

        if (attackHitbox != null)
            attackHitbox.Deactivate();

        attackRoutine = null;
    }

    // FACE PLAYER
    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // RETURN TO ORIGINAL POSITION
    void ReturnToStart()
    {
        if (attackRoutine != null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            startPosition
        );

        if (distance > 0.5f)
        {
            MoveTowards(startPosition);

            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

    // DIE
    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (attackHitbox != null)
            attackHitbox.Deactivate();

        animator.SetBool("Walk", false);
        animator.SetTrigger("Die");
    }
}
