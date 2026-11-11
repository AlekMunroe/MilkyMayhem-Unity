using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerRespawnTrigger : MonoBehaviour
{
    [Header("Fallback")]
    [SerializeField] private Transform fallbackSpawnPoint;

    [Header("Respawning")]
    [SerializeField, Min(0f)] private float respawnHeightOffset = 0.1f;
    [SerializeField, Min(0.05f)] private float safePositionRecordTime = 0.25f;

    private PlayerController playerController;

    private Vector3 lastSafePosition;
    private Quaternion lastSafeRotation;
    private float recordTimer;
    private bool hasSafePosition;

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void Start()
    {
        if (playerController == null)
        {
            playerController = PlayerController.Instance;
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerRespawnTrigger: PlayerController not found");

            enabled = false;
            return;
        }

        SetFallbackPosition();
    }

    private void FixedUpdate()
    {
        recordTimer -= Time.fixedDeltaTime;

        if(recordTimer > 0f || !playerController.IsGrounded)
        {
            return;
        }

        recordTimer = safePositionRecordTime;

        // Set the safe location
        lastSafePosition = playerController.transform.position;
        lastSafeRotation = Quaternion.Euler(0f, playerController.transform.eulerAngles.y, 0f);

        hasSafePosition = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController enteredPlayer = other.GetComponentInParent<PlayerController>();

        if (enteredPlayer == null || enteredPlayer != playerController)
        {
            return;
        }

        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        Vector3 respawnPosition;
        Quaternion respawnRotation;

        // Choose the respawn location
        if (hasSafePosition)
        {
            respawnPosition = lastSafePosition;
            respawnRotation = lastSafeRotation;
        }
        else if(fallbackSpawnPoint != null)
        {
            respawnPosition = fallbackSpawnPoint.position;
            respawnRotation = fallbackSpawnPoint.rotation;
        }
        else
        {
            respawnPosition = playerController.transform.position;
            respawnRotation = playerController.transform.rotation;
        }

        // Add the offset
        respawnPosition += Vector3.up * respawnHeightOffset;

        playerController.RespawnAt(respawnPosition, respawnRotation);
    }

    private void SetFallbackPosition()
    {
        if (fallbackSpawnPoint != null)
        {
            lastSafePosition = fallbackSpawnPoint.position;
            lastSafeRotation = fallbackSpawnPoint.rotation;
        }
        else
        {
            lastSafePosition = playerController.transform.position;
            lastSafeRotation = playerController.transform.rotation;
        }
    }
}
