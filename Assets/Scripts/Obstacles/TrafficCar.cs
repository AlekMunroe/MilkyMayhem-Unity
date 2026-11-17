using UnityEngine;
using System.Collections.Generic;

public class TrafficCar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 18f;
    [SerializeField ]private int target = 0;
    [SerializeField] private float slowDownDistance = 5f;
    [SerializeField] private float junctionSpeed = 8f;
    [SerializeField] private float rotationSpeed = 5f;
    private float originalSpeed;
    
    [Header("Traffic Points")]
    [SerializeField] private GameObject trafficPointsContainer;
    [SerializeField] private List<Transform> trafficPoints = new List<Transform>();

    void Start()
    {
        if (trafficPointsContainer == null || trafficPointsContainer.transform.childCount == 0)
        {
            Debug.LogError("TrafficCar: Traffic points container is not assigned");
            enabled = false;
            return;
        }
        else
        {
            foreach (Transform point in trafficPointsContainer.transform)
            {
                // Add the point to the trafficPoints list
                trafficPoints.Add(point);
            }
        }

        if (trafficPoints.Count > 0)
        {
            transform.position = trafficPoints[target].position;
        }
    }

    void Update()
    {
        UpdateCarMovement();
    }

    void UpdateCarMovement()
    {
        if (originalSpeed == 0)
        {
            originalSpeed = speed;
        }

        Transform goal = trafficPoints[target];
        Vector3 goalPosition = new Vector3(goal.position.x, 0, goal.position.z);
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);
        Vector3 direction = goal.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion lookAtGoal = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtGoal, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, goal.position) < 0.5f)
        {
            target = (target + 1) % trafficPoints.Count;
        }

        if (goal.CompareTag("Junction") &&
             Vector3.Distance(transform.position, goal.position) < slowDownDistance)
            {
                speed = junctionSpeed;
            }
            
        else
            {
                speed = originalSpeed;
            }


    }
}
