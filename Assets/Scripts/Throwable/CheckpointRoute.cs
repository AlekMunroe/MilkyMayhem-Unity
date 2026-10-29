using UnityEngine;

[DisallowMultipleComponent]
public class CheckpointRoute : MonoBehaviour
{
    [SerializeField, Min(1)] private int routeNumber = 1;

    public int RouteNumber => routeNumber;
}
