using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
public class CheckpointSearchArea : MonoBehaviour
{
    [Header("Search Area")]
    [SerializeField, Min(0.1f)] private float areaRadius = 50f;

    [Header("Development")]
    [SerializeField] private Color areaColour = new Color(1f, 0.75f, 0f, 0.75f);

    private SphereCollider areaCollider;

    private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();

    public bool IsPlayerInside => playerColliders.Count > 0;
    public Vector3 CompassPosition => transform.position;
    public float AreaRadius => areaRadius;

    private void Awake()
    {
        areaCollider = GetComponent<SphereCollider>();

        UpdateCollider();
    }

    private void OnValidate()
    {
        areaCollider = GetComponent<SphereCollider>();

        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if(areaCollider == null)
        {
            return;
        }

        areaCollider.isTrigger = true;
        areaCollider.radius = areaRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collisionRoot = other.transform.root.gameObject;

        if (!collisionRoot.CompareTag("Player"))
        {
            return;
        }

        playerColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        playerColliders.Remove(other);
    }

    private void OnDisable()
    {
        playerColliders.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColour;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireSphere(Vector3.zero, areaRadius);

        Gizmos.matrix = Matrix4x4.identity;
    }
}
