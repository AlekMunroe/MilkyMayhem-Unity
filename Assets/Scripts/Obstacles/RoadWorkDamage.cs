using UnityEngine;

public class RoadWorkDamage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseDamage = 30f;

    private void OnTriggerStay(Collider other)
    {
        //Only the player should have the "Player" tag.
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.CutHealth(baseDamage);
        }
    }
}
