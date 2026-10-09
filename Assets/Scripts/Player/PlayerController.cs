using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Using Unity's Input System and Rigidbody to move the player.
/// </summary>

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("<color=red>Perminant blocks - READ COMMENT</color>")]
    // These are only to be used perminantly, do not reference these anywhere else
    [SerializeField] private bool perm_canSprint;
    [SerializeField] private bool perm_canJump;
    [SerializeField] private bool perm_canParkourJump;
    [SerializeField] private bool perm_canWalk;
    [SerializeField] private bool perm_canSlide;

    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float airAcceleration = 5f;
    [SerializeField] private float airDrag = 1.5f;
    [SerializeField] private float groundAcceleration = 45f;
    [SerializeField] private float groundDeceleration = 55f;

    private Vector2 movementInput;

    
    [Header("Sprinting")]
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float maxSprintStamina = 2f;
    [SerializeField] private float sprintDrainSpeed = 1f;
    [SerializeField] private float sprintRegenerationSpeed = 0.75f;
    [SerializeField] private float sprintRegenerationDelay = 1f; 
    

    [Header("Parkour Jump")]
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float forwardJumpBoost = 2f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private float groundDistance = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheckRadius;

    [SerializeField, Range(0.1f, 1f)]
    private float jumpReleaseMultiplier = 0.5f;

    private bool jumpReleased;

    [Header("Wall Movement")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float maxWallClingTime = 1f;
    [SerializeField] private float wallSlideSpeed = 3f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpHeight = 2.5f;
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private float wallReattachDelay = 0.25f;

    [Header("Sliding")] 
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideDuration = 1.2f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float slideCooldown = 0.5f;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private float camSlidingHeight = 0.5f;

    [Header("Animations")]
    //Sliding
    [SerializeField] private Animation playerAnim;
    [SerializeField] private AnimationClip slideAnimClip;
    [SerializeField] private AnimationClip resetAnimClip; //Used to reset all animations to the default state

    
    private Rigidbody playerRigidbody;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded;

    private float currentMovementSpeed;
    private float currentSprintStamina;
    private float sprintRegenTimer;

    private float coyoteTimer;
    private float jumpBufferTimer;

    public bool isWallClinging;
    private float wallClingTimer;
    private float wallReattachTimer;
    private Vector3 wallNormal;

    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;
    
    private float normalControllerHeight;
    private Vector3 normalControllerCenter;
    private Vector3 slideDirection;
    private bool freezePlayer;

    private Vector3 normalCameraLocalPosition;

    /// <summary>
    /// Gets the remaining sprint stamina as a value between 1 and 0.
    /// </summary>
    public float SprintStaminaPercent
    {
        get
        {
            if(maxSprintStamina <= 0f)
            {
                return 0f;
            }

            return currentSprintStamina / maxSprintStamina;
        }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate PlayerController Instance destroyed");

            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerRigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // This script will control gravity so Unity's default gravity should be disabled
        playerRigidbody.useGravity = false;

        currentMovementSpeed = speed;
        currentSprintStamina = maxSprintStamina;

        normalControllerHeight = capsuleCollider.height;
        normalControllerCenter = capsuleCollider.center;

        if(playerCam != null)
        {
            normalCameraLocalPosition = playerCam.transform.localPosition;
        }

        SetupAnimationClip(slideAnimClip, WrapMode.ClampForever);
        SetupAnimationClip(resetAnimClip, WrapMode.ClampForever);
    }

    private void Update()
    {
        // Get the keyboard
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null || freezePlayer)
        {
            return;
        }

        ReadMovementInput(keyboard);
        UpdateSliding(keyboard);
        UpdateJumpTimers(keyboard);
        UpdateSprint(keyboard);

        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            jumpReleased = true;
        }
    }

    private void FixedUpdate()
    {
        if (freezePlayer)
        {
            return;
        }

        UpdateGroundCheck();
        UpdateWallState();

        if (!isSliding)
        {
            HandleJumping();
        }

        UpdateGravity();
        UpdateMovement();

        // The released input has now been delt with by the physics update
        jumpReleased = false;
    }

    ///<summary>
    /// Check if the player is touching the ground
    /// </summary>
    private void UpdateGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckRadius.position, groundDistance, groundMask);
    }

    /// <summary>
    /// Apply a custom gravity to the Rigidbody
    /// </summary>
    private void UpdateGravity()
    {
        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        if(isGrounded && currentVelocity.y < 0f)
        {
            // A small force down to keep the player on the floor
            currentVelocity.y = -2f;
        }
        else if (isWallClinging)
        {
            if(wallClingTimer <= maxWallClingTime)
            {
                // Stop the player from falling temporarily
                currentVelocity.y = 0f;
            }
            else
            {
                // SLide down
                currentVelocity.y = -wallSlideSpeed;
                //currentVelocity.y = wallSlideSpeed;
            }
        }
        else
        {
            // Normal graity while in the air
            currentVelocity.y += gravity * Time.fixedDeltaTime;
        }

        playerRigidbody.linearVelocity = currentVelocity;

    }

    /// <summary>
    /// Reads the current WASD input
    /// </summary>
    /// <param name="keyboard">Keyboard.current</param>
    private void ReadMovementInput(Keyboard keyboard)
    {
        movementInput = Vector2.zero;

        if (keyboard.aKey.isPressed)
        {
            movementInput.x -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            movementInput.x += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            movementInput.y -= 1f;
        }

        if (keyboard.wKey.isPressed)
        {
            movementInput.y += 1f;
        }

        movementInput = Vector2.ClampMagnitude(movementInput, 1f);
    }

    /// <summary>
    /// Reads WASD using the Input System and moves the player
    /// </summary>
    private void UpdateMovement()
    {
        if (!perm_canWalk)
        {
            return;
        }

        Vector3 currentVelocity = playerRigidbody.linearVelocity;
        Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 moveDirection = (transform.right * movementInput.x) + (transform.forward * movementInput.y);

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        Vector3 targetHorizontalVelocity;

        if (isSliding)
        {
            targetHorizontalVelocity = slideDirection * slideSpeed;
        }
        else
        {
            targetHorizontalVelocity = moveDirection * currentMovementSpeed;
        }

        float acceleration;

        if (isSliding)
        {
            acceleration = groundAcceleration;
        }
        else if (isGrounded)
        {
            acceleration = moveDirection.sqrMagnitude > 0.01f ? groundAcceleration : groundDeceleration;
        }
        else
        {
            acceleration = moveDirection.sqrMagnitude > 0.01f ? airAcceleration : airDrag;
        }

        Vector3 newHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetHorizontalVelocity, acceleration * Time.fixedDeltaTime);

        playerRigidbody.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
    }

    /// <summary>
    /// Update the coyote time, jump buffers and thw wall jump delays
    /// </summary>
    private void UpdateJumpTimers(Keyboard keyboard)
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        
        wallReattachTimer -= Time.deltaTime;
    }

    /// <summary>
    /// handles all jumps
    /// </summary>
    private void HandleJumping()
    {
        if (!perm_canJump)
        {
            return;
        }

        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        if(jumpReleased && currentVelocity.y > 0f)
        {
            currentVelocity.y *= jumpReleaseMultiplier;
            playerRigidbody.linearVelocity = currentVelocity;
        }

        if(jumpBufferTimer <= 0f)
        {
            return;
        }

        if(coyoteTimer > 0f)
        {
            PerformParkourJump();
            return;
        }

        if (isWallClinging)
        {
            PerformWallJump();
        }
    }

    

    /// <summary>
    /// Do a normal jump with a small forward boost
    /// </summary>
    private void PerformParkourJump()
    {
        if (!perm_canParkourJump)
        {
            return;
        }

        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        // Alek: Honestly, I dont understand how this calculation works. Reddit tells me it works, I tested it so yeah... :/
        currentVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        Vector3 forwardBoost = transform.forward * forwardJumpBoost;

        currentVelocity.x += forwardBoost.x;
        currentVelocity.z += forwardBoost.z;

        playerRigidbody.linearVelocity = currentVelocity;

        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }

    /// <summary>
    /// Pushes the rigidbody up and away from the wall
    /// </summary>
    private void PerformWallJump()
    {
        float upwardSpeed = Mathf.Sqrt(wallJumpHeight * -2f * gravity);

        Vector3 newVelocity = wallNormal * wallJumpForce;
        newVelocity.y = upwardSpeed;

        playerRigidbody.linearVelocity = newVelocity;

        // Reset values
        isWallClinging = false;
        wallClingTimer = 0f;
        wallReattachTimer = wallReattachDelay;
        jumpBufferTimer = 0f;
    }

    /// <summary>
    /// Drain stamina while sprinting and regenerate it when not sprinting
    /// </summary>
    /// <param name="keyboard"></param>
    private void UpdateSprint(Keyboard keyboard)
    {
        if (!perm_canSprint)
        {
            return;
        }

        bool shiftHeld = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        bool movementHeld = movementInput.sqrMagnitude > 0.01f;
        bool canSprint = currentSprintStamina > 0f;
        bool isSprinting = shiftHeld && movementHeld && canSprint;

        if (isSprinting)
        {
            currentMovementSpeed = sprintSpeed;

            currentSprintStamina -= sprintDrainSpeed * Time.deltaTime;
            currentSprintStamina = Mathf.Max(currentSprintStamina, 0f);

            sprintRegenTimer = sprintRegenerationDelay;
        }

        else
        {
            currentMovementSpeed = speed;
            sprintRegenTimer -= Time.deltaTime;

            if(sprintRegenTimer <= 0f)
            {
                currentSprintStamina += sprintRegenerationSpeed * Time.deltaTime;

                currentSprintStamina = Mathf.Min(currentSprintStamina, maxSprintStamina);
            }
        }
    }

    /// <summary>
    /// Check if the player in the air is touching a wall
    /// </summary>
    private void UpdateWallState()
    {
        if (isGrounded || playerRigidbody.linearVelocity.y > 0f)
        {
            isWallClinging = false;
            wallClingTimer = 0f;
            return;
        }

        bool touchingWall = TryFindWall(out RaycastHit wallHit);

        if (!touchingWall)
        {
            isWallClinging = false;
            wallClingTimer = 0f;
            return;
        }

        if (wallReattachTimer > 0f)
        {
            isWallClinging = false;
            return;
        }

        wallNormal = wallHit.normal;
        isWallClinging = true;
        wallClingTimer += Time.deltaTime;
    }

    /// <summary>
    /// Send a raycast around the platyer to find a wall nearby
    /// </summary>
    /// <param name="wallHit"></param>
    /// <returns></returns>
    private bool TryFindWall(out RaycastHit wallHit)
    {
        Vector3 rayOrigin = capsuleCollider.bounds.center;
        
        float rayDistance = capsuleCollider.radius + wallCheckDistance;
        
        if (Physics.Raycast(rayOrigin, transform.forward, out wallHit, rayDistance, wallMask))
        {
            return true;
        }

        if (Physics.Raycast(rayOrigin, -transform.forward, out wallHit, rayDistance, wallMask))
        {
            return true;
        }

        if (Physics.Raycast(rayOrigin, transform.right, out wallHit, rayDistance, wallMask))
        {
            return true;
        }

        if (Physics.Raycast(rayOrigin, -transform.right, out wallHit, rayDistance, wallMask))
        {
            return true;
        }

        wallHit = default;
        return false;
    }

    /// <summary>
    /// Adds a Legacy Animation clip and sets how it behaves when it finishes.
    /// </summary>
    /// <param name="animationClip">The Legacy Animation clip to prepare.</param>
    /// <param name="wrapMode">What the animation should do when it finishes.</param>
    private void SetupAnimationClip(AnimationClip animationClip, WrapMode wrapMode)
    {
        if (playerAnim == null || animationClip == null)
        {
            return;
        }
        
        // Add the clip if it is not already in the Animation component.
        if (playerAnim.GetClip(animationClip.name) == null)
        {
            playerAnim.AddClip(animationClip, animationClip.name);
        }
        
        AnimationState animationState = playerAnim[animationClip.name];

        if (animationState != null)
        {
            animationState.wrapMode = wrapMode;
        }
    }
    
    /// <summary>
    /// Starts, updates and finish the actual sliding
    /// </summary>
    private void UpdateSliding(Keyboard keyboard)
    {
        if (!perm_canSlide)
        {
            return;
        }

        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            
            // Stop sliding when the timer ends or the player jumps
            if (slideTimer <= 0f || !isGrounded)
            {
                StopSliding();
            }

            return;
        }
        
        bool slidePressed = keyboard.leftCtrlKey.wasPressedThisFrame || keyboard.rightCtrlKey.wasPressedThisFrame;
        bool movementKeyHeld = keyboard.wKey.isPressed || keyboard.aKey.isPressed || keyboard.sKey.isPressed || keyboard.dKey.isPressed;
        bool canSlide = slidePressed && movementKeyHeld && isGrounded && slideCooldownTimer <= 0f;

        if (canSlide)
        {
            StartSliding(keyboard);
        }
    }

    private void StartSliding(Keyboard keyboard)
    {
        isSliding = true;
        slideTimer = slideDuration;

        slideDirection = GetSlideDirection(keyboard);
        
        // Shrink the height of the player and keep the bottom part to the floor
        float heightDifference = normalControllerHeight - slideHeight;

        capsuleCollider.height = slideHeight;
        capsuleCollider.center = normalControllerCenter - (Vector3.up * heightDifference * 0.5f);

        // Visually lower the player's visual height
        if(playerCam != null)
        {
            playerCam.transform.localPosition = normalCameraLocalPosition + (Vector3.down * camSlidingHeight);
        }

        PlayAnimation(slideAnimClip, WrapMode.ClampForever);
    }

    /// <summary>
    /// Stops the slide animation and resets the player's height
    /// </summary>
    private void StopSliding()
    {
        isSliding = false;
        slideCooldownTimer = slideCooldown;
        
        capsuleCollider.height = normalControllerHeight;
        capsuleCollider.center = normalControllerCenter;

        if(playerCam != null)
        {
            playerCam.transform.localPosition = normalCameraLocalPosition;
        }

        // The reset animation returns animated objects to their default pose.
        PlayAnimation(resetAnimClip, WrapMode.ClampForever);
    }

    /// <summary>
    /// Get the direction that the player was moving when the slide starts
    /// </summary>
    /// <param name="keyboard"></param>
    /// <returns></returns>
    private Vector3 GetSlideDirection(Keyboard keyboard)
    {
        float horizontalInput = 0f;
        float forwardInput = 0f;

        if (keyboard.aKey.isPressed)
        {
            horizontalInput -= 1f;
        }
        
        if (keyboard.dKey.isPressed)
        {
            horizontalInput += 1f;
        }
        
        if (keyboard.sKey.isPressed)
        {
            forwardInput -= 1f;
        }
        
        if (keyboard.wKey.isPressed)
        {
            forwardInput += 1f;
        }

        Vector3 direction = (transform.right * horizontalInput) + (transform.forward * forwardInput);

        direction = Vector3.ClampMagnitude(direction, 1f);
        
        // Go forward if there is no direction
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        return direction;
    }

    /// <summary>
    /// Plays a Legacy Animation clip from its first frame.
    /// </summary>
    /// <param name="animationClip">The clip to play.</param>
    /// <param name="wrapMode">What the animation should do when it finishes.</param>
    private void PlayAnimation(AnimationClip animationClip, WrapMode wrapMode)
    {
        if (playerAnim == null || animationClip == null)
        {
            return;
        }
        
        AnimationState animationState = playerAnim[animationClip.name];

        if (animationState == null)
        {
            return;
        }
        
        // Restart the animation from the first frame
        animationState.time = 0f;
        animationState.speed = 1f;
        animationState.wrapMode = wrapMode;
        
        playerAnim.Play(animationClip.name, PlayMode.StopAll);
    }

    public void UpdatePause(bool isPaused)
    {
        if(isPaused && WorldController.isGamePaused)
        {
            freezePlayer = true;
            Debug.Log("PlayerController: Player paused");
            return;
        }

        freezePlayer = false;
        Debug.Log("PlayerController: Player unpaused");
    }

    public CameraController GetCameraController()
    {
        return GetComponent<CameraController>();
    }
}
