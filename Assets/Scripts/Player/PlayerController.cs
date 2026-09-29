using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves the player using a CharacterController and Unity's Input System.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -20f;

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

    [Header("Wall Movement")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float maxWallClingTime = 1f;
    [SerializeField] private float wallSlideSpeed = 3f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpHeight = 2.5f;
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private float wallJumpVelocityDecay = 12f;
    [SerializeField] private float wallReattachDelay = 0.25f;

    [Header("Sliding")] 
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideDuration = 1.2f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float slideCooldown = 0.5f;

    [Header("Slide Visuals")]
    [SerializeField] private Transform playerVisuals;

    [SerializeField, Range(0.1f, 1f)] private float slidingHeightMultiplier = 0.5f;

    [Header("Animations")]
    //Sliding
    [SerializeField] private Animation playerAnim;
    [SerializeField] private AnimationClip slideAnimClip;
    [SerializeField] private AnimationClip resetAnimClip; //Used to reset all animations to the default state

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float currentMovementSpeed;
    private float currentSprintStamina;
    private float sprintRegenTimer;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool isWallClinging;
    private float wallClingTimer;
    private float wallReattachTimer;
    private Vector3 wallNormal;
    private Vector3 wallJumpVelocity;

    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;
    
    private float normalControllerHeight;
    private Vector3 normalControllerCenter;
    private Vector3 slideDirection;

    private Vector3 normalVisualScale;
    
    /// <summary>
    /// Get the current sprint stamina as a value between 0 and 1 for the UI
    /// </summary>
    public float SprintStaminaPercent => currentSprintStamina / maxSprintStamina;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        currentMovementSpeed = speed;
        currentSprintStamina = maxSprintStamina;
        
        normalControllerHeight = controller.height;
        normalControllerCenter = controller.center;

        if (playerVisuals != null)
        {
            normalVisualScale = playerVisuals.localScale;
        }
        
        SetupAnimationClip(slideAnimClip, WrapMode.ClampForever);
        SetupAnimationClip(resetAnimClip, WrapMode.ClampForever);
    }

    private void Update()
    {
        // The updated way to get input
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }
        
        UpdateGroundCheck();
        UpdateSliding(keyboard);
        UpdateJumpTimers(keyboard);
        UpdateWallState();
        if (!isSliding) //Make sure you cant jump while sliding
        {
            HandleJumping(keyboard);
        }
        UpdateSprint(keyboard);
        UpdateGravity();
        Walk();
    }

    /// <summary>
    /// Checks if the player is on the floor and adds velocity on the Y axis
    /// </summary>
    private void UpdateGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            // Keep the player on the ground
            velocity.y = -2f;
        }

        if (isWallClinging)
        {
            if (wallClingTimer <= maxWallClingTime)
            {
                //Stop falling
                velocity.y = 0f;
            }
            else
            {
                //Slide down after clinging to the wall for too long
                velocity.y = -wallSlideSpeed;
            }

            return;
        }
        
        velocity.y += gravity * Time.deltaTime;
    }

    /// <summary>
    /// Reads WASD using the Input System and moves the player
    /// </summary>
    private void Walk()
    {
        // Keyboard.current, replacing the legacy Input.GetAxis
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        float horizontalInput = 0f;
        float forwardInput = 0f;

        // Read each movement key using the new Input System
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

        Vector3 moveDirection =
            (transform.right * horizontalInput) +
            (transform.forward * forwardInput);

        // Stops diagonal movement from being faster than forward
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Slowly remove the force from a wall jump
        wallJumpVelocity = Vector3.MoveTowards(wallJumpVelocity, Vector3.zero, wallJumpVelocityDecay * Time.deltaTime);
        
        // Use walking or sprinting speed
        //Vector3 finalMovement = (moveDirection * currentMovementSpeed) + wallJumpVelocity;

        Vector3 horizontalMovement;
        if (isSliding)
        {
            horizontalMovement = slideDirection * slideSpeed;
        }
        else
        {
            horizontalMovement = (moveDirection * currentMovementSpeed) + wallJumpVelocity;
        }

        Vector3 finalMovement = horizontalMovement;
        
        // Add jumping, gravity or the wall sliding
        finalMovement.y = velocity.y;
        
        controller.Move(finalMovement * Time.deltaTime);
    }

    ///<summary>
    /// Check if the player is touching the ground
    /// </summary>
    private void UpdateGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckRadius.position, groundDistance, groundMask);
    }

    /// <summary>
    /// Update the coyote time, jump buffers and thw wall jump delays
    /// </summary>
    /// <param name="keyboard"></param>
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
    /// Choose between normal jump and wall jump
    /// </summary>
    /// <param name="keyboard"></param>
    private void HandleJumping(Keyboard keyboard)
    {
        // If you let go of the space early, you will get a shorter jump
        if (keyboard.spaceKey.wasReleasedThisFrame && velocity.y > 0f)
        {
            velocity.y *= jumpReleaseMultiplier;
        }

        if (jumpBufferTimer <= 0f)
        {
            return;
        }

        if (isWallClinging)
        {
            PerformWallJump();
            return;
        }

        if (coyoteTimer > 0f)
        {
            PerformParkourJump();
        }
    }
    
    /// <summary>
    /// Do a normal jump with forward movement
    /// </summary>
    private void PerformParkourJump()
    {
        //Alek: Honestly, I dont understand how this calculation works. Reddit tells me it works, I tested it so yeah... :/
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        wallJumpVelocity += transform.forward * forwardJumpBoost;

        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }

    /// <summary>
    /// Pushes the player up and away from the wall
    /// </summary>
    private void PerformWallJump()
    {
        velocity.y = Mathf.Sqrt(wallJumpHeight * -2f * gravity);

        wallJumpVelocity = wallNormal * wallJumpForce;

        isWallClinging = false;
        wallClingTimer = 0f;
        wallReattachTimer = wallReattachDelay;
        jumpBufferTimer = 0f;
    }

    /// <summary>
    /// Draining sprint stamina while sprinting and regenerate it if not sprinting
    /// </summary>
    /// <param name="keyboard"></param>
    private void UpdateSprint(Keyboard keyboard)
    {
        bool shiftHeld = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;

        bool movementKeyHeld = keyboard.wKey.isPressed || keyboard.aKey.isPressed || keyboard.sKey.isPressed ||
                               keyboard.dKey.isPressed;

        bool canSprint = currentSprintStamina > 0f;

        bool isSprinting = shiftHeld && movementKeyHeld && canSprint && isGrounded;
        
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

            if (sprintRegenTimer <= 0f)
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
        if (isGrounded)
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
        Vector3 rayOrigin = controller.bounds.center;
        
        float rayDistance = controller.radius + wallCheckDistance;

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
    /// <param name="keyboard"></param>
    private void UpdateSliding(Keyboard keyboard)
    {
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

        controller.height = slideHeight;

        controller.center = normalControllerCenter - (Vector3.up * heightDifference * 0.5f);

        // Visually lower the player while the collider is shortened.
        if (playerVisuals != null)
        {
            Vector3 slidingScale = normalVisualScale;
            slidingScale.y = normalVisualScale.y * slidingHeightMultiplier;
            playerVisuals.localScale = slidingScale;
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
        
        controller.height = normalControllerHeight;
        controller.center = normalControllerCenter;

        if (playerVisuals != null)
        {
            playerVisuals.localScale = normalVisualScale;
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
}
