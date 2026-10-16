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
                $"and previous input: {player.angleDelta} \n" +
                $"Current input: {player.moveInput} \n" +
                $"Current input magnitude/length: {player.moveInput.sqrMagnitude} \n" +
                $"Previous input: {player.previousStick} \n" +
                $"Rotation check: {player.rotationCheck} \n" +
                $"Spin state trigger angle: {player.spinAngleTrigger} \n" +
                $"Is player spinning: {player.isSpinning} \n" +
                $"Was player spinning: {player.wasSpinning} \n" +
                $"Spin cooldown timer: {player.spinCooldownTimer} \n" +
                $"Character controller grounded: {player.controller.isGrounded} \n" +
                $"Spin state length timer: {player.spinStateTimer} ";
            
        return;
    }
}
