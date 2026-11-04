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

    private void OnCollisionEnter(Collision other)
    {   
        //Only the player should have the "Player" tag.
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 collisionNormal = other.GetContact(0).normal;
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
            if (collisionVelocity.magnitude < collisionVelocityThreshold && collisionNormal != Vector3.forward)
            {
                //The player is glancing off the car probably, so we don't want to apply damage.
                //The car should also be moving forward always, so based on this assumption, the front should deal damage no matter what.
                return;
            }

            float collisionSpeed = other.relativeVelocity.magnitude;
            if (collisionSpeed > minCollisionSpeed)
            {
                float damage = baseDamage + (collisionSpeed * damageMultiplier);
                PlayerController.Instance.ModifyHealth(damage);
                PlayerController.Instance.ExternalMovement((-collisionNormal + Vector3.up) * collisionKnockbackForce);
            }
        }
    }
}
