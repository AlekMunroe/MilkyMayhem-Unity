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

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    private float verticalRotation;

    private void Start()
    {
        if(camObject == null)
        {
            Debug.LogError("CameraController: The camObject has not been assigned.");

            enabled = false;
            return;
        }

        if (Mouse.current == null)
        {
            Debug.LogWarning("CameraController: Mouse is not detected. Please ensure a mouse is connected to the computer.");

            enabled = false;
            return;
        }
    }

    private void Update()
    {
        RotateCamera();
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
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        //Rotate the camera vertically
        camObject.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}