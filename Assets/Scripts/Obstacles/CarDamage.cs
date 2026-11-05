using System.Collections;
using UnityEngine;

public class CarDamage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseDamage = 25f;
    [Tooltip("Multiplies the damage dealt to the player based on the speed of the collision")]
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float minCollisionSpeed = 5f;
    [SerializeField] private float collisionKnockbackForce = 10f;
    [SerializeField] private float collisionVelocityThreshold = 20f;
    [SerializeField, Range(0f, 1f)] private float frontCollisionThreshold = 0.5f;

    private void OnCollisionEnter(Collision other)
    {   
        //Only the player should have the "Player" tag.
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if(playerController == null)
            {
                return;
            }

            ContactPoint contact = other.GetContact(0);
            Vector3 collisionNormal = contact.normal;
            Vector3 collisionVelocity = other.relativeVelocity;

            if (Vector3.Dot(collisionNormal, Vector3.up) > 0.5f)
            {
                //The player is colliding with the top of the car, so we don't want to apply damage.
                return;
            }
            if (Vector3.Dot(collisionNormal, Vector3.down) > 0.5f)
            {
                //How would the player even get under the car???
                return;
            }

            Vector3 directionToCollision = contact.point - transform.position;
            directionToCollision.y = 0f;

            bool hitFrontOfCar = false;

            if(directionToCollision.sqrMagnitude > 0.001f)
            {
                float frontDot = Vector3.Dot(transform.forward, directionToCollision.normalized);
                hitFrontOfCar = frontDot >= frontCollisionThreshold;
            }

            if(collisionVelocity.magnitude < collisionVelocityThreshold && !hitFrontOfCar)
            {
                // The player is probably glancing off the side of the car
                return;
            }

            float collisionSpeed = other.relativeVelocity.magnitude;
            if (collisionSpeed > minCollisionSpeed)
            {
                float damage = baseDamage + (collisionSpeed * damageMultiplier);

                playerController.ModifyHealth(damage);
                playerController.ExternalMovement((-collisionNormal + Vector3.up) * collisionKnockbackForce);
            }
        }
    }
}
