using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Used to move the first person camera connected to the player capsule.
/// Attach this component to the same capsule as PlayerController
/// </summary>
[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private GameObject camObject;
    [SerializeField] private Camera playerCamera;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Head bob")]
    [SerializeField, Min(0f)] private float headBobSpeed = 8f;
    [SerializeField, Min(0f)] private float headBobPitch = 0.75f;
    [SerializeField, Min(0f)] private float headBobRoll = 1f;
    [SerializeField, Min(0f)] private float headBobSmoothing = 10f;

    private float verticalRotation;
    private bool invertVerticalCamera;

    private Rigidbody playerRigidbody;
    private float headBobStrength;
    private float headBobTimer;
    private float currentHeadBobPitch;
    private float currentHeadBobRoll;

    private void Start()
    {
        if (Mouse.current == null)
        {
            Debug.LogWarning("CameraController: Mouse is not detected. Please ensure a mouse is connected to the computer.");

            enabled = false;
            return;
        }

        CheckNull(camObject);

        mouseSensitivity = GameSettingsController.MouseSensitivity;
        invertVerticalCamera = GameSettingsController.InvertVerticalCamera;
        headBobStrength = GameSettingsController.CameraShakeStrength;

        playerRigidbody = GetComponent<Rigidbody>();

        if(playerCamera == null)
        {
            Debug.LogError("CameraController: The camera doesnt exist.");

            enabled = false;
            return;
        }

        if(playerRigidbody == null)
        {
            Debug.LogError("CameraController: The player rigidbody does not exist.");

            enabled = false;
            return;
        }

        playerCamera.fieldOfView = GameSettingsController.FieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.RoundCompleted += HandleRoundCompleted;
    }

    private void Update()
    {
        RotateCamera();
        UpdateHeadBob();
    }

    /// <summary>
    /// Rotate the player horizontally and the camera vertically
    /// </summary>
    /// 
    private void RotateCamera()
    {
        Vector2 mouseMovement = Mouse.current.delta.ReadValue();

        float mouseX = mouseMovement.x * mouseSensitivity;
        float mouseY = mouseMovement.y * mouseSensitivity;

        // Rotate the entire player horizontally
        this.transform.Rotate(Vector3.up * mouseX);

        // Limit the cameras vertical rotation
        float verticalDirection = invertVerticalCamera ? 1f : -1f;

        verticalRotation += mouseY * verticalDirection;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
    }

    /// <summary>
    /// Move the camera's transform and controls camera rotation
    /// </summary>
    private void UpdateHeadBob()
    {
        Vector3 horizontalVelocity = playerRigidbody.linearVelocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.sqrMagnitude > 0.1f;

        float targetPitch = 0f;
        float targetRoll = 0f;

        if (isMoving && !WorldController.isGamePaused && headBobStrength > 0f)
        {
            headBobTimer += Time.deltaTime * headBobSpeed;

            targetPitch = Mathf.Sin(headBobTimer * 2f) * headBobPitch * headBobStrength;
            targetRoll = Mathf.Sin(headBobTimer) * headBobRoll * headBobStrength;
        }
        else
        {
            headBobTimer = 0f;
        }

        currentHeadBobPitch = Mathf.Lerp(currentHeadBobPitch, targetPitch, headBobSmoothing * Time.deltaTime);

        currentHeadBobRoll = Mathf.Lerp(currentHeadBobRoll, targetRoll, headBobSmoothing * Time.deltaTime);

        camObject.transform.localRotation = Quaternion.Euler(verticalRotation + currentHeadBobPitch, 0f, currentHeadBobRoll);
    }

    private void HandleRoundCompleted()
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.RoundCompleted -= HandleRoundCompleted;
        }
    }

    private void CheckNull(UnityEngine.Object reference)
    {
        if (reference == null)
        {
            Debug.LogError(GetType().Name + ": A reference is not assigned", this);

            enabled = false;
        }
    }
}