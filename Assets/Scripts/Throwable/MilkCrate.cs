using UnityEngine;

public class MilkCrate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int numOfBounces = 0;
    [SerializeField] private float bounceIntensity = 500f;

    new private Collider collider;
    new private Rigidbody rigidbody;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (numOfBounces > 0)
        {
            ContactPoint contactPoint = collision.contacts[0];
            rigidbody.AddForce(FindForceDirection(contactPoint) * bounceIntensity);
            numOfBounces--;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when colliding with an object. Calculates the direction of the force to bounce.
    /// </summary>
    /// <param name="contactPoint"></param>
    /// <returns></returns>
    private Vector3 FindForceDirection(ContactPoint contactPoint)
    {
        Vector3 forceDirection = transform.position - contactPoint.point;
        return forceDirection;
    }
}
