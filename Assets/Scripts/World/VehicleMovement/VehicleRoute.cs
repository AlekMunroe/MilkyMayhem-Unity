using UnityEngine;

public class VehicleRoute : MonoBehaviour
{
    [SerializeField] private Transform[] nodes = new Transform[0];
    [SerializeField] private bool loop;

    [SerializeField] private bool showVisualisation = true;

    public int NodeCount => nodes.Length;
    public bool Loop => loop;

    public Transform GetNode(int index)
    {
        if (index < 0 || index >= nodes.Length)
        {
            return null;
        }

        return nodes[index];
    }

    private void OnDrawGizmos()
    {
        if (!showVisualisation)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(nodes[i].position, 0.3f);

            if (i + 1 < nodes.Length && nodes[i + 1] != null)
            {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }

        if (loop && nodes.Length > 1 && nodes[0] != null && nodes[nodes.Length - 1] != null)
        {
            Gizmos.DrawLine(nodes[nodes.Length - 1].position, nodes[0].position);
        }
    }
}