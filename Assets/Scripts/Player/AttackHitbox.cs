
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public Collider hitboxCollider;

    [Header("Damage")]
    public float normalDamage = 20f;
    public float heavyDamage = 40f;

    private float currentDamage;

    private Coroutine attackRoutine;

    // Prevent multiple hits on the same target per attack
    private readonly HashSet<HealthBruh> hitEnemies =
        new HashSet<HealthBruh>();

    private readonly HashSet<BossHealth> hitBosses =
        new HashSet<BossHealth>();

    private void Awake()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();

        if (hitboxCollider == null)
        {
            Debug.LogError("AttackHitbox: No Collider assigned!");
            return;
        }

        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;
    }

    public void PerformAttack(float damage, float activeTime)
    {
        if (hitboxCollider == null)
            return;

        CancelAttack();

        currentDamage = damage;

        hitEnemies.Clear();
        hitBosses.Clear();

        attackRoutine = StartCoroutine(
            EnableHitbox(activeTime)
        );

        Debug.Log("Player attack hitbox activated!");
    }

    private IEnumerator EnableHitbox(float activeTime)
    {
        hitboxCollider.enabled = true;

        yield return new WaitForSeconds(activeTime);

        hitboxCollider.enabled = false;

        attackRoutine = null;

        Debug.Log("Player attack hitbox deactivated!");
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTarget(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckTarget(other);
    }

    private void CheckTarget(Collider other)
    {
        // Check boss first
        BossHealth boss = FindComponentInParents<BossHealth>(
            other.transform
        );

        if (boss != null)
        {
            if (hitBosses.Contains(boss))
                return;

            hitBosses.Add(boss);

            Debug.Log("PLAYER HIT BOSS! Damage: " + currentDamage);

            boss.TakeDamage(currentDamage);
            return;
        }

        // Check normal enemy
        HealthBruh enemy = FindComponentInParents<HealthBruh>(
            other.transform
        );

        if (enemy != null)
        {
            if (hitEnemies.Contains(enemy))
                return;

            if (enemy.currentHealth <= 0f)
                return;

            hitEnemies.Add(enemy);

            Debug.Log("PLAYER HIT ENEMY! Damage: " + currentDamage);

            enemy.TakeDamage(currentDamage);
        }
    }

    private T FindComponentInParents<T>(Transform target)
        where T : Component
    {
        Transform current = target;

        while (current != null)
        {
            T component = current.GetComponent<T>();

            if (component != null)
                return component;

            current = current.parent;
        }

        return null;
    }

    public void CancelAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;

        hitEnemies.Clear();
        hitBosses.Clear();
    }
}
