using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [SerializeField] private VehicleRoute route;

    [SerializeField, Min(0f)] private float movementSpeed = 5f;
    [SerializeField, Min(0f)] private float turningSpeed = 180f;
    [SerializeField, Min(0f)] private float arrivalDistance = 0.1f;

    private int currentNodeIndex;
    private bool hasFinished;

    private void Start()
    {
        if (!ValidateRoute())
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (hasFinished)
        {
            return;
        }

        // Stop if the route fails whilst in play mode
        if (route == null || route.NodeCount == 0)
        {
            StopWithError("The route is missing or has no nodes");

            return;
        }

        Transform currentNode = route.GetNode(currentNodeIndex);

        if (currentNode == null)
        {
            StopWithError("Node at index " + currentNodeIndex + " is missing");

            return;
        }

        Vector3 targetPosition = currentNode.position;
        Vector3 direction = targetPosition - transform.position;

        // Turn horizontally
        Vector3 turnDirection = new Vector3(direction.x, 0f, direction.z);

        if (turnDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(turnDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation.normalized, targetRotation, turningSpeed * Time.deltaTime);
        }

        // Mode towards the node
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= arrivalDistance)
        {
            AdvanceToNextNode();
        }
    }

    private void AdvanceToNextNode()
    {
        if (currentNodeIndex < route.NodeCount - 1)
        {
            currentNodeIndex++;
        }
        else if (route.Loop)
        {
            currentNodeIndex = 0;
        }
        else
        {
            hasFinished = true;
        }
    }

    private bool ValidateRoute()
    {
        if (route == null)
        {
            Debug.LogError("VehicleMovement: VehicleRoute is not assigned");

            return false;
        }

        if (route.NodeCount == 0)
        {
            Debug.LogError("VehicleMovement: The route has no nodes");

            return false;
        }

        for (int i = 0; i < route.NodeCount; i++)
        {
            if (route.GetNode(i) == null)
            {
                Debug.LogError("VehicleMovement: Node array index " + i + " is not assigned");

                return false;
            }
        }

        return true;
    }

    private void StopWithError(string message)
    {
        Debug.LogError("VehicleMovement: " + message);

        enabled = false;
    }
}