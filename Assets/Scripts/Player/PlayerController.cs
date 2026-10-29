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
    [Header("<color=grey>These are only to be used perminantly, do not reference these anywhere else</color>")]
    [SerializeField] private bool perm_canSprint;
    [SerializeField] private bool perm_canJump;
    [SerializeField] private bool perm_canParkourJump;
    [SerializeField] private bool perm_canGroundMove;
    [SerializeField] private bool perm_canWallMove;
    [SerializeField] private bool perm_canAirMove;
    [SerializeField] private bool perm_canSlide;

    [Header("Movement")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float gravity = -45f;
    [SerializeField] private float airStrafeSpeed = 1f;
    [SerializeField] private float groundAcceleration = 140f;
    [SerializeField] private float groundDeceleration = 190f;

    private Vector2 movementInput;

    [Header("Milk store movement")]
    [SerializeField, Min(0f)] private float milkStoreMovementSpeed = 7f;

    private bool isInMilkStore;
    public bool IsInMilkStore => isInMilkStore;

    [Header("Sprinting")]
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float maxSprintStamina = 2f;
    [SerializeField] private float sprintDrainSpeed = 1f;
    [SerializeField] private float sprintRegenerationSpeed = 0.75f;
    [SerializeField] private float sprintRegenerationDelay = 1f;


    [Header("Parkour Jump")]
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float forwardJumpBoost = 3f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Surface Check")]
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float maxGroundAngle = 55f;
    [SerializeField] private float minWallAngle = 70f;
    [SerializeField] private float maxWallAngle = 110f;

    [SerializeField, Range(0.1f, 1f)]
    private float jumpReleaseMultiplier = 0.5f;

    private bool jumpReleased;

    [Header("Wall Movement")]
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float maxWallClingTime = 0.5f;
    [SerializeField] private float wallClingBoost = 20f;
    [SerializeField] private float wallStickForce = 10f;
    [SerializeField] private float wallSlideSpeed = 5f;
    [SerializeField] private float wallJumpForwardForce = 8f;
    [SerializeField] private float wallDetachSpeed = 2f;
    [SerializeField] private float wallMomentumHalfLife = 20f;

    [Header("Wall Climbing")]
    [SerializeField] private bool canWallClimb = false;
    [SerializeField] private float wallClimbVerticalBoost = 7.5f;
    [SerializeField] private float wallClimbBoost = 5f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpHeight = 6f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallReattachDelay = 0.3f;

    [Header("Sliding")]
    [SerializeField] private float slideDuration = 1.2f;
    [Tooltip("Value is read as x:y")]
    [SerializeField] private Vector2 momentumConversionRatio = new Vector2(1, 10); //Value is read as x:y
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float slideHeightCheckDistance = 1f;
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

    private bool isWallClinging;
    private float wallClingTimer;
    private float wallReattachTimer;
    private Vector3 wallNormal;
    private Vector3 storedWallNormal;
    private Vector3 storedWallMomentum;
    private Vector3 maxWallMomentum;
    private Vector3 minWallMomentum;

    private bool isSliding;
    private float maxFallingVelocity;
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
            if (maxSprintStamina <= 0f)
            {
                return 0f;
            }

            return currentSprintStamina / maxSprintStamina;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
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

        if (playerCam != null)
        {
            normalCameraLocalPosition = playerCam.transform.localPosition;
        }

        SetupAnimationClip(slideAnimClip, WrapMode.ClampForever);
        SetupAnimationClip(resetAnimClip, WrapMode.ClampForever);
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RoundCompleted += HandleRoundCompleted;
        }
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
        HandleJumping();
        UpdateGravity();
        if (isGrounded)
        {
            UpdateGroundMovement();
        }
        else if (isWallClinging)
        {
            UpdateWallMovement();
        }
        else
        {
            UpdateAirMovement();
        }

        // The released input has now been delt with by the physics update
        jumpReleased = false;
    }

    ///<summary>
    /// Check if the player is touching the ground
    /// </summary>
    private void UpdateGroundCheck()
    {
        // https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Bounds.html

        Bounds playerBounds = capsuleCollider.bounds;

        // A sphere slightly smaller than the capsuleCollider
        float sphereRadus = Mathf.Min(playerBounds.extents.x, playerBounds.extents.z) * 0.9f;

        // Cast from centre to a little below the capsuleCollider
        float sphereCastDistance = playerBounds.extents.y - sphereRadus + groundCheckDistance;

        bool surfaceFound = Physics.SphereCast(playerBounds.center, sphereRadus, Vector3.down, out RaycastHit groundHIt, sphereCastDistance, environmentMask, QueryTriggerInteraction.Ignore);

        if (!surfaceFound)
        {
            isGrounded = false;
            return;
        }

        float surfaceAngle = Vector3.Angle(groundHIt.normal, Vector3.up);

        isGrounded = surfaceAngle <= maxGroundAngle;
    }

    /// <summary>
    /// Apply a custom gravity to the Rigidbody
    /// </summary>
    private void UpdateGravity()
    {
        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        if (isGrounded && currentVelocity.y < 0f)
        {
            // A small force down to keep the player on the floor
            currentVelocity.y = -2f;
        }
        else if (isWallClinging)
        {
            if (wallClingTimer <= maxWallClingTime)
            {
                // Stop the player from falling temporarily
                //Note from Theo: After some testing and feedback from other members of the team, I have changed the wall cling to be a small boost upwards instead of just stopping, similar to Titanfall 2.
                float finalClingBoost = wallClingBoost * (maxWallClingTime - wallClingTimer);
                currentVelocity.y = finalClingBoost;
                canWallClimb = true;
            }
            else
            {
                // Slide down
                float finalWallSlideSpeed = wallSlideSpeed * (wallClingTimer - maxWallClingTime);
                if (finalWallSlideSpeed > wallSlideSpeed)
                {
                    finalWallSlideSpeed = wallSlideSpeed;
                }
                currentVelocity.y = -finalWallSlideSpeed;
                canWallClimb = false;
            }
        }
        else
        {
            // Normal graity while in the air
            currentVelocity.y += gravity * Time.fixedDeltaTime;
        }

        if (isGrounded || isSliding)
        {
            canWallClimb = false;
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
    private void UpdateGroundMovement()
    {
        if (!perm_canGroundMove)
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
            targetHorizontalVelocity = slideDirection;
        }
        else
        {
            targetHorizontalVelocity = moveDirection * currentMovementSpeed;
        }

        float acceleration;

        if (isGrounded)
        {
            acceleration = moveDirection.sqrMagnitude > 0.01f ? groundAcceleration : groundDeceleration;
        }
        else
        {
            acceleration = groundAcceleration;
        }

        Vector3 newHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetHorizontalVelocity, acceleration * Time.fixedDeltaTime);

        playerRigidbody.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
    }

    public void SetMilkStoreMovement(bool enabled)
    {
        isInMilkStore = enabled;

        currentMovementSpeed = enabled ? milkStoreMovementSpeed : speed;
    }

    /// <summary>
    /// Reads WASD inputs and handles movement while the player is on a wall.
    /// </summary>
    private void UpdateWallMovement()
    {
        if (!perm_canWallMove || isInMilkStore)
        {
            return;
        }

        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        // Gradually reduce momentum when attaching to a wall
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 horizontalWallNormal = Vector3.ProjectOnPlane(wallNormal, Vector3.up).normalized;

        // Keep momentum moving along the wall
        Vector3 velocityAlongWall = Vector3.ProjectOnPlane(horizontalVelocity, horizontalWallNormal);

        float momentumMultiplier = Mathf.Pow(0.5f, Time.fixedDeltaTime / wallMomentumHalfLife);

        velocityAlongWall *= momentumMultiplier;

        velocityAlongWall += wallNormal * -wallStickForce;

        playerRigidbody.linearVelocity = new Vector3(velocityAlongWall.x, currentVelocity.y, velocityAlongWall.z);
    }


    /// <summary>
    /// Reads WASD inputs and handles movement in the air, such as air strafing.
    /// </summary>
    private void UpdateAirMovement()
    {
        if (!perm_canAirMove)
        {
            return;
        }

        // Keep wall jump direction
        if (wallReattachTimer > 0f)
        {
            return;
        }

        Vector3 moveDirection = (transform.right * movementInput.x) + (transform.forward * movementInput.y);
        Vector3 projectVelocity = Vector3.Project(playerRigidbody.linearVelocity, moveDirection);

        bool isVectorAway = Vector3.Dot(moveDirection, projectVelocity) < 0f;
        if (projectVelocity.magnitude < airStrafeSpeed || isVectorAway)
        {
            Vector3 idealVelocity = moveDirection.normalized * currentMovementSpeed;
            if (!isVectorAway)
            {
                idealVelocity = Vector3.ClampMagnitude(idealVelocity, airStrafeSpeed - projectVelocity.magnitude);
            }
            else
            {
                idealVelocity = Vector3.ClampMagnitude(idealVelocity, airStrafeSpeed + projectVelocity.magnitude);
            }
            playerRigidbody.linearVelocity += idealVelocity;
        }
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
        Keyboard keyboard = Keyboard.current;

        if (!perm_canJump || isInMilkStore)
        {
            return;
        }

        Vector3 currentVelocity = playerRigidbody.linearVelocity;

        if (jumpReleased && currentVelocity.y > 0f)
        {
            currentVelocity.y *= jumpReleaseMultiplier;
            playerRigidbody.linearVelocity = currentVelocity;
        }

        if (jumpBufferTimer <= 0f)
        {
            return;
        }

        if (coyoteTimer > 0f)
        {
            PerformParkourJump();
            return;
        }

        if (isWallClinging)
        {
            PerformWallJump(keyboard);
        }
    }



    /// <summary>
    /// Do a normal jump with a small forward boost
    /// </summary>
    private void PerformParkourJump()
    {
        if (!perm_canParkourJump || isInMilkStore)
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
    private void PerformWallJump(Keyboard keyboard)
    {
        float upwardSpeed = Mathf.Sqrt(wallJumpHeight * -2f * gravity);

        // Add momentum and push the player away from the wall
        Vector3 newVelocity = playerRigidbody.linearVelocity;
        newVelocity += wallNormal * wallJumpForce;
        newVelocity.y = upwardSpeed;

        if (keyboard.wKey.isPressed)
        {
            newVelocity += transform.forward * wallJumpForwardForce;
        }

        if (keyboard.sKey.isPressed)
        {
            newVelocity -= transform.forward * wallJumpForwardForce;
        }

        playerRigidbody.linearVelocity = newVelocity;

        // Reset values
        isWallClinging = false;
        canWallClimb = false;
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
        if (!perm_canSprint || isInMilkStore)
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
        bool touchingWall = TryFindWall(out RaycastHit wallHit);
        wallNormal = wallHit.normal;
        if ((touchingWall || canWallClimb) && storedWallNormal == Vector3.zero)
        {
            storedWallNormal = wallNormal;
        }
        else if (!canWallClimb)
        {
            storedWallNormal = Vector3.zero;
        }

        if (touchingWall && storedWallMomentum == Vector3.zero)
        {
            storedWallMomentum = playerRigidbody.linearVelocity;
        }
        else
        {
            storedWallMomentum = Vector3.zero;
        }

        if (isGrounded)
        {
            isWallClinging = false;
            wallClingTimer = 0f;
            return;
        }

        if (!touchingWall)
        {
            if (canWallClimb)
            {
                WallClingClimb();
            }
            isWallClinging = false;
            wallClingTimer = 0f;
            return;
        }

        // Detach from wall

        if (wallReattachTimer > 0f)
        {
            isWallClinging = false;
            return;
        }

        Vector3 moveDirection = (transform.right * movementInput.x) + (transform.forward * movementInput.y);

        Vector3 horizontalWallNormal = Vector3.ProjectOnPlane(wallNormal, Vector3.up).normalized;

        if (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.leftCtrlKey.isPressed)
        {
            Vector3 currentVelocity = playerRigidbody.linearVelocity;
            Vector3 detachVelocity = horizontalWallNormal * wallDetachSpeed;

            detachVelocity.y = currentVelocity.y;

            playerRigidbody.linearVelocity = detachVelocity;

            isWallClinging = false;
            canWallClimb = false;
            wallClingTimer = 0f;
            wallReattachTimer = wallReattachDelay;
            return;
        }

        isWallClinging = true;
        wallClingTimer += Time.deltaTime;
    }


    /// <summary>
    /// Handles the climbing action while the player clings over a wall's height.
    /// </summary>
    /// <param name="wallNormal"> The normal vector of the wall being clung to. </param>
    private void WallClingClimb()
    {
        if (playerRigidbody.linearVelocity.y <= 0f)
        {
            playerRigidbody.linearVelocity -= storedWallNormal * wallClimbBoost;
            playerRigidbody.linearVelocity += Vector3.up * wallClimbVerticalBoost;
            canWallClimb = false;
        }
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

        // Seperate direction wall detection
        // I am using multiple if statements for readability
        if (TryFindWallInDirection(rayOrigin, transform.forward, rayDistance, out wallHit))
        {
            return true;
        }

        if (TryFindWallInDirection(rayOrigin, -transform.forward, rayDistance, out wallHit))
        {
            return true;
        }

        if (TryFindWallInDirection(rayOrigin, transform.right, rayDistance, out wallHit))
        {
            return true;
        }

        if (TryFindWallInDirection(rayOrigin, -transform.right, rayDistance, out wallHit))
        {
            return true;
        }

        wallHit = default;
        return false;
    }

    /// <summary>
    /// Check if a surface in the direction is a wall based on the angle.
    /// </summary>
    /// <param name="rayOrigin">Starting point</param>
    /// <param name="rayDirection">Direction to check</param>
    /// <param name="rayDistance">Max detection distance</param>
    /// <param name="wallHit"></param>
    /// <returns>True if the surface is in range of the wall angle range</returns>
    private bool TryFindWallInDirection(Vector3 rayOrigin, Vector3 rayDirection, float rayDistance, out RaycastHit wallHit)
    {
        bool surfaceFound = Physics.Raycast(rayOrigin, rayDirection, out wallHit, rayDistance, environmentMask, QueryTriggerInteraction.Ignore);

        if (!surfaceFound)
        {
            return false;
        }

        float surfaceAngle = Vector3.Angle(wallHit.normal, Vector3.up);

        return surfaceAngle >= minWallAngle && surfaceAngle <= maxWallAngle;
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
        if (!perm_canSlide || isInMilkStore)
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

            Bounds playerBounds = capsuleCollider.bounds;

            // A sphere slightly smaller than the capsuleCollider
            float sphereRadus = Mathf.Min(playerBounds.extents.x, playerBounds.extents.z) * 0.9f;

            // Cast from centre to a little above the capsuleCollider
            float sphereCastDistance = playerBounds.extents.y - sphereRadus + slideHeightCheckDistance;

            bool surfaceFound = Physics.SphereCast(playerBounds.center, sphereRadus, Vector3.up, out RaycastHit ceilingHIt, sphereCastDistance, environmentMask, QueryTriggerInteraction.Ignore);

            // Stop sliding when the timer ends or the player jumps but not while theyre under an object while sliding
            if ((slideTimer <= 0f || !isGrounded) && !surfaceFound)
            {
                StopSliding();
                maxFallingVelocity = 0f;
            }

            return;
        }

        bool slidePressed = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;

        if (slidePressed && !isGrounded)
        {
            SlideBuffer();
        }

        bool canSlide = slidePressed && isGrounded && slideCooldownTimer <= 0f;

        if (canSlide)
        {
            slideDirection = GetSlideDirection();
            if (slideDirection.sqrMagnitude < 0.01f)
            {
                return;
            }

            StartSliding();
        }
    }

    private void StartSliding()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // Shrink the height of the player and keep the bottom part to the floor
        float heightDifference = normalControllerHeight - slideHeight;

        capsuleCollider.height = slideHeight;
        capsuleCollider.center = normalControllerCenter - (Vector3.up * heightDifference * 0.5f);

        // Visually lower the player's visual height
        if (playerCam != null)
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

        if (playerCam != null)
        {
            playerCam.transform.localPosition = normalCameraLocalPosition;
        }

        // The reset animation returns animated objects to their default pose.
        PlayAnimation(resetAnimClip, WrapMode.ClampForever);
    }

    private void SlideBuffer()
    {
        float fallingVelocity = playerRigidbody.linearVelocity.y;
        if (fallingVelocity < 0 && fallingVelocity < maxFallingVelocity)
        {
            maxFallingVelocity = fallingVelocity;
        }
    }

    /// <summary>
    /// Get the direction that the player was moving when the slide starts
    /// </summary>
    /// <param name="keyboard"></param>
    /// <returns></returns>
    private Vector3 GetSlideDirection()
    {
        //Momentum conversion for vertical momentum to horizontal momentum

        Vector3 direction = playerRigidbody.linearVelocity;

        float conversionRatio = Mathf.Abs(maxFallingVelocity * momentumConversionRatio.x / momentumConversionRatio.y);
        Debug.Log(conversionRatio);
        if (conversionRatio < 1)
        {
            conversionRatio = 1;
        }
        Vector3 horizontalDirection = new Vector3(direction.x * conversionRatio, 0, direction.z * conversionRatio);

        return horizontalDirection;
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
        if (isPaused && WorldController.isGamePaused)
        {
            freezePlayer = true;
            return;
        }

        freezePlayer = false;
    }

    private void HandleRoundCompleted()
    {
        freezePlayer = true;
        movementInput = Vector2.zero;

        playerRigidbody.linearVelocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }

    public CameraController GetCameraController()
    {
        return GetComponent<CameraController>();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RoundCompleted -= HandleRoundCompleted;
        }
    }
}