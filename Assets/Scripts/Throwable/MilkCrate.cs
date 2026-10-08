using UnityEngine;

public class MilkCrate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int numOfBounces = 0;
    [SerializeField] private float bounceIntensity = 500f;
    [SerializeField] private float bounceDecayMaxCooldown = 0.1f;

    private float bounceDecayCooldown;
    new private Collider collider;
    new private Rigidbody rigidbody;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (bounceDecayCooldown > 0)
        {
            bounceDecayCooldown -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (numOfBounces <= 0 && bounceDecayCooldown <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            ContactPoint contactPoint = collision.contacts[0];
            rigidbody.AddForce(FindForceDirection(contactPoint) * bounceIntensity);
            numOfBounces--;
            if (bounceDecayCooldown <= 0)
            {
                bounceDecayCooldown = bounceDecayMaxCooldown;
            }
        }
    }

    /// <summary>
    /// Called when colliding with an object. Calculates the direction of the force to bounce.
    /// </summary>
    private Vector3 FindForceDirection(ContactPoint contactPoint)
    {
        Vector3 forceDirection = transform.position - contactPoint.point;
        return forceDirection;
    }
}
