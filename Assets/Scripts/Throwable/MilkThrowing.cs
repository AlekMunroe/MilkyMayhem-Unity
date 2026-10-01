using UnityEngine;
using UnityEngine.InputSystem;

public class MilkThrowing : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float throwStrength = 100f;
    [SerializeField] private Vector3 projectileSpawnPoint;

    [Header("Randomness Settings")]
    [SerializeField] private float randomRange = 5f;
    
    [Header("Projectile Object")]
    [SerializeField] private Rigidbody projectilePrefab;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }
        UpdateThrowing(mouse);
    }

    private void UpdateThrowing(Mouse mouse)
    {
        bool throwPressed = mouse.leftButton.wasPressedThisFrame;
        if (throwPressed)
        {
            ThrowMilk();
        }
    }

    private void ThrowMilk()
    {
        projectileSpawnPoint = transform.position;
        Quaternion projectileRotation = Random.rotation;
        float randomRotation = Random.Range(-randomRange,randomRange);
        Rigidbody spawnedProjectile = Instantiate(projectilePrefab, projectileSpawnPoint, projectileRotation);
        spawnedProjectile.AddForce(transform.forward * throwStrength);
        spawnedProjectile.AddTorque(randomRange,randomRange,randomRange);
    }
}
