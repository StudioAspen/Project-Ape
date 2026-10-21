using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Text")]
    private TextMeshProUGUI spinDebugText;
    private bool isMenuActive;

    [Header("Debug Input")]
    private Gamepad controller;

    [Header("Debug References")]
    private TestThirdPersonController player;

    void Start()
    {
        spinDebugText = FindAnyObjectByType<TextMeshProUGUI>();
        controller = Gamepad.current;
        spinDebugText.enabled = false;
        player = FindAnyObjectByType<TestThirdPersonController>();
    }

    void Update()
    {
        SpinDebug();   
    }
    
    void SpinDebug()
    {
        if (controller.rightStickButton.wasPressedThisFrame && !isMenuActive)
        {
            isMenuActive = true;
            spinDebugText.enabled = true;
            
        }

        else if (controller.rightStickButton.wasPressedThisFrame && isMenuActive)
        {
            isMenuActive = false;
            spinDebugText.enabled = false;
            
        }
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
                $"Was player spinning: {TestThirdPersonController.wasSpinning} \n" +
                $"Spin cooldown timer: {TestThirdPersonController.spinCooldownTimer} \n" +
                $"Character controller grounded: {player.controller.isGrounded} \n" +
                $"Spin state length timer: {TestThirdPersonController.spinStateTimer} ";
            
        return;
    }
}
