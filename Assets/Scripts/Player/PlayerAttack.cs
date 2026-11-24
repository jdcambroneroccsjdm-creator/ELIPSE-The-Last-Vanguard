
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Normal Attack Combo")]
    public float comboWindow = 0.8f;
    public float normalAttackLock = 0.6f;

    [Header("Heavy Attack")]
    public float heavyAttackLock = 1f;

    [Header("Hitbox Settings")]
    public AttackHitbox attackHitbox;
    public float normalDamage = 20f;
    public float heavyDamage = 40f;

    public float normalHitboxDuration = 0.35f;
    public float heavyHitboxDuration = 0.45f;

    [Header("Hitbox Delay")]
    public float normalHitboxDelay = 0.15f;
    public float heavyHitboxDelay = 0.25f;

    [Header("Attack Sound")]
    public AudioSource attackAudioSource;
    public AudioClip normalAttackSound;
    public AudioClip heavyAttackSound;

    [Range(0f, 1f)]
    public float attackVolume = 1f;

    public float normalSoundDelay = 0.2f;
    public float heavySoundDelay = 0.4f;

    [Header("Animation")]
    public Animator animator;

    private float attackTimer;
    private float comboTimer;

    private int comboStep = 0;

    private Coroutine hitboxRoutine;

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (attackHitbox == null)
            attackHitbox = GetComponentInChildren<AttackHitbox>();

        if (attackAudioSource == null)
            attackAudioSource = GetComponent<AudioSource>();

        if (attackAudioSource == null)
            attackAudioSource = gameObject.AddComponent<AudioSource>();

        attackAudioSource.playOnAwake = false;
        attackAudioSource.spatialBlend = 0f;
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
                IsAttacking = false;
        }

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
                comboStep = 0;
        }
    }

    public void HandleAttackInput()
    {
        if (IsAttacking)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            NormalAttack();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HeavyAttack();
        }
    }

    private void NormalAttack()
    {
        if (comboTimer <= 0f)
            comboStep = 0;

        comboStep++;

        if (comboStep > 2)
            comboStep = 1;

        ResetAttackTriggers();

        // Start animation immediately
        if (animator != null)
        {
            if (comboStep == 1)
                animator.SetTrigger("NormalAttack1");
            else
                animator.SetTrigger("NormalAttack2");
        }

        // Play sound independently
        PlayAttackSound(normalAttackSound, normalSoundDelay);

        // Start hitbox independently
        StartHitbox(
            normalDamage,
            normalHitboxDelay,
            normalHitboxDuration
        );

        comboTimer = comboWindow;
        attackTimer = normalAttackLock;
        IsAttacking = true;
    }

    private void HeavyAttack()
    {
        ResetAttackTriggers();

        // Start animation immediately
        if (animator != null)
            animator.SetTrigger("HeavyAttack");

        // Play sound independently
        PlayAttackSound(heavyAttackSound, heavySoundDelay);

        // Start hitbox independently
        StartHitbox(
            heavyDamage,
            heavyHitboxDelay,
            heavyHitboxDuration
        );

        comboStep = 0;
        comboTimer = 0f;

        attackTimer = heavyAttackLock;
        IsAttacking = true;
    }

    private void PlayAttackSound(AudioClip clip, float delay)
    {
        if (clip == null || attackAudioSource == null)
            return;

        // Independent coroutine for every sound
        StartCoroutine(DelayedSound(clip, delay));
    }

    private IEnumerator DelayedSound(AudioClip clip, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (clip != null && attackAudioSource != null)
        {
            attackAudioSource.PlayOneShot(clip, attackVolume);
        }
    }

    private void StartHitbox(float damage, float delay, float duration)
    {
        if (attackHitbox == null)
            return;

        if (hitboxRoutine != null)
            StopCoroutine(hitboxRoutine);

        attackHitbox.CancelAttack();

        hitboxRoutine = StartCoroutine(
            DelayedHitbox(damage, delay, duration)
        );
    }

    private IEnumerator DelayedHitbox(
        float damage,
        float delay,
        float duration)
    {
        yield return new WaitForSeconds(delay);

        if (attackHitbox != null && IsAttacking)
        {
            attackHitbox.PerformAttack(damage, duration);
        }

        hitboxRoutine = null;
    }

    private void ResetAttackTriggers()
    {
        if (animator == null)
            return;

        animator.ResetTrigger("NormalAttack1");
        animator.ResetTrigger("NormalAttack2");
        animator.ResetTrigger("HeavyAttack");
    }

    public void ResetForRespawn()
    {
        IsAttacking = false;

        attackTimer = 0f;
        comboTimer = 0f;
        comboStep = 0;

        if (hitboxRoutine != null)
        {
            StopCoroutine(hitboxRoutine);
            hitboxRoutine = null;
        }

        if (attackHitbox != null)
            attackHitbox.CancelAttack();

        ResetAttackTriggers();
    }
}
