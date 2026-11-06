using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Camera Switch")]
    [Tooltip("The player's normal first-person camera.")]
    public Camera firstPersonCamera;
    [Tooltip("The separate camera used for the cutscene animation.")]
    public Camera animationCamera;

    [Tooltip("Add the scripts that let the player move/look, such as your movement and mouse-look scripts.")]
    public MonoBehaviour[] playerMovementScripts;

    [Tooltip("Optional: the player's CharacterController, if used.")]
    public CharacterController playerCharacterController;

    [Header("Portal Stretch Animation")]
    [Tooltip("Animator on the portal GameObject that contains both stretch animations.")]
    public Animator portalAnimator;
    public string portalStretchState = "Portal_Stretch";
    public string portalStretchUndoState = "Portal_Stretch_Undo";

    [Header("Portal Prompt")]
    [Tooltip("Assign the GameObject containing the Press Y TextMeshPro text, or its parent.")]
    public GameObject pressYText;

    [Header("Camera Animation")]
    public Animator cameraAnimator;
    public string cameraAnimationState = "Camera for Animation";
    [Tooltip("Seconds after pressing Y before the camera Animator is disabled and the camera stays at its final position.")]
    public float animationCleanupDelay = 7f;

    [Header("K Key Sequence")]
    [Tooltip("These GameObjects are permanently hidden when K is pressed.")]
    public GameObject[] objectsToHideWhenKPressed;

    [Tooltip("The original animation set: Cone and Box_1 through Box_7.")]
    public GameObject[] animatedObjectsToActivateWhenKPressed = new GameObject[8];

    [Tooltip("The reverse animation set: Cone_Reverse and Box_1_Reverse through Box_7_Reverse.")]
    public GameObject[] reverseAnimatedObjectsToActivateWhenKPressed = new GameObject[8];

    [Header("K Key Skybox")]
    [Tooltip("The daytime skybox. This is restored when the reverse animations play.")]
    public Material daySkyboxMaterial;

    [Tooltip("The nighttime skybox. This is applied when the original animations play.")]
    public Material nightSkyboxMaterial;

    [Header("Nighttime-Only Objects")]
    [Tooltip("These three GameObjects are visible at night and hidden during the day.")]
    public GameObject[] nighttimeOnlyObjects = new GameObject[3];

    [Header("Space Key - Galaxy Fade and Area Transition")]
    [Tooltip("Animator that contains your existing FadeIn_Galaxy animation.")]
    public Animator galaxyFadeAnimator;

    [Tooltip("Exact Animator state name for the galaxy fade-in animation.")]
    public string galaxyFadeStateName = "FadeIn_Galaxy";

    [Tooltip("Fallback fade duration if the Animator state length cannot be read.")]
    public float galaxyFadeDuration = 2f;

    [Tooltip("The player's root Transform. If left empty, the CharacterController Transform is used.")]
    public Transform playerRoot;

    [Tooltip("Where the player should appear after the fade when the daytime environment is selected.")]
    public Transform daytimeDestination;

    [Tooltip("Where the player should appear after the fade when the nighttime environment is selected.")]
    public Transform nighttimeDestination;

    [Tooltip("Minimum time K stays locked if an Animator has no readable animation length.")]
    public float kSequenceMinimumLockTime = 0.5f;

    [Tooltip("A small extra delay after the longest animation finishes before K can be pressed again.")]
    public float kSequenceExtraLockTime = 0.15f;

    private Coroutine animationCleanupCoroutine;
    private bool[] movementWasEnabled;
    private bool characterControllerWasEnabled;
    private bool sequenceRunning;
    private bool playerInsideTrigger;
    private bool cutsceneStarted;
    private bool cameraAnimationFinished;
    private bool kSequenceRunning;
    private bool originalObjectsHidden;
    private bool playFirstKSetNext = true;
    private bool galaxyTransitionStarted;

    // Tracks every collider belonging to the player so one collider leaving
    // does not hide the prompt while another player collider is still inside.
    private readonly HashSet<Collider> playerCollidersInside = new HashSet<Collider>();

    private TMP_Text[] promptTextComponents;
    private CanvasGroup[] promptChildCanvasGroups;
    private CanvasGroup[] promptParentCanvasGroups;

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

        // =========================
        // GET STARS
        // =========================

        if (stars != null)
        {
            // Find the Particle System even if it is on a child object.
            starParticles =
                stars.GetComponentInChildren<ParticleSystem>(true);

            // Keep the stars emitting instead of letting a one-shot particle
            // play finish while the player is using the portal/cutscene.
            if (starParticles != null)
            {
                var starMain = starParticles.main;
                starMain.loop = true;
            }

            starRenderer =
                stars.GetComponentInChildren<ParticleSystemRenderer>(true);

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
            // Find the Particle System even if it is on a child object.
            shardParticles =
                shards.GetComponentInChildren<ParticleSystem>(true);

            shardRenderer =
                shards.GetComponentInChildren<ParticleSystemRenderer>(true);

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

        if (sphere != null)
            sphere.SetActive(false);

        SetStarAlpha(0f);

        SetShardAlpha(1f);

        // Keep the portal stretch animation from playing automatically at startup.
        if (portalAnimator != null)
        {
            portalAnimator.enabled = false;
            portalAnimator.Rebind();
            portalAnimator.Update(0f);
        }

        // Stop both animations from playing automatically when Play is pressed.
        // They will be reset and started only when the player enters the trigger.
        if (cameraAnimator != null)
        {
            cameraAnimator.enabled = false;
            cameraAnimator.Rebind();
            cameraAnimator.Update(0f);
        }

        // Remember which movement scripts were enabled before the sequence.
        if (playerMovementScripts != null)
        {
            movementWasEnabled = new bool[playerMovementScripts.Length];
            for (int i = 0; i < playerMovementScripts.Length; i++)
            {
                movementWasEnabled[i] = playerMovementScripts[i] != null && playerMovementScripts[i].enabled;
            }
        }

        if (playerCharacterController != null)
            characterControllerWasEnabled = playerCharacterController.enabled;

        // The normal player camera starts on; the animation camera starts off.
        if (firstPersonCamera != null)
            firstPersonCamera.enabled = true;
        if (animationCamera != null)
        {
            animationCamera.enabled = false;
            animationCamera.gameObject.SetActive(false);
        }

        // Cache the actual TextMeshPro components and any Canvas Groups
        // controlling the prompt's visibility.
        CachePromptComponents();
        SetPromptVisible(false);

        // Both animation sets wait until the player presses K.
        SetAnimationObjectsActive(animatedObjectsToActivateWhenKPressed, false);
        SetAnimationObjectsActive(reverseAnimatedObjectsToActivateWhenKPressed, false);

        // The nighttime-only objects start hidden because the initial state is daytime.
        SetNighttimeObjectsActive(false);

        // Keep the galaxy fade from playing automatically when Play is pressed.
        if (galaxyFadeAnimator != null)
        {
            galaxyFadeAnimator.enabled = false;
            galaxyFadeAnimator.Rebind();
            galaxyFadeAnimator.Update(0f);
        }

        // Start with the daytime skybox, if one has been assigned.
        SetSkybox(daySkyboxMaterial);

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



    private void OnDisable()
    {
        playerCollidersInside.Clear();
        playerInsideTrigger = false;

        SetPromptVisible(false);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Application.isFocused)
            return;

        // The player can begin the cutscene only while inside the portal,
        // after the portal's entry effects have finished.
        if (playerInsideTrigger && !moving && !sequenceRunning &&
            !cutsceneStarted && Keyboard.current.yKey.wasPressedThisFrame)
        {
            StartCoroutine(BeginCameraSequence());
        }

        // K becomes available only after the camera animation has stopped
        // and the animation camera is holding its final position.
        if (cameraAnimationFinished && !kSequenceRunning && !galaxyTransitionStarted &&
            Keyboard.current.kKey.wasPressedThisFrame)
        {
            StartCoroutine(PlayKKeySequence());
        }

        // Space confirms the currently selected environment and begins the final transition.
        // It cannot interrupt a K animation set or start the fade more than once.
        if (cameraAnimationFinished && !kSequenceRunning && !galaxyTransitionStarted &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(FinishGalaxyTransition());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        // Ignore duplicate enter events from the same collider.
        if (!playerCollidersInside.Add(other))
            return;

        // Another player collider is already inside, so keep the current state.
        if (playerCollidersInside.Count > 1)
            return;

        playerInsideTrigger = true;

        // Play the portal stretch animation as soon as the player enters.
        PlayPortalAnimation(portalStretchState);

        // Keep the prompt visible for the entire time the player is inside.
        // Once Y has been pressed, the prompt stays hidden for the cutscene.
        if (!cutsceneStarted)
            SetPromptVisible(true);

        // Do not restart the entry effects if the cutscene has already begun.
        // If an exit animation is currently running, it will restart the entry
        // effects after it finishes.
        if (!moving && !sequenceRunning && !cutsceneStarted)
            StartCoroutine(EnterPortal());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        // Only treat this as a real exit when the last player collider leaves.
        if (!playerCollidersInside.Remove(other))
            return;

        if (playerCollidersInside.Count > 0)
            return;

        playerInsideTrigger = false;

        // Play the portal's return animation as soon as the player fully leaves.
        PlayPortalAnimation(portalStretchUndoState);

        SetPromptVisible(false);

        // Do not cancel the cutscene once the player has pressed Y.
        // If an entry animation is running, it will start the exit effects
        // after the entry animation finishes.
        if (!moving && !sequenceRunning)
            StartCoroutine(ExitPortal());
    }

    private void CachePromptComponents()
    {
        if (pressYText == null)
            return;

        promptTextComponents = pressYText.GetComponentsInChildren<TMP_Text>(true);
        promptChildCanvasGroups = pressYText.GetComponentsInChildren<CanvasGroup>(true);
        promptParentCanvasGroups = pressYText.GetComponentsInParent<CanvasGroup>(true);
    }

    private void SetPromptVisible(bool visible)
    {
        if (pressYText == null)
            return;

        pressYText.SetActive(visible);

        if (!visible)
            return;

        // Refresh the references in case the prompt hierarchy changed.
        CachePromptComponents();

        // A Canvas Group with alpha 0 can hide text even when its GameObject
        // and TextMeshPro component are active.
        if (promptChildCanvasGroups != null)
        {
            foreach (CanvasGroup group in promptChildCanvasGroups)
                RestoreCanvasGroup(group);
        }

        if (promptParentCanvasGroups != null)
        {
            foreach (CanvasGroup group in promptParentCanvasGroups)
                RestoreCanvasGroup(group);
        }

        // Explicitly enable and restore the visibility of the actual TMP text.
        if (promptTextComponents != null)
        {
            foreach (TMP_Text prompt in promptTextComponents)
            {
                if (prompt == null)
                    continue;

                prompt.enabled = true;

                Color textColor = prompt.color;
                textColor.a = 1f;
                prompt.color = textColor;

                prompt.canvasRenderer.SetAlpha(1f);
                prompt.ForceMeshUpdate();
            }
        }
    }


    private void RestoreCanvasGroup(CanvasGroup group)
    {
        if (group == null)
            return;

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }


    private bool IsPlayerCollider(Collider other)
    {
        // Player colliders are sometimes placed on child objects that do not
        // have the Player tag themselves. Check the parent objects as well.
        Transform current = other.transform;

        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;

            current = current.parent;
        }

        return false;
    }


    private void PlayPortalAnimation(string stateName)
    {
        if (portalAnimator == null || string.IsNullOrEmpty(stateName))
            return;

        portalAnimator.enabled = true;
        portalAnimator.Play(stateName, 0, 0f);
        portalAnimator.Update(0f);
    }


    // =========================================================
    // ENTER PORTAL
    // =========================================================

    private IEnumerator EnterPortal()
    {
        moving = true;

        // The player can still move and use the normal camera at this point.
        // Only the portal effects play when the trigger is approached.
        SetVFX(false);

        if (shakeScript != null)
            shakeScript.enabled = false;

        // Move Rift + Rift Distort and fade the stars in.
        yield return StartCoroutine(MoveRift(true));

        if (portalVisual != null)
            portalVisual.localPosition = originalPosition;

        if (riftDistort != null)
            riftDistort.localPosition = originalRiftDistortPosition;

        // Turn on the shards.
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

        if (shakeScript != null)
            shakeScript.enabled = true;

        moving = false;

        // If the player left while the entry effects were playing,
        // immediately play the exit effects.
        if (!playerInsideTrigger && !sequenceRunning)
            StartCoroutine(ExitPortal());
    }


    // The camera cutscene begins only after the player presses Y.
    private IEnumerator BeginCameraSequence()
    {
        if (sequenceRunning)
            yield break;

        sequenceRunning = true;
        cutsceneStarted = true;

        // Hide the prompt and lock player movement for the cutscene.
        SetPromptVisible(false);

        // Restart both portal particle effects when Y is pressed so they
        // visibly play during the camera sequence as well.
        PlayCutsceneVFX();

        // Turn on the sphere two seconds after Y is pressed.
        StartCoroutine(ActivateSphereAfterDelay());

        SetPlayerControl(false);

        if (firstPersonCamera != null)
            firstPersonCamera.enabled = false;

        // Turn on the cutscene camera only now.
        if (animationCamera != null)
        {
            animationCamera.gameObject.SetActive(true);
            animationCamera.enabled = true;
            Debug.Log("Animation camera enabled: " + animationCamera.name, animationCamera);
        }
        else
        {
            Debug.LogError("Animation Camera is not assigned on the Trigger component.", this);
        }

        // Reset and play the camera cutscene from the beginning.
        if (cameraAnimator != null && !string.IsNullOrEmpty(cameraAnimationState))
        {
            cameraAnimator.gameObject.SetActive(true);
            cameraAnimator.enabled = true;
            cameraAnimator.speed = 1f;
            cameraAnimator.Rebind();
            cameraAnimator.Update(0f);

            int stateHash = Animator.StringToHash(cameraAnimationState);
            cameraAnimator.Play(stateHash, 0, 0f);
            cameraAnimator.Update(0f);

            if (!cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName(cameraAnimationState))
            {
                Debug.LogWarning(
                    "Camera animation state '" + cameraAnimationState +
                    "' may not match a state in the Animator Controller. " +
                    "Check the exact state name (not just the animation clip name).",
                    cameraAnimator);
            }
        }
        else
        {
            Debug.LogError("Camera Animator or Camera Animation State is missing on the Trigger component.", this);
        }

        // Start the cleanup timer only after the cutscene begins.
        if (animationCleanupCoroutine != null)
            StopCoroutine(animationCleanupCoroutine);

        animationCleanupCoroutine = StartCoroutine(ForceAnimationObjectsOffAfterDelay());

        yield return null;
    }


    private IEnumerator ActivateSphereAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (sphere != null)
            sphere.SetActive(true);
    }


    private void PlayCutsceneVFX()
    {
        // STARS
        if (stars != null)
        {
            stars.SetActive(true);
            SetStarAlpha(1f);

            if (starParticles != null)
            {
                starParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                starParticles.Play(true);
            }
        }

        // SHARDS
        if (shards != null)
        {
            shards.SetActive(true);
            SetShardAlpha(1f);

            if (shardParticles != null)
            {
                shardParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                shardParticles.Play(true);
            }
        }
    }


    private IEnumerator ForceAnimationObjectsOffAfterDelay()
    {
        yield return new WaitForSeconds(animationCleanupDelay);

        // Disable only the Animator. Do not Rebind it: that would reset the
        // camera to its starting pose instead of leaving it at the final pose.
        if (cameraAnimator != null)
            cameraAnimator.enabled = false;

        // Keep the animation camera active at its final position.
        // Keep the first-person camera and player controls disabled.
        sequenceRunning = false;
        cameraAnimationFinished = true;
        animationCleanupCoroutine = null;

        // Disable only the trigger collider. The Trigger script itself must
        // remain enabled so it can listen for K after the camera settles.
        Collider[] triggerColliders = GetComponents<Collider>();
        foreach (Collider triggerCollider in triggerColliders)
        {
            if (triggerCollider != null && triggerCollider.isTrigger)
                triggerCollider.enabled = false;
        }
    }

    private IEnumerator PlayKKeySequence()
    {
        // K only works after the camera animation has finished.
        // This lock also prevents rapid presses from interrupting the animations.
        if (!cameraAnimationFinished || kSequenceRunning)
            yield break;

        kSequenceRunning = true;

        // Hide the original scene objects only on the first K press.
        if (!originalObjectsHidden)
        {
            if (objectsToHideWhenKPressed != null)
            {
                foreach (GameObject objectToHide in objectsToHideWhenKPressed)
                {
                    if (objectToHide != null)
                        objectToHide.SetActive(false);
                }
            }

            originalObjectsHidden = true;
        }

        float longestAnimationDuration;

        if (playFirstKSetNext)
        {
            // FIRST K: original animations + nighttime skybox.
            SetAnimationObjectsActive(reverseAnimatedObjectsToActivateWhenKPressed, false);
            longestAnimationDuration =
                ActivateAndPlayAnimationSet(animatedObjectsToActivateWhenKPressed);

            SetSkybox(nightSkyboxMaterial);
            SetNighttimeObjectsActive(true);
        }
        else
        {
            // SECOND K: reverse animations + daytime skybox.
            SetAnimationObjectsActive(animatedObjectsToActivateWhenKPressed, false);
            longestAnimationDuration =
                ActivateAndPlayAnimationSet(reverseAnimatedObjectsToActivateWhenKPressed);

            SetSkybox(daySkyboxMaterial);
            SetNighttimeObjectsActive(false);
        }

        // Alternate which set will play on the next valid K press.
        playFirstKSetNext = !playFirstKSetNext;

        // Wait for the longest animation before allowing another K press.
        // The minimum lock also protects against missing Animator components.
        float lockDuration = Mathf.Max(
            longestAnimationDuration,
            kSequenceMinimumLockTime
        ) + kSequenceExtraLockTime;

        yield return new WaitForSecondsRealtime(lockDuration);

        kSequenceRunning = false;
    }

    private float ActivateAndPlayAnimationSet(GameObject[] animatedObjects)
    {
        if (animatedObjects == null)
            return 0f;

        // Activate every object first so all animations can begin together.
        foreach (GameObject animatedObject in animatedObjects)
        {
            if (animatedObject != null)
                animatedObject.SetActive(true);
        }

        float longestAnimationDuration = 0f;

        // Then reset and start the Animators.
        foreach (GameObject animatedObject in animatedObjects)
        {
            if (animatedObject == null)
                continue;

            Animator[] objectAnimators =
                animatedObject.GetComponentsInChildren<Animator>(true);

            if (objectAnimators.Length == 0)
            {
                Debug.LogWarning(
                    "No Animator was found on '" + animatedObject.name +
                    "' or its children. Add an Animator with an Animator Controller assigned.",
                    animatedObject);
                continue;
            }

            foreach (Animator objectAnimator in objectAnimators)
            {
                if (objectAnimator == null)
                    continue;

                objectAnimator.enabled = true;
                objectAnimator.speed = 1f;
                objectAnimator.Rebind();
                objectAnimator.Update(0f);

                // Read the default state's length so K stays locked until
                // the longest animation in this set has had time to finish.
                if (objectAnimator.runtimeAnimatorController != null &&
                    objectAnimator.layerCount > 0)
                {
                    AnimatorStateInfo stateInfo =
                        objectAnimator.GetCurrentAnimatorStateInfo(0);

                    float playbackSpeed =
                        Mathf.Abs(objectAnimator.speed * stateInfo.speed);

                    if (playbackSpeed > 0.01f)
                    {
                        float duration = stateInfo.length / playbackSpeed;
                        longestAnimationDuration =
                            Mathf.Max(longestAnimationDuration, duration);
                    }
                }
            }
        }

        return longestAnimationDuration;
    }

    private void SetAnimationObjectsActive(GameObject[] animatedObjects, bool active)
    {
        if (animatedObjects == null)
            return;

        foreach (GameObject animatedObject in animatedObjects)
        {
            if (animatedObject != null)
                animatedObject.SetActive(active);
        }
    }

    private void SetSkybox(Material skyboxMaterial)
    {
        if (skyboxMaterial == null)
            return;

        RenderSettings.skybox = skyboxMaterial;
        DynamicGI.UpdateEnvironment();
    }

    private void SetNighttimeObjectsActive(bool active)
    {
        if (nighttimeOnlyObjects == null)
            return;

        foreach (GameObject nighttimeObject in nighttimeOnlyObjects)
        {
            if (nighttimeObject != null)
                nighttimeObject.SetActive(active);
        }
    }

    private IEnumerator FinishGalaxyTransition()
    {
        // Lock immediately so repeated Space presses cannot restart the fade.
        if (galaxyTransitionStarted || kSequenceRunning || !cameraAnimationFinished)
            yield break;

        galaxyTransitionStarted = true;

        float fadeWaitTime = Mathf.Max(0f, galaxyFadeDuration);

        if (galaxyFadeAnimator != null && !string.IsNullOrEmpty(galaxyFadeStateName))
        {
            galaxyFadeAnimator.gameObject.SetActive(true);
            galaxyFadeAnimator.enabled = true;
            galaxyFadeAnimator.speed = 1f;
            galaxyFadeAnimator.Rebind();
            galaxyFadeAnimator.Update(0f);

            int fadeStateHash = Animator.StringToHash(galaxyFadeStateName);
            galaxyFadeAnimator.Play(fadeStateHash, 0, 0f);
            galaxyFadeAnimator.Update(0f);

            AnimatorStateInfo fadeState = galaxyFadeAnimator.GetCurrentAnimatorStateInfo(0);
            if (fadeState.IsName(galaxyFadeStateName) && fadeState.length > 0f)
            {
                fadeWaitTime = fadeState.length / Mathf.Max(0.01f, Mathf.Abs(galaxyFadeAnimator.speed));
            }
            else
            {
                Debug.LogWarning(
                    "Galaxy fade state '" + galaxyFadeStateName +
                    "' was not found. The script will use Galaxy Fade Duration instead.",
                    galaxyFadeAnimator);
            }
        }
        else
        {
            Debug.LogWarning(
                "Galaxy Fade Animator or state name is missing. The transition will continue using Galaxy Fade Duration.",
                this);
        }

        // Keep the cinematic camera and player lock in place until the fade is finished.
        yield return new WaitForSecondsRealtime(fadeWaitTime);

        // Reset the portal after the galaxy fade has finished.
        // This plays the portal's return/stretch-undo animation.
        PlayPortalAnimation(portalStretchUndoState);

        // Turn off both portal effects as soon as the undo animation begins.
        // SetVFX also stops and clears their Particle Systems.
        SetVFX(false);

        // Choose the destination from the currently selected day/night state.
        bool isNighttime = !playFirstKSetNext;
        Transform destination = isNighttime ? nighttimeDestination : daytimeDestination;

        // Move the player before restoring the first-person camera and controls.
        Transform rootToMove = playerRoot;
        if (rootToMove == null && playerCharacterController != null)
            rootToMove = playerCharacterController.transform;

        if (destination != null && rootToMove != null)
        {
            // Temporarily disable CharacterController so teleporting does not cause collision issues.
            bool controllerWasEnabled = playerCharacterController != null && playerCharacterController.enabled;
            if (playerCharacterController != null)
                playerCharacterController.enabled = false;

            rootToMove.SetPositionAndRotation(destination.position, destination.rotation);

            if (playerCharacterController != null)
                playerCharacterController.enabled = controllerWasEnabled;
        }
        else if (destination == null)
        {
            Debug.LogWarning(
                (isNighttime ? "Nighttime" : "Daytime") +
                " Destination is not assigned. The player will remain at their current position.",
                this);
        }
        else
        {
            Debug.LogWarning(
                "Player Root and CharacterController are both unassigned. Assign Player Root to move the player to the selected area.",
                this);
        }

        // Turn off the entire FadeIn_Galaxy GameObject after reaching the spawnpoint.
        if (galaxyFadeAnimator != null)
        {
            galaxyFadeAnimator.enabled = false;
            galaxyFadeAnimator.gameObject.SetActive(false);
        }

        if (animationCamera != null)
        {
            animationCamera.enabled = false;
            animationCamera.gameObject.SetActive(false);
        }

        if (firstPersonCamera != null)
        {
            firstPersonCamera.gameObject.SetActive(true);
            firstPersonCamera.enabled = true;
        }

        if (sphere != null)
            sphere.SetActive(false);

        if (shakeScript != null)
            shakeScript.enabled = false;

        SetPlayerControl(true);

        // Prepare the portal for another complete visit.
        ResetSequenceForNextVisit();
    }

    private void ResetSequenceForNextVisit()
    {
        // Clear the previous visit's state so Y, K, and Space can be used again.
        cutsceneStarted = false;
        cameraAnimationFinished = false;
        galaxyTransitionStarted = false;
        sequenceRunning = false;
        kSequenceRunning = false;
        moving = false;

        // The old trigger collider was disabled when the camera sequence ended.
        // Clear old overlap data before enabling it again so the next entry is fresh.
        playerCollidersInside.Clear();
        playerInsideTrigger = false;
        SetPromptVisible(false);

        Collider[] triggerColliders = GetComponents<Collider>();
        foreach (Collider triggerCollider in triggerColliders)
        {
            if (triggerCollider != null && triggerCollider.isTrigger)
                triggerCollider.enabled = true;
        }
    }

    private void SetPlayerControl(bool allowControl)
    {
        if (playerMovementScripts != null)
        {
            for (int i = 0; i < playerMovementScripts.Length; i++)
            {
                if (playerMovementScripts[i] != null)
                {
                    bool wasEnabled = movementWasEnabled != null &&
                        i < movementWasEnabled.Length && movementWasEnabled[i];
                    playerMovementScripts[i].enabled = allowControl ? wasEnabled : false;
                }
            }
        }

        if (playerCharacterController != null)
            playerCharacterController.enabled = allowControl ? characterControllerWasEnabled : false;
    }


    // =========================================================
    // EXIT PORTAL
    // =========================================================

    private IEnumerator ExitPortal()
    {
        moving = true;

        if (animationCleanupCoroutine != null)
        {
            StopCoroutine(animationCleanupCoroutine);
            animationCleanupCoroutine = null;
        }

        if (cameraAnimator != null)
        {
            cameraAnimator.enabled = false;
            cameraAnimator.Rebind();
            cameraAnimator.Update(0f);
        }

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

        moving = false;
        sequenceRunning = false;
        if (animationCamera != null)
            animationCamera.enabled = false;
        if (firstPersonCamera != null)
            firstPersonCamera.enabled = true;
        SetPlayerControl(true);

        // If the player stepped back into the trigger while the exit effects
        // were finishing, restore the entry effects now instead of leaving
        // the portal switched off.
        if (playerInsideTrigger && !cutsceneStarted)
            StartCoroutine(EnterPortal());
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