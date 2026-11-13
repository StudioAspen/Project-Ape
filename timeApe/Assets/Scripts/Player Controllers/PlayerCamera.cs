using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform cameraTransform; //The Camera pivot goes here in editor
    private PlayerInput inputActions;
    private Vector2 lookInput;
    private float verticalLookRotation;
    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        inputActions = new PlayerInput();
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
        HandleCollision();
    }

    private void HandleRotation()
    {
        float lookSensitivity = 120f;

        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        cameraTransform.parent.Rotate(0f, mouseX, 0f);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 70f);

        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleCollision()
    {
        Debug.DrawRay(cameraTransform.position, Vector3.down, Color.red);
        if (Physics.Raycast(cameraTransform.position, Vector3.down, 3f))
        {
            verticalLookRotation = 0f;
        }
    }
}
