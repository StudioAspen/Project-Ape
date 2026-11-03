using JetBrains.Annotations;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class BackflipTestController : MonoBehaviour
{
    #region Variables
    [Header("Camera")]
    public Transform cameraTransform; //The Camera pivot goes here in editor

    [Header("Jumping")]
    public float coyoteTimer = 0.2f;
    public float gravity = -90f;
    public float jumpHeight = 10f;
    public float jumpRelease = 2.5f;

    [Header("BackFlip")] 
    public float backflipSpeed = 10f;
    public float backflipHeight = 10f;
    public float jumpBufferTime = 0.15f;
    public float backflipGraceTime = 0.15f;
    private float jumpBufferCounter;
    private float backflipGraceCounter;

    [Header("WallJump")] 
    public float groundCheckDistance = 2f;
    public float wallCheckDistance = 0.7f;
    public float wallJumpLock = 0.2f;
    public float wallJumpHorizontal = 10f;
    public float wallJumpVertical = 10f;

    [Header("Movement")]
    public Transform modelTransform; //The Character Model Goes here in the editor
    public float acceleration = 40f;
    public float blockedTimer = 1f;
    public float deadZone = 0.4f;
    public float fastSpeed = 20f;
    public float slowSpeed = 1f;
    public float rotationSpeed = 10f;
    public float modelRotateSpeed = 10f;
    public Vector2 moveInput;
    public Vector2 previousStick = Vector2.zero;
    public Vector3 velocity;
    public CharacterController controller;


    [Header("Skid")]
    public float skidTolerance = -0.7f;
    public float skidDuration = 0.2f;
    public float skidDeceleration = 30f;
    public static float skidTimer;
    public static bool isSkidding;


    [Header("Spin")]
    public float jumpSpinHeight = 20f;
    public float spinAngleTrigger = 720f;
    public float spinTimeout = 0.3f;
    public float spinStateLength = 4f;
    public static float rotationCheck = 0f;
    public static float angleDelta = 0f;
    public static float spinStateTimer;
    public static float spinCooldownTimer;
    public static bool isSpinning;
    public static bool wasSpinning;

    [Header("Roll")]
    public float rollDash = 25f;
    public float rollDuration = 0.2f;

    // Crtl Elements
    bool crtlHeld;

    /*
    [Header("Audio Stuff")]
    public GameObject Player;
    FMOD.Studio.EventInstance FootstepsSound;
    [SerializeField] EventReference FootstepEvent;
    public float FootstepRate = .4f;
    private float StepTime = 0f;


    [SerializeField] EventReference JumpEvent;
    FMOD.Studio.EventInstance JumpSound;


    [SerializeField] EventReference SpinEvent;
    FMOD.Studio.EventInstance SpinSound;

    [SerializeField] EventReference SkidEvent;
    FMOD.Studio.EventInstance SkidSound;
    private float skidTime = 0f;

    [SerializeField] EventReference RollEvent;
    FMOD.Studio.EventInstance RollSound;
    private float RollTime = 0f;
    */

    //private variables
    private bool jumped;
    private bool jumpHold;
    private bool walker;
    private bool isBackflipping;
    private bool movementBlocked;
    private bool isTouchingWall;
    private bool isWallJumping;
    private bool isNearGround;
    private bool isWalking;
    private bool jumpAudioCheck;
    private bool spinAudioCheck;
    private bool Airlock;
 
    private bool rollState;

    private float verticalLookRotation;
    private float speed;
    
    private float clockoyote = 0f;
    private float movementBlockTimer;
    private float midSpeed;
    private float rollTimer = 0f;

    private int spinDirection = 0;


    private PlayerInput inputActions;

    private Vector2 lookInput;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private Vector3 launchDirection;
    private Vector3 rollDirection;
    #endregion

    #region Execution Functions
    /*
    --------------------------------------- UNITY'S EXECUTION FUNCTIONS -----------------------------------------------------------------------
    */

    void Awake()
    {
        controller = GetComponent<CharacterController>(); //Start taking information from controller input
        inputActions = new PlayerInput();

        /*
        //audio variables
        FootstepsSound = FMODUnity.RuntimeManager.CreateInstance(FootstepEvent);
        JumpSound = FMODUnity.RuntimeManager.CreateInstance(JumpEvent);
        SpinSound = FMODUnity.RuntimeManager.CreateInstance(SpinEvent);
        SkidSound = FMODUnity.RuntimeManager.CreateInstance(SkidEvent);
        RollSound = FMODUnity.RuntimeManager.CreateInstance(RollEvent);
        Debug.Log(spinTimeout);
        */
    }

    void OnEnable()
    {
        inputActions.Enable();

        //Connecting Left Joystick and WASD
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        //Connecting Right Joystick and Mouse
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        //Connecting Sprint Alternative for Keyboard
        inputActions.Player.Sprint.performed += ctx => walker = true;
        inputActions.Player.Sprint.canceled += ctx => walker = false;

        inputActions.Player.Crtl.performed += ctx => crtlHeld = true;
        inputActions.Player.Crtl.canceled += ctx => crtlHeld = false;

        //Connecting Jump press
        inputActions.Player.Jump.performed += ctx =>
        {
            if(crtlHeld)
            {
                Debug.Log("Backflip");
                jumpHold = true;
                Backflip();
            }
            else
            {
                Debug.Log("Regular Jump");
                jumpHold = true;
                Jump();
            }
        };

        inputActions.Player.Jump.canceled += ctx =>
        {
            jumpHold = false;
        };

        inputActions.Player.Roll.performed += ctx =>
        {
            RollDetect();
        };
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        RotateCamera();
        RotateModel();
        Roll();
        MoveDirectCheck();
        ApplyGravity();
        MoveanBlockCheck();
        CoyoteTimeCheck();
        WallJumpCheck();
        //AudioChecks();
        DetectSpin();

        Debug.Log(jumpBufferCounter);

        if (inputActions.Player.Jump.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if(isSkidding)
        {
            skidTimer -= Time.deltaTime;

            if(jumpBufferCounter > 0f)
            {
                jumpBufferCounter = 0f;
                isSkidding = false;
                Backflip();
                return;
            }
        }
    }
    #endregion

    #region Movement Checks 
    void MoveanBlockCheck()
    {
        if (movementBlocked) //if the boolean is not on, allow player to move by ignoring this function
        {
            StartMoveanBlock();
        }
        else // if there is no blockage move as usual
        {
            Move();
        }
    }

    void WallJumpCheck()
    {
        isNearGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance); //checks if the player is near ground

        if (!controller.isGrounded) //checks for walls while jumping, no need to check them during ground state.
        {
            StartCheckWall();
        }
    }

    void MoveDirectCheck()
    {
        //Makes movement obey deadzones
        if (moveInput.magnitude > deadZone)
        {
            moveDirection = MoveDirect();
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }
    void CoyoteTimeCheck()
    {
        //defines coyote time to jump
        if (controller.isGrounded || !jumped)
        {
            clockoyote = coyoteTimer;
        }
        else
        {
            clockoyote -= Time.deltaTime;
        }
    }
    #endregion

    #region Main Movement Functions

    Vector3 MoveDirect() //RESPONSIBLE OF HORIZONTAL MOVEMENT
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (isBackflipping || isWallJumping) //ignore user's joystick direction while backflipping
        {
            return launchDirection;
        }

        return (forward * moveInput.y + right * moveInput.x).normalized;
    }

    void Move() //HANDLES HORIZONTAL MOVEMENT SPEED
    {
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
        //ifelse used to let keyboard users walk using shift.
        if (!controller.isGrounded)
        {
            isSkidding = false;
        }

        if (walker || isSpinning)
        {
            midSpeed = slowSpeed;
        }

        else if (inputMagnitude > deadZone)
        {
            midSpeed = Mathf.Lerp(slowSpeed, fastSpeed, inputMagnitude); //Lerp handles the speed scaling from the joystick
        }
        else
        {
            midSpeed = 0f;
        }

        if (!isSkidding && speed > 2f && moveDirection.sqrMagnitude > 0.01f && lastMoveDirection.sqrMagnitude > 0.01f) //skidcheck
        {
            float dot = Vector3.Dot(lastMoveDirection, moveDirection);

            if (dot < skidTolerance)
            {
                isSkidding = true;
                skidTimer = skidDuration;
            }
        }

        if (isSkidding) //when the skid state starts, run down the timer, perform the skid and leave the skid state once the timer has passed
        {
            skidTimer -= Time.deltaTime;

            speed = Mathf.MoveTowards(speed, 0f, skidDeceleration * Time.deltaTime);

            controller.Move(lastMoveDirection * speed * Time.deltaTime);

            if (skidTimer <= 0f)
                isSkidding = false;
            return;
        }

        speed = Mathf.MoveTowards(speed, midSpeed, acceleration * Time.deltaTime);
        controller.Move(moveDirection * speed * Time.deltaTime); //actual moving registered

        if (speed > 0.1f && moveDirection.sqrMagnitude > 0.1f) //detection of the direction
        {
            isWalking = true;
            lastMoveDirection = moveDirection;
        }
        else
        {
            isWalking = false;
        }
    }

    void ApplyGravity() //HANDLES VERTICAL SPEED
    {

        //this resets vertical speed when a player collides with a ceiling
        CollisionFlags flags = controller.Move(velocity * Time.deltaTime * 2f);
        
        if ((flags & CollisionFlags.Above) != 0 && velocity.y > 0)
        {
            velocity.y = 0f;
            movementBlockTimer = 0f; //interrupts special jumps if a wall is hit
        }
        

        //this maintains the player grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumped = false;
        }

        //this implements the cut in jumping for variable jump
        if (velocity.y > 0f && !jumpHold)
        {
            velocity.y += gravity * Time.deltaTime * jumpRelease;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void Jump() //HANDLES CHANGES IN VERTICAL SPEED CAUSED BY JUMPING
    {
        StartCheckWall(); //checks for walls during movement lock.

        //WALL JUMP (only next to a wall and far from ground)
        if (isTouchingWall && jumped && !isNearGround)
        {
            jumped = true;
            jumpAudioCheck = true;
            clockoyote = 0f;

            movementBlocked = true;
            isWallJumping = true;
            movementBlockTimer = wallJumpLock;
            velocity = launchDirection * wallJumpHorizontal;
            velocity.y = wallJumpVertical;
            return;
        }

        //OTHER JUMPS ALLOWED DURING COYOTE TIME
        if (clockoyote > 0f) //preventing user from jumping more than once
        {

            //SPIN JUMP (only in ground and needs to be spinning)
            if (isSpinning)
            {
                velocity.y = Mathf.Sqrt(jumpSpinHeight * -2f * gravity);
                jumped = true;
                jumpAudioCheck = true;
            }

            //BASE JUMP
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                clockoyote = 0f;
                jumped = true;
                jumpAudioCheck = true;
            }

            //BACKFLIP (needs to be in skid state)
            if (isSkidding)
            {
                Debug.Log("Do A Backflip!");
                isSkidding = false;
                speed = 0f;
                Backflip();
                jumpAudioCheck = true;

            }

            //variable reset after jumps
            isSpinning = false;
            rotationCheck = 0f;
            spinDirection = 0;
            previousStick = Vector2.zero;
            spinStateTimer = spinStateLength;
        }
    }


    void DetectSpin()
    {
        if (IsGroundedSphere())
        {
            angleDelta = Vector2.SignedAngle(previousStick, moveInput); //storing spin angle

            if (Mathf.Abs(angleDelta) > 1f && previousStick != Vector2.zero)
            {
                int currentDirection = angleDelta > 0 ? 1 : -1;

                if (spinDirection == 0) //detects beginning of spin
                {
                    spinDirection = currentDirection;
                    spinCooldownTimer = spinTimeout;
                }

                if (currentDirection == spinDirection && !isSpinning) //checks if the joystick goes to the same direction, if it does, add to the spin charge
                {
                    rotationCheck += Mathf.Abs(angleDelta);
                    spinCooldownTimer -= Time.deltaTime;

                    if (spinCooldownTimer < 0f)
                    {
                        rotationCheck = 0f;
                        spinCooldownTimer = spinTimeout;
                        spinDirection = currentDirection;
                    }
                }
                else //if not reset the charge
                {
                    rotationCheck = 0f;
                    spinDirection = currentDirection;
                    spinCooldownTimer = spinTimeout;
                }

                if (rotationCheck >= spinAngleTrigger)
                {
                    isSpinning = true;

                    return;
                }
            }
        }
        previousStick = moveInput;

        if (isSpinning)
        {
            spinStateTimer -= Time.deltaTime;

            if (spinStateTimer < 0f)
            {
                isSpinning = false;
                rotationCheck = 0f;
                spinDirection = 0;
                previousStick = Vector2.zero;
                spinStateTimer = spinStateLength;

                return;
            }
        }
    }

    void RotateCamera() //RESPONSIBLE OF THE CAMERA ROTATION AROUND THE MODEL.
    {
        float lookSensitivity = 120f;

        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        cameraTransform.parent.Rotate(0f, mouseX, 0f);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -70f, 70f);

        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }


    void RotateModel() //RESPONSIBLE OF ROTATING THE MODEL ACCORDING TO THE DIRECTION THE PLAYER IS MOVING
    {

        if (moveDirection.magnitude > 0.1f && !isBackflipping) //Moves only when joystick is tilted OR blocks when the player is doing a backflip
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, modelRotateSpeed * Time.deltaTime);
        }
        
    }


    void Backflip() //RESPONSIBLE OF THE BACKFLIP MECHANIC
    {
        //prevents extra movements and extra jumps during the backflip
        jumped = true;
        clockoyote = 0f;
        launchDirection = MoveDirect(); 

        //begins block movement and sets the timer
        movementBlocked = true;
        isBackflipping = true;
        movementBlockTimer = blockedTimer;

        //sets direction and height for the backflip.
        velocity.x = launchDirection.x * backflipSpeed;
        velocity.y = Mathf.Sqrt(backflipHeight * -2f * gravity);
        velocity.z = launchDirection.z * backflipSpeed;
    }

    void StartMoveanBlock() //RESPONSIBLE OF BLOCKING USER MOVEMENT WHEN PERFORMING CERTAIN TYPES OF JUMPS
    {

        movementBlockTimer -= Time.deltaTime; //start the blocked movement period

        //then reset everything once the time is up or if the player prematurely gets on a platform
        if (movementBlockTimer <= 0f || controller.isGrounded)
        {
            movementBlocked = false;

            if (isBackflipping) //reset values after Backflip
            {
                isBackflipping = false;
                velocity.x = 0f;
                velocity.z = 0f;
                launchDirection = Vector3.zero;
            }
            if (isWallJumping) //reset values after Walljump
            {
                isWallJumping = false;

                speed = new Vector2(velocity.x, velocity.z).magnitude;
                moveDirection = new Vector3(velocity.x, 0f, velocity.z).normalized;
                lastMoveDirection = moveDirection;

                velocity.x = 0f;
                velocity.z = 0f;
                launchDirection = Vector3.zero;
            }
        }
    }


    void StartCheckWall() //RESPONSIBLE OF CHECKING IF THE PLAYER IS TOUCHING A WALL
    {
        isTouchingWall = false;

        Vector3 origin = transform.position + Vector3.up;

        if (Physics.Raycast(origin, modelTransform.forward, out RaycastHit hit, wallCheckDistance)) //the raycast checks if there is a wall
        {
            isTouchingWall = true;
            launchDirection = hit.normal;
        }
    }

    void RollDetect() //TRIGGERS BOOLEANS AND TIMES WHEN PLAYER PRESSES THE ROLL BUTTON
    {
        if (rollState) return; //prevent user input from spamming.

        rollTimer = rollDuration; //starts the roll
        rollState = true; //tells roll() in update to run properly.
    }

    void Roll() //PERFORMS THE ACTUAL ROLL
    {
        if (controller.isGrounded) // Airlock permits player to only roll once middair, preventing infinite spam.
        {
            Airlock = false;
        }
        if (!rollState) return;

        launchDirection = MoveDirect();
        rollTimer -= Time.deltaTime;

        if (moveInput.magnitude > deadZone && !Airlock) //checks that the player is actually moving towards somewhere, preventing mistriggers with no input
        {
            velocity = launchDirection * rollDash;
        }

        if (rollTimer <= 0f) //once the roll is over, reset everything and activate Airlock if the player hasn't hit the ground,
        {
            rollState = false;
            velocity.x = 0f;
            velocity.z = 0f;
            Airlock = true;
        }
    }
    #endregion

    #region Ground Check Functions

    Vector3 SphereCastOrigin()
    {
        Vector3 origin = transform.position + (Vector3.up * 0.5f);

        return origin;
    }

    Vector3 RayCastOrigin()
    {
        Vector3 origin = transform.position + (Vector3.up * 0.3f);

        return origin;
    }

    public bool IsGrounded()
    {
        float rayDistance = 0.6f;
        Vector3 debugRayEnd = RayCastOrigin() + (Vector3.down * rayDistance);

        Debug.DrawLine(RayCastOrigin(), debugRayEnd, Color.blue);
        if (Physics.Raycast(RayCastOrigin(), Vector3.down, rayDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsGroundedSphere()
    {
        float rayDistance = 0.8f;

        if (Physics.SphereCast(SphereCastOrigin(), controller.radius - 0.4f, Vector3.down, out RaycastHit hitInfo, rayDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        if (IsGroundedSphere())
        {
            Gizmos.DrawSphere(SphereCastOrigin(), controller.radius - 0.4f);
        }
    }

    #endregion

    #region Audio Functions
    /*
         --------------------------------------- AUDIO FUNCTIONS ----------------------------------------------------------------------------------
         */

    /*
    void AudioChecks() //contains all the checks for movement sounds
    {
        StepTime += Time.deltaTime;
        skidTime += Time.deltaTime;
        RollTime += Time.deltaTime;

        //footsteps sound

        if (isWalking && isNearGround && !isSkidding && !isSpinning && !jumped && !isBackflipping && !rollState)
        {
            if (StepTime >= FootstepRate)
            {
                FootstepsSound.start();
                StepTime = 0f;
            }
        }
        else if (isSkidding || isSpinning || jumped || isBackflipping || rollState)
        {
            FootstepsSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        //jump sound

        if (jumpAudioCheck && jumped)
        {
            JumpSound.start();
            jumpAudioCheck = false;

        }

        //spinning sound

        if (isSpinning && spinAudioCheck)
        {
            Debug.Log("i");
            SpinSound.start();
            spinAudioCheck = false;
        }
        
        if(!isSpinning)
        {
            SpinSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        //skid sound

        if (isSkidding && isNearGround)
        {
            if (skidTime >= skidDuration + .2f)
            {
                SkidSound.start();
                skidTime = 0f;
            }
        }
        else
        {
            SkidSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (rollState)
        {
            if (RollTime >= rollDuration + .2f)
            {
                RollSound.start();
                RollTime = 0f;
            }
        }
        else
        {
            RollSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    */
    #endregion
}