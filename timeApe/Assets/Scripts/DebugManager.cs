using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Text")]
    private TextMeshProUGUI spinDebugText;
    private bool isMenuActive;
    private Gamepad controller;

    void Start()
    {
        spinDebugText = FindAnyObjectByType<TextMeshProUGUI>();
        controller = Gamepad.current;
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
                $"Jump velocity: {Mathf.Round(TestThirdPersonController.velocity.y),0} \n" +
                $"Angle between current input \n" +
                $"and previous input: {Mathf.Round(Mathf.Abs(TestThirdPersonController.angleDelta)),0} \n" +
                $"Current input: {TestThirdPersonController.moveInput} \n" +
                $"Previous input: {TestThirdPersonController.previousStick} \n" +
                $"Rotation check: {Mathf.Round(TestThirdPersonController.rotationCheck),0} \n" +
                $"Stick speed: {Mathf.Round(TestThirdPersonController.stickSpeed),0} \n" +
                $"Spin state trigger angle: {TestThirdPersonController.spinAngleTrigger} \n" +
                $"Is player spinning: {TestThirdPersonController.isSpinning} \n" +
                $"Was player spinning: {TestThirdPersonController.wasSpinning} \n" +
                $"Reset spin timer: {Mathf.Round(TestThirdPersonController.testTimer),2} \n" +
                $"Spin state length timer: {Mathf.Round(TestThirdPersonController.spinStateTimer),0} ";
            
        return;
    }
}
