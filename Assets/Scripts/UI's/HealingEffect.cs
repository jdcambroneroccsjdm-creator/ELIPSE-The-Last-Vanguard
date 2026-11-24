
using UnityEngine;

public class PotionEffect : MonoBehaviour
{
    public ParticleSystem healingEffect;

    private void Awake()
    {
        if (healingEffect != null)
        {
            healingEffect.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void PlayHealingEffect()
    {
        if (healingEffect != null)
        {
            healingEffect.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            healingEffect.Play();
        }
    }
}
