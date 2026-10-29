using UnityEngine;

[DisallowMultipleComponent]
public class CheckpointCompass : MonoBehaviour
{
    [Header("Compass UI")]
    [SerializeField] private GameObject compassUI;
    [SerializeField] private RectTransform compassNeedle;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;

    [Header("Rotation")]
    [SerializeField, Min(0f)] private float rotationSpeed = 360f;

    [Tooltip("Use this if the compass image doesnt point up")]
    [SerializeField] private float imageRotationOffset;

    private void Start()
    {
        if(playerTransform == null && PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }

        if(GameManager.Instance == null)
        {
            Debug.LogError("CheckpointCompass: GameManager not found");

            enabled = false;
            return;
        }

        if(playerTransform == null)
        {
            Debug.LogError("CheckpointCompass: PlayerTransform not assigned");

            enabled = false;
            return;
        }
        
        if(compassUI == null)
        {
            Debug.LogError("CheckpointCompass: CompassUI not assigned");

            enabled = false;
            return;
        }
        
        if(compassNeedle == null)
        {
            Debug.LogError("CheckpointCompass: CompassNeedle not assigned");

            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.IsRoundRunning)
        {
            SetCompassVisible(false);

            return;
        }

        CheckpointSearchArea closestArea = FindClosestSearchArea(out bool playerInsideArea);

        if(closestArea == null || playerInsideArea)
        {
            SetCompassVisible(false);

            return;
        }

        SetCompassVisible(true);

        PointTowardsArea(closestArea);
    }

    private CheckpointSearchArea FindClosestSearchArea(out bool playerInsideArea)
    {
        CheckpointSearchArea closestArea = null;
        float closestDistanceSquared = Mathf.Infinity;

        playerInsideArea = false;

        foreach(MilkCheckpoint checkpoint in GameManager.Instance.Checkpoints)
        {
            if(checkpoint == null || checkpoint.IsCleared || !checkpoint.gameObject.activeInHierarchy)
            {
                continue;
            }

            CheckpointSearchArea searchArea = checkpoint.SearchArea;

            if(searchArea == null || !searchArea.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (searchArea.IsPlayerInside)
            {
                playerInsideArea = true;
            }

            Vector3 areaDirection = searchArea.CompassPosition - playerTransform.position;

            areaDirection.y = 0f;

            float distanceSquared = areaDirection.sqrMagnitude;

            if(distanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestArea = searchArea;
            }
        }

        return closestArea;
    }

    private void PointTowardsArea(CheckpointSearchArea searchArea)
    {
        Vector3 worldDirection = searchArea.CompassPosition - playerTransform.position;

        worldDirection.y = 0f;

        if(worldDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector3 localDirection = playerTransform.InverseTransformDirection(worldDirection.normalized);

        float directionAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float targetRotation = -directionAngle + imageRotationOffset;
        float currentRotation = compassNeedle.localEulerAngles.z;
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);

        compassNeedle.localRotation = Quaternion.Euler(0f, 0f, newRotation);
    }

    private void SetCompassVisible(bool visibility)
    {
        if(compassUI.activeSelf != visibility)
        {
            compassUI.SetActive(visibility);
        }
    }
}
