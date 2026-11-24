
using UnityEngine;

public class TargetLockOn : MonoBehaviour
{
    [Header("Target Settings")]
    public float targetSearchRange = 15f;

    [Header("Lock On Distance")]
    public float maxLockOnDistance = 20f;

    [Header("Target Status")]
    public Transform CurrentTarget { get; private set; }

    private void Update()
    {
        // Check current target
        if (CurrentTarget != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                CurrentTarget.position
            );

            // Release target if too far away or dead
            if (distance > maxLockOnDistance || !IsTargetAlive(CurrentTarget))
            {
                CurrentTarget = null;
            }
            else
            {
                return;
            }
        }

        FindTarget();
    }

    private void FindTarget()
    {
        float closestDistance = targetSearchRange;
        Transform closestTarget = null;

        // Find normal enemies
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy == null || !IsTargetAlive(enemy.transform))
                continue;

            CheckTarget(
                enemy.transform,
                ref closestDistance,
                ref closestTarget
            );
        }

        // Find bosses
        BossAI[] bosses = FindObjectsOfType<BossAI>();

        foreach (BossAI boss in bosses)
        {
            if (boss == null || !IsTargetAlive(boss.transform))
                continue;

            CheckTarget(
                boss.transform,
                ref closestDistance,
                ref closestTarget
            );
        }

        CurrentTarget = closestTarget;
    }

    private void CheckTarget(
        Transform target,
        ref float closestDistance,
        ref Transform closestTarget)
    {
        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distance <= closestDistance)
        {
            closestDistance = distance;
            closestTarget = target;
        }
    }

    private bool IsTargetAlive(Transform target)
    {
        // Check normal enemy health
        HealthBruh enemyHealth =
            target.GetComponentInChildren<HealthBruh>();

        if (enemyHealth == null)
            enemyHealth = target.GetComponent<HealthBruh>();

        if (enemyHealth != null)
            return enemyHealth.currentHealth > 0f;

        // Check boss health component
        BossHealth bossHealth =
            target.GetComponentInChildren<BossHealth>();

        if (bossHealth == null)
            bossHealth = target.GetComponent<BossHealth>();

        if (bossHealth != null)
            return true;

        return false;
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }
}
