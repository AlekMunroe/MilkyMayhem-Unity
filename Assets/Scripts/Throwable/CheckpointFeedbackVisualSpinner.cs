using UnityEngine;

public class CheckpointFeedbackVisualSpinner : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;

    void Update()
    {
        this.transform.Rotate(rotation * 1 * Time.deltaTime); 
    }
}
