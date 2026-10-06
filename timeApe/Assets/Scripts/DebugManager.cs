using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Text")]
    private TextMeshProUGUI spinDebugText;

    void Start()
    {
        spinDebugText = FindAnyObjectByType<TextMeshProUGUI>();
    }

    void Update()
    {
        SpinDebug();   
    }
    
    void SpinDebug()
    {
        spinDebugText.text = $"|SPIN STATE DEBUG| \n" +
            $"Jump velocity is {Mathf.Round(TestThirdPersonController.velocity.y),0} \n" +
            $"Angle between current input \n" +
            $"and previous input is: {TestThirdPersonController.angleDelta} \n" +
            $"Current input is: {TestThirdPersonController.moveInput} \n" +
            $"Previous input {TestThirdPersonController.previousStick} \n" +
            $"Spin direction is: {TestThirdPersonController.spinDirection} \n" +
            $"Rotation check is: {Mathf.Round(TestThirdPersonController.rotationCheck), 0} \n" +
            $"Spin state trigger angle is: {TestThirdPersonController.spinAngleTrigger} \n" +
            $"Is player spinning: {TestThirdPersonController.isSpinning}";

        spinDebugText.color = Color.black;

        return;
    }
}
