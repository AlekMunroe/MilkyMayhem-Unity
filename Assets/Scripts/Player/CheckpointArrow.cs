using UnityEngine;

/// <summary>
/// Points an arrow in the direction of the closest checkpoint
/// </summary>
public class CheckpointArrow : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private GameObject arrowObjectOrigin;
    [SerializeField] private float rotationSpeed = 360f;

    [Tooltip("Use this if the model doesnt point on the Z axis correctly")]
    [SerializeField] private Vector3 modelRotationOffset;

    private void Start()
    {
        if(arrowObjectOrigin == null)
        {
            Debug.LogError("CheckpointArrow: The arrow object isnt assigned");

            enabled = false;
            return;
        }

        if(GameManager.Instance == null)
        {
            Debug.LogError("CheckpointArrow: No GameManager exists in the scene");

            enabled = false;
        }
    }

    void Update()
    {
        PointAtClosestCheckpoint();
    }

    /// <summary>
    /// Finds the closest checkpoint and points the arrow at it
    /// </summary>
    private void PointAtClosestCheckpoint()
    {
        MilkCheckpoint closestCheckpoint = FindClosestCheckpoint();

        if(closestCheckpoint == null)
        {
            arrowObjectOrigin.SetActive(false);
            return;
        }

        arrowObjectOrigin.SetActive(true);

        Vector3 checkpointDirection = closestCheckpoint.transform.position - arrowObjectOrigin.transform.position;

        if(checkpointDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(checkpointDirection.normalized) * Quaternion.Euler(modelRotationOffset);

        arrowObjectOrigin.transform.rotation = Quaternion.RotateTowards(arrowObjectOrigin.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Returns the closest checkpoint
    /// </summary>
    /// <returns></returns>
    private MilkCheckpoint FindClosestCheckpoint()
    {
        MilkCheckpoint closestCheckpoint = null;
        float closestDistanceSquared = Mathf.Infinity;

        foreach(MilkCheckpoint checkpoint in GameManager.Instance.Checkpoints)
        {
            if(checkpoint == null || checkpoint.IsCleared || !checkpoint.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distanceSquared = (checkpoint.transform.position - transform.position).sqrMagnitude;

            if(distanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }
}
