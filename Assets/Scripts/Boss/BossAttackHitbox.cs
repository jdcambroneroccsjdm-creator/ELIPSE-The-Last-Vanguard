
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public BoxCollider hitboxCollider;

    private float currentDamage;
    private bool isActive = false;

    private readonly HashSet<PlayerHealth> hitPlayers =
        new HashSet<PlayerHealth>();

    private Coroutine hitboxRoutine;

    void Awake()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<BoxCollider>();

        if (hitboxCollider != null)
        {
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
        }
    }

    public void Activate(float damage, float duration)
    {
        if (hitboxRoutine != null)
            StopCoroutine(hitboxRoutine);

        currentDamage = damage;
        hitPlayers.Clear();

        hitboxRoutine = StartCoroutine(EnableHitbox(duration));
    }

    IEnumerator EnableHitbox(float duration)
    {
        isActive = true;

        hitboxCollider.enabled = true;

        yield return new WaitForSeconds(duration);

        Deactivate();
    }

    void OnTriggerStay(Collider other)
    {
        if (!isActive)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        // Prevent multiple hits during one attack
        if (hitPlayers.Contains(playerHealth))
            return;

        hitPlayers.Add(playerHealth);

        playerHealth.TakeDamage(currentDamage);
    }

    public void Deactivate()
    {
        isActive = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;

        if (hitboxRoutine != null)
        {
            StopCoroutine(hitboxRoutine);
            hitboxRoutine = null;
        }
    }
}
