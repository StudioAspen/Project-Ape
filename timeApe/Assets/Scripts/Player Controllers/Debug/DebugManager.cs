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
    private PlayerController player;

    void Start()
    {
        controller = Gamepad.current;

        spinDebugText.enabled = false;
        skiddingDebugText.enabled = false;

        player = FindAnyObjectByType<PlayerController>();
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
                $"and previous input: {player.angleDelta} \n" +
                $"Current input: {player.moveInput} \n" +
                $"Current input magnitude/length: {player.moveInput.sqrMagnitude} \n" +
                $"Previous input: {player.previousStick} \n" +
                $"Rotation check: {player.rotationCheck} \n" +
                $"Spin state trigger angle: {player.spinAngleTrigger} \n" +
                $"Is player spinning: {player.isSpinning} \n" +
                $"Spin cooldown timer: {player.spinCooldownTimer} \n" +
                $"Character controller grounded: {player.controller.isGrounded} \n" +
                $"Sphere grounded: {player.IsGroundedSphere()} \n" +
                $"Spin state length timer: {player.spinStateTimer} ";
            
        return;
    }

    void HandleSkidText()
    {
        skiddingDebugText.text = $"|SKIDDING STATE DEBUG| \n" +
                $"Skid Tolerance: {player.skidTolerance} \n" +
                $"Skid Duration: {player.skidDuration} \n" +
                $"Skid Deceleration: {player.skidDeceleration} \n" +
                $"Skid Timer: {player.skidTimer} \n" +
                $"Is Skidding: {player.isSkidding}";

        return;
    }
}
