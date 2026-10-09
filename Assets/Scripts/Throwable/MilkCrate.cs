using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MilkCrate : MonoBehaviour
{
    [Header("Bounce")]
    [SerializeField, Min(0)] private int maximumBounces = 2;
    [SerializeField, Range(0f, 1f)] private float retainedBounceVelocity = 0.7f;
    [SerializeField, Min(0f)] private float minimumBounceSpeed = 1f;
    [SerializeField, Min(0f)] private float bounceCooldown = 0.1f;
    [SerializeField, Range(0f, 1f)] private float retainedAngularVelocity = 0.5f;

    [Header("Lifetime")]
    [SerializeField, Min(0.1f)] private float maximumLifetime = 10f;

    [Header("Collision")]
    [SerializeField] private LayerMask bounceSurfaceMask = ~0;

    private Rigidbody projectileRigidbody;
    private int remainingBounces;
    private float nextAllowedBounceTime;
    private Vector3 velocityBeforePhysicsUpdate;

    [Header("Delivery")]
    [SerializeField, Min(0f)] private float breakingImpactSpeed = 15f;

    public bool IsBroken { get; private set; }
    public bool IsLanded { get; private set; }

    public float CurrentSpeed
    {
        get
        {
            if(projectileRigidbody == null)
            {
                return 0f;
            }

            return projectileRigidbody.linearVelocity.magnitude;
        }
    }

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody>();

        if(projectileRigidbody == null)
        {
            Debug.LogError("MilkCrate: The projectile requires a Rigidbody");

            enabled = false;
            return;
        }

        remainingBounces = maximumBounces;
    }

    private void Start()
    {
        // Remove every projectile that misses every surface and checkpoint
        Destroy(gameObject, maximumLifetime);
    }

    private void FixedUpdate()
    {
        if(projectileRigidbody != null)
        {
            velocityBeforePhysicsUpdate = projectileRigidbody.linearVelocity;
        }
    }

    /// <summary>
    /// Add a controlled bounce when the crate hits a surface
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionRoot = collision.transform.root.gameObject;

        if(collision.relativeVelocity.magnitude >= breakingImpactSpeed)
        {
            MarkBroken();
        }

        // Stop from collising with the player or other projectiles
        if(collisionRoot.CompareTag("Player") || collisionRoot.CompareTag("Projectile"))
        {
            return;
        }

        // Checkpoints handle through MilkCheckpoint
        if (collisionRoot.CompareTag("Checkpoint"))
        {
            return;
        }

        // Ignore if the surfaces are outside the selected layers
        if (!IsLayerInBounceMask(collision.gameObject.layer))
        {
            return;
        }

        // Stop one impact from being counted multiple times
        if(Time.time < nextAllowedBounceTime)
        {
            return;
        }

        // If no bounces are left, use normal Rigidbody physics to stop the crate
        if(remainingBounces <= 0)
        {
            return;
        }

        Vector3 impactVelocity = velocityBeforePhysicsUpdate;

        if (impactVelocity.magnitude < minimumBounceSpeed)
        {
            remainingBounces = 0;
            return;
        }

        ContactPoint contactPoint = collision.GetContact(0);

        Vector3 reflectedVelocity = Vector3.Reflect(impactVelocity, contactPoint.normal);

        projectileRigidbody.linearVelocity = reflectedVelocity * retainedBounceVelocity;

        // Reduce a lot of rolling while keeping some rotation
        projectileRigidbody.angularVelocity *= retainedAngularVelocity;

        remainingBounces--;

        nextAllowedBounceTime = Time.time + bounceCooldown;
    }

    /// <summary>
    /// Check if a layer is in the bounce surface mask
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private bool IsLayerInBounceMask(int layer)
    {
        int layerValue = 1 << layer;

        return (bounceSurfaceMask.value & layerValue) != 0;
    }

    /// <summary>
    /// Mark the milk as broken so it cannot complete its delivery
    /// </summary>
    public void MarkBroken()
    {
        IsBroken = true;
    }

    /// <summary>
    /// Stops the crate and place it at the checkpoints landing position
    /// </summary>
    /// <param name="landingPoint">Where the crate should be placed</param>
    public void LandAtCheckpoint(Transform landingPoint)
    {
        if(IsBroken || IsLanded)
        {
            return;
        }

        projectileRigidbody.linearVelocity = Vector3.zero;
        projectileRigidbody.angularVelocity = Vector3.zero;
        projectileRigidbody.isKinematic = true;

        if(landingPoint != null)
        {
            transform.SetPositionAndRotation(landingPoint.position, landingPoint.rotation);
        }

        IsLanded = true;
    }

    /// <summary>
    /// Disables bounding for a crate that is placed and not thrown
    /// </summary>
    public void DisableBouncing()
    {
        remainingBounces = 0;

        if(projectileRigidbody != null)
        {
            projectileRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
