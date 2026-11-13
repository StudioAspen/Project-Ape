using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject cameraTransform; //The Camera pivot goes here in editor
    private PlayerInput inputActions;
    private Vector2 lookInput;
    private float verticalLookRotation;
    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        inputActions = new PlayerInput();
        cameraTransform = GameObject.Find("CameraPivot");
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        float lookSensitivity = 120f;

        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        cameraTransform.transform.parent.Rotate(0f, mouseX, 0f);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 70f);

        cameraTransform.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }
}
