using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [Header("TMP References")]
    public TextMeshProUGUI spinDebugText;
    public TextMeshProUGUI skiddingDebugText;

    [Header("Input")]
    private bool isMenuActive;
    private Gamepad controller;

    [Header("Component References")]
    private TestThirdPersonController player;

    void Start()
    {
        controller = Gamepad.current;

        spinDebugText.enabled = false;
        skiddingDebugText.enabled = false;

        player = FindAnyObjectByType<TestThirdPersonController>();
    }

    void Update()
    {
        HandleInput();
        HandleSkidText();
        HandleSpinText();
    }
    
    void HandleInput()
    {
        if (controller.rightStickButton.wasPressedThisFrame && !isMenuActive)
        {
            isMenuActive = true;
            spinDebugText.enabled = true;
            skiddingDebugText.enabled = true;

        }

        else if (controller.rightStickButton.wasPressedThisFrame && isMenuActive)
        {
            isMenuActive = false;
            spinDebugText.enabled = false;
            skiddingDebugText.enabled = false;
        }
    }

    void HandleSpinText()
    {
            spinDebugText.text = $"|SPIN STATE DEBUG| \n" +
                $"Jump velocity: {player.velocity.y} \n" +
                $"Angle between current input \n" +
                $"and previous input: {TestThirdPersonController.angleDelta} \n" +
                $"Current input: {player.moveInput} \n" +
                $"Current input magnitude/length: {player.moveInput.sqrMagnitude} \n" +
                $"Previous input: {player.previousStick} \n" +
                $"Rotation check: {TestThirdPersonController.rotationCheck} \n" +
                $"Spin state trigger angle: {player.spinAngleTrigger} \n" +
                $"Is player spinning: {TestThirdPersonController.isSpinning} \n" +
                $"Spin cooldown timer: {TestThirdPersonController.spinCooldownTimer} \n" +
                $"Character controller grounded: {player.controller.isGrounded} \n" +
                $"Sphere grounded: {player.IsGroundedSphere()} \n" +
                $"Spin state length timer: {TestThirdPersonController.spinStateTimer} ";
            
        return;
    }

    void HandleSkidText()
    {
        skiddingDebugText.text = $"|SKIDDING STATE DEBUG| \n" +
                $"Skid Tolerance: {player.skidTolerance} \n" +
                $"Skid Duration: {player.skidDuration} \n" +
                $"Skid Deceleration: {player.skidDeceleration} \n" +
                $"Skid Timer: {TestThirdPersonController.skidTimer} \n" +
                $"Is Skidding: {TestThirdPersonController.isSkidding}";

        return;
    }
}
