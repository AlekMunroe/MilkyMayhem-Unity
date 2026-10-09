using UnityEngine;

public class MilkCheckpoint : MonoBehaviour
{
    [Header("<color=red>Development</color>")]
    [SerializeField] private GameObject _dev_visualRepresentation;

    [Header("Delivery")]
    [SerializeField, Min(0f)] private float maximumSafeDeliverySpeed = 10f;
    [SerializeField, Min(0)] private int scoreValue = 100;
    [SerializeField] private Transform landingPoint;

    [Header("Feedback")]
    [SerializeField] private GameObject successfulDeliveryVisual;
    [SerializeField] private GameObject failedDeliveryVisual;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successfulDeliverySound;
    [SerializeField] private AudioClip failedDeliverySound;

    private bool isCleared;

    public bool IsCleared => isCleared;

    private void Awake()
    {
        Collider checkpointCollider = GetComponent<Collider>();
        checkpointCollider.isTrigger = true;

        if(_dev_visualRepresentation == null)
        {
            Debug.LogWarning("MilkCheckpoint: The dev visual representation for the checkpoint is not assigned");
        }

        if(successfulDeliveryVisual != null)
        {
            successfulDeliveryVisual.SetActive(false);
        }

        if(failedDeliveryVisual != null)
        {
            failedDeliveryVisual.SetActive(false);
        }
    }

    private void Start()
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
        {
            _dev_visualRepresentation.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCleared)
        {
            return;
        }

        MilkCrate milkCrate = other.GetComponentInParent<MilkCrate>();

        if(milkCrate == null)
        {
            return;
        }

        bool deliveryFailed = milkCrate.IsBroken || milkCrate.CurrentSpeed > maximumSafeDeliverySpeed;

        if (deliveryFailed)
        {
            milkCrate.MarkBroken();
            ShowFailedDelivery();
            return;
        }

        if(GameManager.Instance == null)
        {
            Debug.LogError("MilkCheckpoint: No GameManager exists");
            return;
        }

        bool registered = GameManager.Instance.RegisterCheckpointCleared(this, scoreValue);

        if (!registered)
        {
            return;
        }

        isCleared = true;

        milkCrate.LandAtCheckpoint(landingPoint);

        ShowSuccessfulDelivery();
    }   

    /// <summary>
    /// Show feedback to the player after a successful delivery
    /// </summary>
    private void ShowSuccessfulDelivery()
    {
        if (_dev_visualRepresentation != null)
        {
            _dev_visualRepresentation.SetActive(false);
        }

        if (failedDeliveryVisual != null)
        {
            failedDeliveryVisual.SetActive(false);
        }

        if (successfulDeliveryVisual != null)
        {
            successfulDeliveryVisual.SetActive(true);
        }

        if (audioSource != null &&
            successfulDeliverySound != null)
        {
            audioSource.PlayOneShot(successfulDeliverySound);
        }
    }

    /// <summary>
    /// Show feeedback to the player after a failed delivery
    /// </summary>
    private void ShowFailedDelivery()
    {
        if (successfulDeliveryVisual != null)
        {
            successfulDeliveryVisual.SetActive(false);
        }

        if (failedDeliveryVisual != null)
        {
            failedDeliveryVisual.SetActive(true);
        }

        if (audioSource != null &&
            failedDeliverySound != null)
        {
            audioSource.PlayOneShot(failedDeliverySound);
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
