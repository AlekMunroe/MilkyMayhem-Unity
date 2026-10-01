using UnityEngine;

public class MilkCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            GameObject projectile = other.gameObject;
            if (projectile != null)
            {
                Destroy(projectile);
                ClearSelfCheckpoint();
            }
        }
    }   

    /// <summary>
    /// Marks the checkpoint as cleared. This should trigger other game logic, such as updating the player's progress.
    /// </summary>
    private void ClearSelfCheckpoint()
    {
        Debug.Log("Checkpoint cleared!");
    }
}
