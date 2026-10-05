using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    //[Header("Debug Variables")]

    [Header("Debug Text")]
    private TextMeshProUGUI jumpVelocity;

    void Start()
    {
        jumpVelocity = FindAnyObjectByType<TextMeshProUGUI>();
    }

    void Update()
    {
        JumpVelocity();   
    }
    
    void JumpVelocity()
    {
        jumpVelocity.text = $"Jump velocity is {Mathf.Round(TestThirdPersonController.velocity.y),0}";
        jumpVelocity.color = Color.black;

        return;
    }
}
