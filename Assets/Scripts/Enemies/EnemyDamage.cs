
using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackInterval = 1.5f;
    public float attackRange = 2f;

    [Header("Animation Timing")]
    public float hitDelay = 0.5f;

    [Header("Stun Settings")]
    public float stunDuration = 1.5f;

    [Header("References")]
    public Transform player;
    public Animator animator;

    private float attackTimer;
    private bool isAttacking;
    private bool isStunned;
    private Coroutine attackCoroutine;
    private Coroutine stunCoroutine;

    private void Update()
    {
        if (isStunned || player == null || animator == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && !isAttacking)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(hitDelay);

        // stun pag inaatake
        if (!isStunned && player != null &&
            Vector3.Distance(transform.position, player.position)
                <= attackRange)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health == null)
                health = player.GetComponentInParent<PlayerHealth>();

            if (health != null)
                health.TakeDamage(damage);
        }

        attackTimer = attackInterval;
        isAttacking = false;
        attackCoroutine = null;
    }

    public void StunEnemy()
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;

        // To Stop the current attack animation.
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Stunned");

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        stunCoroutine = null;
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
