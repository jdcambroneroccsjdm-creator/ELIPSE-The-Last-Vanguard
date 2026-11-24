
using UnityEngine;
using System.Collections;

public class EnemyHitFlash : MonoBehaviour
{
    [Header("Hit Flash Settings")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.12f;

    [Header("Hit Audio Settings")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float hitVolume = 1f;

    private AudioSource audioSource;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private MaterialPropertyBlock[] originalBlocks;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        originalBlocks = new MaterialPropertyBlock[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalBlocks[i] = new MaterialPropertyBlock();
            renderers[i].GetPropertyBlock(originalBlocks[i]);
        }
    }

    public void Flash()
    {
        // Play hit sound
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, hitVolume);
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            RestoreOriginal();
        }

        flashCoroutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            rend.GetPropertyBlock(propertyBlock);

            foreach (Material mat in rend.sharedMaterials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_BaseColor"))
                    propertyBlock.SetColor("_BaseColor", flashColor);

                if (mat.HasProperty("_Color"))
                    propertyBlock.SetColor("_Color", flashColor);
            }

            rend.SetPropertyBlock(propertyBlock);
        }

        yield return new WaitForSeconds(flashDuration);

        RestoreOriginal();
        flashCoroutine = null;
    }

    private void RestoreOriginal()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].SetPropertyBlock(originalBlocks[i]);
        }
    }
}
