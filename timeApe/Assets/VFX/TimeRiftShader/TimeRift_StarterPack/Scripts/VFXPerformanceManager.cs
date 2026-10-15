using UnityEngine;

public class VFXPerformanceManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("VFX Particle Systems")]
    public ParticleSystem[] particleEffects;

    [Header("Distance")]
    public float fullQualityDistance = 10f;
    public float reducedQualityDistance = 25f;
    public float disableDistance = 50f;

    private int[] originalMaxParticles;
    private float[] originalEmissionRates;

    private Renderer[] renderers;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Remember original particle settings
        if (particleEffects != null)
        {
            originalMaxParticles =
                new int[particleEffects.Length];

            originalEmissionRates =
                new float[particleEffects.Length];

            for (int i = 0; i < particleEffects.Length; i++)
            {
                if (particleEffects[i] == null)
                    continue;

                originalMaxParticles[i] =
                    particleEffects[i].main.maxParticles;

                originalEmissionRates[i] =
                    particleEffects[i]
                    .emission
                    .rateOverTime
                    .constant;
            }
        }

        // Get renderers on this object and its children
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (playerCamera == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                playerCamera.transform.position
            );

        UpdateParticles(distance);
        UpdateRenderers(distance);
    }

    private void UpdateParticles(float distance)
    {
        if (particleEffects == null)
            return;

        for (int i = 0; i < particleEffects.Length; i++)
        {
            if (particleEffects[i] == null)
                continue;

            var main =
                particleEffects[i].main;

            var emission =
                particleEffects[i].emission;

            // Close = normal quality
            if (distance <= fullQualityDistance)
            {
                main.maxParticles =
                    originalMaxParticles[i];

                emission.rateOverTime =
                    originalEmissionRates[i];
            }

            // Medium distance = reduce particles
            else if (distance <= reducedQualityDistance)
            {
                main.maxParticles =
                    Mathf.Max(
                        1,
                        originalMaxParticles[i] / 2
                    );

                emission.rateOverTime =
                    originalEmissionRates[i] * 0.5f;
            }

            // Very far = minimal particles
            else
            {
                main.maxParticles =
                    Mathf.Max(
                        1,
                        originalMaxParticles[i] / 4
                    );

                emission.rateOverTime =
                    originalEmissionRates[i] * 0.25f;
            }
        }
    }

    private void UpdateRenderers(float distance)
    {
        if (renderers == null)
            return;

        bool shouldRender =
            distance <= disableDistance;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            renderer.forceRenderingOff =
                !shouldRender;
        }
    }
}
