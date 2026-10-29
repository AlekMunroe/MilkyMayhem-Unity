using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class MilkStoreExitTrigger : MonoBehaviour
{
    [SerializeField] private GameObject milkStoreDoorObject;
    [SerializeField] private Transform outsideReference;

    private bool hasStartedRound;

    void Start()
    {
        if(PlayerController.Instance != null)
        {
            PlayerController.Instance.SetMilkStoreMovement(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasStartedRound)
        {
            return;
        }

        GameObject collisionRoot = other.transform.root.gameObject;

        if (!collisionRoot.CompareTag("Player"))
        {
            return;
        }

        if(outsideReference == null)
        {
            Debug.LogError("MilkStoreExitTrigger: Missing outside reference point");

            return;
        }

        Vector3 playerDirection = collisionRoot.transform.position - transform.position;
        Vector3 outsideDirection = outsideReference.position - transform.position;

        bool isPlayerOutside = Vector3.Dot(playerDirection, outsideDirection) > 0f;

        if(!isPlayerOutside)
        {
            return;
        }

        hasStartedRound = true;
        GetComponent<Collider>().enabled = false;

        if(milkStoreDoorObject != null)
        {
            milkStoreDoorObject.SetActive(true);
        }

        if(PlayerController.Instance != null)
        {
            PlayerController.Instance.SetMilkStoreMovement(false);
        }

        if(GameManager.Instance != null)
        {
            GameManager.Instance.StartRound();
        }
    }
}
