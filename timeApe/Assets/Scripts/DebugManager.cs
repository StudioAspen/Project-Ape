using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Text")]
    private TextMeshProUGUI spinDebugText;
    private bool isMenuActive;
    private Gamepad controller;
    private Keyboard keyboardInput;

    void Start()
    {
        spinDebugText = FindAnyObjectByType<TextMeshProUGUI>();
        controller = Gamepad.current;
        keyboardInput = Keyboard.current;
        spinDebugText.enabled = false;
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
                $"Jump velocity: {TestThirdPersonController.velocity.y} \n" +
                $"Angle between current input \n" +
                $"and previous input: {TestThirdPersonController.angleDelta} \n" +
                $"Current input: {TestThirdPersonController.moveInput} \n" +
                $"Current input magnitude/length: {TestThirdPersonController.moveInput.sqrMagnitude} \n" +
                $"Previous input: {TestThirdPersonController.previousStick} \n" +
                $"Rotation check: {TestThirdPersonController.rotationCheck} \n" +
                $"Stick speed: {Mathf.Round(TestThirdPersonController.stickSpeed),0} \n" +
                $"Spin state trigger angle: {TestThirdPersonController.spinAngleTrigger} \n" +
                $"Is player spinning: {TestThirdPersonController.isSpinning} \n" +
                $"Was player spinning: {TestThirdPersonController.wasSpinning} \n" +
                $"Reset spin timer: {TestThirdPersonController.testTimer} \n" +
                $"Spin cooldown timer: {TestThirdPersonController.spinCooldownTimer} \n" +
                $"Spin state length timer: {TestThirdPersonController.spinStateTimer} ";
            
        return;
    }
}
