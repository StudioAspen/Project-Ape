
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControllerInputManager : MonoBehaviour
{
    [Header("Controller Events")]

    [Tooltip("A / Cross: Confirm action.")]
    public UnityEvent onConfirm;

    [Tooltip("B / Circle: Back action.")]
    public UnityEvent onBack;

    [Tooltip("Y / Triangle: Open portal.")]
    public UnityEvent onOpenPortal;

    [Tooltip("Left or Right: Switch between day and night.")]
    public UnityEvent onSwitchDayNight;


    [Header("Keyboard Settings")]

    [Tooltip("B key for the Back action.")]
    public Key backKey = Key.B;

    [Tooltip("Enable keyboard Back input.")]
    public bool allowKeyboardBack = true;


    [Header("Left Stick Settings")]

    [Tooltip("How far the stick must move before registering a direction.")]
    [Range(0.1f, 1f)]
    public float stickDeadzone = 0.65f;


    [Header("Input Settings")]

    [Tooltip("Allow controller inputs while the game is focused.")]
    public bool requireApplicationFocus = true;


    // Prevents repeated day/night switching while holding a direction.
    private bool horizontalInputLocked = false;


    private void Update()
    {
        if (requireApplicationFocus && !Application.isFocused)
            return;

        HandleControllerInput();
        HandleKeyboardBack();
    }


    // =====================================================
    // CONTROLLER INPUT
    // =====================================================

    private void HandleControllerInput()
    {
        if (Gamepad.all.Count == 0)
            return;

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad == null)
                continue;


            // =============================================
            // A / CROSS - CONFIRM
            // =============================================

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                onConfirm?.Invoke();
            }


            // =============================================
            // B / CIRCLE - BACK
            // =============================================

            if (gamepad.buttonEast.wasPressedThisFrame)
            {
                onBack?.Invoke();
            }


            // =============================================
            // Y / TRIANGLE - OPEN PORTAL
            // =============================================

            if (gamepad.buttonNorth.wasPressedThisFrame)
            {
                onOpenPortal?.Invoke();
            }


            // =============================================
            // D-PAD LEFT / RIGHT
            // =============================================

            bool leftPressed =
                gamepad.dpad.left.wasPressedThisFrame;

            bool rightPressed =
                gamepad.dpad.right.wasPressedThisFrame;


            // =============================================
            // LEFT STICK LEFT / RIGHT
            // =============================================

            float stickX = gamepad.leftStick.x.ReadValue();

            bool stickLeft = stickX <= -stickDeadzone;
            bool stickRight = stickX >= stickDeadzone;


            // =============================================
            // COMBINE D-PAD AND LEFT STICK
            // =============================================

            bool horizontalPressed =
                leftPressed ||
                rightPressed ||
                stickLeft ||
                stickRight;


            // Switch only once until the player releases
            // the stick or D-pad direction.

            if (horizontalPressed && !horizontalInputLocked)
            {
                horizontalInputLocked = true;

                onSwitchDayNight?.Invoke();
            }


            // Unlock when both D-pad and stick return
            // to their neutral positions.

            bool stickNeutral =
                Mathf.Abs(stickX) < stickDeadzone;

            bool dpadNeutral =
                !gamepad.dpad.left.isPressed &&
                !gamepad.dpad.right.isPressed;

            if (stickNeutral && dpadNeutral)
            {
                horizontalInputLocked = false;
            }
        }
    }


    // =====================================================
    // KEYBOARD BACKUP
    // =====================================================

    private void HandleKeyboardBack()
    {
        if (!allowKeyboardBack)
            return;

        if (Keyboard.current == null)
            return;

        if (backKey == Key.None)
            return;

        if (Keyboard.current[backKey].wasPressedThisFrame)
        {
            onBack?.Invoke();
        }
    }
}
