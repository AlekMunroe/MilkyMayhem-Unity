using UnityEngine;
using UnityEngine.InputSystem;

public class MilkThrowing : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float throwStrength = 500f;
    [SerializeField] private float playerVelocityMultiplier = 25f;
    [SerializeField] private float throwMaxCooldown = 2f;
    [SerializeField] private Vector3 projectileSpawnPointOffset = new Vector3(0, 0, 0);

    [Header("Randomness Settings")]
    [SerializeField] private float randomRange = 5f;
    
    [Header("Projectile Object")]
    [SerializeField] private Rigidbody projectilePrefab;

    [Header("Player Object")]
    [SerializeField] private GameObject playerObject;

    private Vector3 projectileSpawnPoint;
    private float throwCooldown;
    private Vector3 playerVelocity;
    private CharacterController playerController;


    private void Awake()
    {
        //References the player object's CharacterController component.
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        playerController = playerObject.GetComponent<CharacterController>();
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
    /// Update logic for checking if the player is attempting to throw the milk projectile. Also manages the cooldown timer for throwing.
    /// </summary>
    private void UpdateThrowing(Mouse mouse)
    {
        bool throwPressed = mouse.leftButton.wasPressedThisFrame;
        if (throwCooldown > 0)
        {
            throwCooldown -= Time.deltaTime;
        }
        
        //Temporary fix for throwing while paused. Will be replaced with a proper event system later.
        if (throwPressed && throwCooldown <= 0 && Time.timeScale > 0)
        {
            ThrowMilk();
        }
    }

    /// <summary>
    /// Handles the logic for throwing the milk projectile.
    /// </summary>
    private void ThrowMilk()
    {
        Vector3 projectileFinalOffset = transform.forward * projectileSpawnPointOffset.z + transform.up * projectileSpawnPointOffset.y + transform.right * projectileSpawnPointOffset.x;
        projectileSpawnPoint = transform.position + projectileFinalOffset;
        Quaternion projectileRotation = Random.rotation;
        float randomRotation = Random.Range(-randomRange,randomRange);

        //Reads the velocity returned by the CharacterController.
        playerVelocity = playerController.velocity;
        Debug.Log(playerVelocity);

        Rigidbody spawnedProjectile = Instantiate(projectilePrefab, projectileSpawnPoint, projectileRotation);

        spawnedProjectile.AddForce(transform.forward * throwStrength + playerVelocity * playerVelocityMultiplier);
        spawnedProjectile.AddTorque(randomRange,randomRange,randomRange);

        throwCooldown = throwMaxCooldown;
    }
}
