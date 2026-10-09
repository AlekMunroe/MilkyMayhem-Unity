using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class MilkThrowing : MonoBehaviour
{
    [Header("Throwing")]
    [SerializeField, Min(0f)] private float throwStrength = 12f;
    [SerializeField, Min(0f)] private float playerVelocityMultiplier = 1f;
    [SerializeField, Min(0f)] private float throwMaxCooldown = 0.5f;
    [SerializeField, Min(0f)] private float projectileLifetime = 10f;

    [Tooltip("The local position in front of the camera where the milk is created")]
    [SerializeField] private Vector3 projectileSpawnPointOffset = new Vector3(0f, -0.15f, 1f);

    [Header("Randomness")]
    [SerializeField, Min(0f)] private float randomTorqueStrength = 1f;
    
    [Header("Projectile")]
    [Tooltip("Assign the Rigidbody from the MilkCrate prefab")]
    [SerializeField] private Rigidbody projectilePrefab;

    [Header("Player")]
    [Tooltip("Optional: The script will search its parents then create PlayerController.Instance")]
    [SerializeField] private PlayerController playerController;

    private Rigidbody playerRigidbody;
    private Collider[] playerColliders;
    private float throwCooldown;


    private void Start()
    {
        if(playerController == null)
        {
            playerController = PlayerController.Instance;
        }

        if(playerController == null)
        {
            Debug.LogError("MilkThrowing: No PlayerController could be found.");
            
            enabled = false;
            return;
        }

        playerRigidbody = playerController.GetComponent<Rigidbody>();

        if(playerRigidbody == null)
        {
            Debug.LogError("MilkThrowing: The player doesnt have a Rigidbody");

            enabled = false;
            return;
        }

        playerColliders = playerController.GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        UpdateThrowing(mouse);
    }

    /// <summary>
    /// Checks for input and updates the cooldown
    /// </summary>
    /// <param name="mouse">The current mouse input</param>
    private void UpdateThrowing(Mouse mouse)
    {
        if(throwCooldown > 0f)
        {
            throwCooldown -= Time.deltaTime;
        }

        // Dont allow throwing if the game is paused or the cursor is unlocked for a menu
        if (WorldController.isGamePaused || (Cursor.lockState != CursorLockMode.Locked))
        {
            return;
        }

        // Dont allow clicks on visible UI
        bool pointOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if(Cursor.visible && pointOverUI)
        {
            return;
        }

        if (throwCooldown > 0f)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame && throwCooldown <= 0f)
        {
            ThrowMilk();
        }
        else if (mouse.rightButton.wasPressedThisFrame)
        {
            PlaceMilk();
        }
    }

    /// <summary>
    /// Handles the logic for throwing the milk projectile.
    /// </summary>
    private void ThrowMilk()
    {
        // Convert the local offset into a world position
        Vector3 projectileSpawnPoint = transform.TransformPoint(projectileSpawnPointOffset);

        // Match the projectiles start rotation to the camera
        Quaternion projectileRotation = transform.rotation;

        Rigidbody spawnedProjectile = Instantiate(projectilePrefab, projectileSpawnPoint, projectileRotation);

        IgnorePlayerCollisions(spawnedProjectile);

        // Carry some players movement into the throw
        spawnedProjectile.linearVelocity = playerRigidbody.linearVelocity * playerVelocityMultiplier;

        // Apply a single throw impulse
        spawnedProjectile.AddForce(transform.forward * throwStrength, ForceMode.Impulse);

        // Add random rotation
        Vector3 randomTorque = Random.insideUnitSphere * randomTorqueStrength;

        spawnedProjectile.AddTorque(randomTorque, ForceMode.Impulse);

        // Remove the missed projectiles after its time has expired
        Destroy(spawnedProjectile.gameObject, projectileLifetime);

        throwCooldown = throwMaxCooldown;
    }

    /// <summary>
    /// Places a milk in front of the player without throwing it
    /// </summary>
    private void PlaceMilk()
    {
        Vector3 projectileSpawnPoint = transform.TransformPoint(projectileSpawnPointOffset);

        Quaternion projectileRotation = transform.rotation;

        Rigidbody placedProjectile = Instantiate(projectilePrefab, projectileSpawnPoint, projectileRotation);

        IgnorePlayerCollisions(placedProjectile);

        // No rotation or movement
        placedProjectile.linearVelocity = Vector3.zero;
        placedProjectile.angularVelocity = Vector3.zero;

        MilkCrate milkCrate = placedProjectile.GetComponent<MilkCrate>();

        if (milkCrate != null)
        {
            milkCrate.DisableBouncing();
        }
        else
        {
            Debug.LogWarning(
                "MilkThrowing: The placed projectile has no MilkCrate component.");
        }

        Destroy(placedProjectile.gameObject, projectileLifetime);

        throwCooldown = throwMaxCooldown;
    }

    /// <summary>
    /// Stops a new projectile from colliding with the player
    /// </summary>
    /// <param name="spawnedProjectile">The rigidbody that belongs to the projectile</param>
    private void IgnorePlayerCollisions(Rigidbody spawnedProjectile)
    {
        Collider[] projectileColliders = spawnedProjectile.GetComponentsInChildren<Collider>();

        foreach(Collider projectileCollider in projectileColliders)
        {
            foreach(Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(projectileCollider, playerCollider);
            }
        }
    }
}
