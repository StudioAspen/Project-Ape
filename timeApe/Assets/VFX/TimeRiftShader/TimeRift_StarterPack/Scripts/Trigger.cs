using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour
{
    [Header("Portal")]
    public Transform portalVisual;

    [Header("VFX")]
    public GameObject shards;
    public GameObject stars;

    [Header("Rift Distort")]
    public Transform riftDistort;
    public float riftDistortMoveDistance = 0.03f;

    [Header("Sphere")]
    public GameObject sphere;
    public float sphereGrowTime = 0.8f;

    [Header("Shake")]
    public Shake shakeScript;

    [Header("Movement")]
    public float moveDistance = 0.1f;
    public float moveSpeed = 20f;
    public float moveTime = 0.8f;

    [Header("Star Fade")]
    public float starFadeTime = 0.3f;

    [Header("Shard Fade")]
    public float shardFadeTime = 0.3f;

    private Vector3 originalPosition;
    private Vector3 originalRiftDistortPosition;
    private Vector3 originalSphereScale;

    private bool moving = false;

    private ParticleSystem starParticles;
    private ParticleSystemRenderer starRenderer;
    private Material starMaterial;
    private Color originalStarMaterialColor;

    private ParticleSystem shardParticles;
    private ParticleSystemRenderer shardRenderer;
    private Material shardMaterial;
    private Color originalShardMaterialColor;


    private void Start()
    {
        // Remember Rift's starting position
        if (portalVisual != null)
        {
            originalPosition = portalVisual.localPosition;
        }

        // Remember Rift Distort's starting position
        if (riftDistort != null)
        {
            originalRiftDistortPosition =
                riftDistort.localPosition;
        }

        // Remember Sphere's normal size
        if (sphere != null)
        {
            originalSphereScale =
                sphere.transform.localScale;
        }


        // =========================
        // GET STARS
        // =========================

        if (stars != null)
        {
            starParticles =
                stars.GetComponent<ParticleSystem>();

            starRenderer =
                stars.GetComponent<ParticleSystemRenderer>();

            if (starRenderer != null)
            {
                starMaterial =
                    starRenderer.material;

                if (starMaterial.HasProperty("_BaseColor"))
                {
                    originalStarMaterialColor =
                        starMaterial.GetColor("_BaseColor");
                }
                else if (starMaterial.HasProperty("_Color"))
                {
                    originalStarMaterialColor =
                        starMaterial.GetColor("_Color");
                }
            }
        }


        // =========================
        // GET SHARDS
        // =========================

        if (shards != null)
        {
            shardParticles =
                shards.GetComponent<ParticleSystem>();

            shardRenderer =
                shards.GetComponent<ParticleSystemRenderer>();

            if (shardRenderer != null)
            {
                shardMaterial =
                    shardRenderer.material;

                if (shardMaterial.HasProperty("_BaseColor"))
                {
                    originalShardMaterialColor =
                        shardMaterial.GetColor("_BaseColor");
                }
                else if (shardMaterial.HasProperty("_Color"))
                {
                    originalShardMaterialColor =
                        shardMaterial.GetColor("_Color");
                }
            }
        }


        // =========================
        // START EVERYTHING OFF
        // =========================

        SetVFX(false);

        SetStarAlpha(0f);

        SetShardAlpha(1f);


        if (sphere != null)
        {
            sphere.SetActive(false);

            sphere.transform.localScale =
                originalSphereScale;
        }


        if (riftDistort != null)
        {
            riftDistort.localPosition =
                originalRiftDistortPosition;
        }


        if (shakeScript != null)
        {
            shakeScript.enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!moving)
        {
            StartCoroutine(EnterPortal());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!moving)
        {
            StartCoroutine(ExitPortal());
        }
    }


    // =========================================================
    // ENTER PORTAL
    // =========================================================

    private IEnumerator EnterPortal()
    {
        moving = true;


        // Everything stays OFF
        SetVFX(false);


        // Shake OFF
        if (shakeScript != null)
        {
            shakeScript.enabled = false;
        }


        // Sphere OFF
        if (sphere != null)
        {
            sphere.SetActive(false);

            sphere.transform.localScale =
                originalSphereScale;
        }


        // Move Rift + Rift Distort
        // Stars fade in during movement
        yield return StartCoroutine(
            MoveRift(true)
        );


        // Return Rift
        if (portalVisual != null)
        {
            portalVisual.localPosition =
                originalPosition;
        }


        // Return Rift Distort
        if (riftDistort != null)
        {
            riftDistort.localPosition =
                originalRiftDistortPosition;
        }


        // =========================
        // SHARDS ON
        // =========================

        if (shards != null)
        {
            shards.SetActive(true);

            SetShardAlpha(1f);

            if (shardParticles != null)
            {
                shardParticles.Clear();
                shardParticles.Play();
            }
        }


        // =========================
        // SHAKE ON
        // =========================

        if (shakeScript != null)
        {
            shakeScript.enabled = true;
        }


        // =========================
        // SPHERE APPEARS
        // =========================

        yield return StartCoroutine(
            GrowSphere()
        );


        moving = false;
    }


    // =========================================================
    // EXIT PORTAL
    // =========================================================

    private IEnumerator ExitPortal()
    {
        moving = true;


        // Shake OFF FIRST
        if (shakeScript != null)
        {
            shakeScript.enabled = false;
        }


        // Move Rift + Rift Distort
        // Stars and Shards fade out
        yield return StartCoroutine(
            MoveRift(false)
        );


        // Return Rift
        if (portalVisual != null)
        {
            portalVisual.localPosition =
                originalPosition;
        }


        // Return Rift Distort
        if (riftDistort != null)
        {
            riftDistort.localPosition =
                originalRiftDistortPosition;
        }


        // =========================
        // SPHERE SHRINKS
        // =========================

        yield return StartCoroutine(
            ShrinkSphere()
        );


        // =========================
        // TURN EVERYTHING OFF
        // =========================

        SetVFX(false);


        if (starParticles != null)
        {
            starParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }


        if (shardParticles != null)
        {
            shardParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }


        SetStarAlpha(0f);

        SetShardAlpha(1f);


        // Make sure Sphere is OFF
        if (sphere != null)
        {
            sphere.SetActive(false);

            sphere.transform.localScale =
                originalSphereScale;
        }


        moving = false;
    }


    // =========================================================
    // RIFT MOVEMENT
    // =========================================================

    private IEnumerator MoveRift(bool entering)
    {
        float timer = 0f;

        bool starsStarted = false;
        bool shardsStarted = false;


        while (timer < moveTime)
        {
            timer += Time.deltaTime;

            float progress =
                timer / moveTime;


            // =========================
            // RIFT LEFT / RIGHT ON Z
            // =========================

            float z =
                Mathf.Sin(timer * moveSpeed)
                * moveDistance;


            if (portalVisual != null)
            {
                Vector3 position =
                    originalPosition;

                position.z += z;

                portalVisual.localPosition =
                    position;
            }


            // =========================
            // RIFT DISTORT
            // SAME SPEED
            // ON Y AXIS
            // =========================

            float distortY =
                Mathf.Sin(timer * moveSpeed)
                * riftDistortMoveDistance;


            if (riftDistort != null)
            {
                Vector3 distortPosition =
                    originalRiftDistortPosition;

                distortPosition.y += distortY;

                riftDistort.localPosition =
                    distortPosition;
            }


            // =========================
            // STARS
            // =========================

            if (!starsStarted &&
                timer >= moveTime * 0.5f)
            {
                starsStarted = true;


                if (entering)
                {
                    yield return StartCoroutine(
                        FadeStarsIn()
                    );
                }
                else
                {
                    yield return StartCoroutine(
                        FadeStarsOut()
                    );
                }
            }


            // =========================
            // SHARDS ONLY FADE OUT
            // =========================

            if (!entering &&
                !shardsStarted &&
                timer >= moveTime * 0.5f)
            {
                shardsStarted = true;

                yield return StartCoroutine(
                    FadeShardsOut()
                );
            }


            yield return null;
        }


        // Return Rift
        if (portalVisual != null)
        {
            portalVisual.localPosition =
                originalPosition;
        }


        // Return Rift Distort
        if (riftDistort != null)
        {
            riftDistort.localPosition =
                originalRiftDistortPosition;
        }
    }


    // =========================================================
    // STARS FADE IN
    // =========================================================

    private IEnumerator FadeStarsIn()
    {
        if (stars == null)
            yield break;


        stars.SetActive(true);


        if (starParticles != null)
        {
            starParticles.Clear();
            starParticles.Play();
        }


        SetStarAlpha(0f);


        float timer = 0f;


        while (timer < starFadeTime)
        {
            timer += Time.deltaTime;


            float alpha =
                Mathf.Clamp01(
                    timer / starFadeTime
                );


            SetStarAlpha(alpha);


            yield return null;
        }


        SetStarAlpha(1f);
    }


    // =========================================================
    // STARS FADE OUT
    // =========================================================

    private IEnumerator FadeStarsOut()
    {
        if (stars == null)
            yield break;


        stars.SetActive(true);


        float timer = 0f;


        while (timer < starFadeTime)
        {
            timer += Time.deltaTime;


            float alpha =
                1f -
                Mathf.Clamp01(
                    timer / starFadeTime
                );


            SetStarAlpha(alpha);


            yield return null;
        }


        SetStarAlpha(0f);


        if (starParticles != null)
        {
            starParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }


        stars.SetActive(false);
    }


    // =========================================================
    // SHARDS FADE OUT
    // =========================================================

    private IEnumerator FadeShardsOut()
    {
        if (shards == null)
            yield break;


        shards.SetActive(true);


        // Make sure Shards are running
        if (shardParticles != null)
        {
            shardParticles.Play();
        }


        SetShardAlpha(1f);


        float timer = 0f;


        while (timer < shardFadeTime)
        {
            timer += Time.deltaTime;


            float alpha =
                1f -
                Mathf.Clamp01(
                    timer / shardFadeTime
                );


            SetShardAlpha(alpha);


            yield return null;
        }


        SetShardAlpha(0f);


        if (shardParticles != null)
        {
            shardParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }


        shards.SetActive(false);
    }


    // =========================================================
    // SPHERE GROW
    // =========================================================

    private IEnumerator GrowSphere()
    {
        if (sphere == null)
            yield break;


        sphere.SetActive(true);


        // Start small
        sphere.transform.localScale =
            Vector3.zero;


        float timer = 0f;


        while (timer < sphereGrowTime)
        {
            timer += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    timer / sphereGrowTime
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            sphere.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    originalSphereScale,
                    smoothProgress
                );


            yield return null;
        }


        sphere.transform.localScale =
            originalSphereScale;
    }


    // =========================================================
    // SPHERE SHRINK
    // =========================================================

    private IEnumerator ShrinkSphere()
    {
        if (sphere == null)
            yield break;


        sphere.SetActive(true);


        // Start at normal size
        sphere.transform.localScale =
            originalSphereScale;


        float timer = 0f;


        while (timer < sphereGrowTime)
        {
            timer += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    timer / sphereGrowTime
                );


            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            sphere.transform.localScale =
                Vector3.Lerp(
                    originalSphereScale,
                    Vector3.zero,
                    smoothProgress
                );


            yield return null;
        }


        // Completely small
        sphere.transform.localScale =
            Vector3.zero;


        sphere.SetActive(false);
    }


    // =========================================================
    // STAR ALPHA
    // =========================================================

    private void SetStarAlpha(float alpha)
    {
        if (starMaterial == null)
            return;


        Color color =
            originalStarMaterialColor;

        color.a = alpha;


        if (starMaterial.HasProperty("_BaseColor"))
        {
            starMaterial.SetColor(
                "_BaseColor",
                color
            );
        }
        else if (starMaterial.HasProperty("_Color"))
        {
            starMaterial.SetColor(
                "_Color",
                color
            );
        }
    }


    // =========================================================
    // SHARD ALPHA
    // =========================================================

    private void SetShardAlpha(float alpha)
    {
        if (shardMaterial == null)
            return;


        Color color =
            originalShardMaterialColor;

        color.a = alpha;


        if (shardMaterial.HasProperty("_BaseColor"))
        {
            shardMaterial.SetColor(
                "_BaseColor",
                color
            );
        }
        else if (shardMaterial.HasProperty("_Color"))
        {
            shardMaterial.SetColor(
                "_Color",
                color
            );
        }
    }


    // =========================================================
    // VFX ON / OFF
    // =========================================================

    private void SetVFX(bool active)
    {
        // SHARDS
        if (shards != null)
        {
            shards.SetActive(active);


            if (shardParticles != null)
            {
                if (active)
                {
                    shardParticles.Clear();
                    shardParticles.Play();
                }
                else
                {
                    shardParticles.Stop(
                        true,
                        ParticleSystemStopBehavior.StopEmittingAndClear
                    );
                }
            }
        }


        // STARS
        if (stars != null)
        {
            stars.SetActive(active);


            if (starParticles != null)
            {
                if (active)
                {
                    starParticles.Clear();
                    starParticles.Play();
                }
                else
                {
                    starParticles.Stop(
                        true,
                        ParticleSystemStopBehavior.StopEmittingAndClear
                    );
                }
            }
        }
    }
}