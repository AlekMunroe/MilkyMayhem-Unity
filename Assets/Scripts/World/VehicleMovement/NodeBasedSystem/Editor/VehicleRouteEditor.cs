using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleRoute))]
public class VehicleRouteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VehicleRoute route = (VehicleRoute)target;

        // Remember the nodes before adding them
        Transform[] previousNodes = new Transform[route.NodeCount];

        for (int i = 0; i < previousNodes.Length; i++)
        {
            previousNodes[i] = route.GetNode(i);
        }

        int undoGroup = Undo.GetCurrentGroup();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < previousNodes.Length; i++)
            {
                Transform previousNode = previousNodes[i];

                if (previousNode == null || !previousNode.IsChildOf(route.transform) || previousNode == route.transform)
                {
                    continue;
                }

                bool stillAssigned = false;

                for (int j = 0; j < route.NodeCount; j++)
                {
                    if (route.GetNode(j) == previousNode)
                    {
                        stillAssigned = true;

                        break;
                    }
                }

                if (!stillAssigned)
                {
                    Undo.DestroyObjectImmediate(previousNode.gameObject);
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
        {
            if (GUILayout.Button("Add Node"))
            {
                AddNode();
            }
        }
    }

    private void AddNode()
    {
        VehicleRoute route = (VehicleRoute)target;

        serializedObject.Update();

        SerializedProperty nodes = serializedObject.FindProperty("nodes");
        int nodeIndex = nodes.arraySize;

        // Put the first node at the route GameObject position
        Vector3 position = route.transform.position;

        // Put the next node five in front of the last node
        for (int i = nodeIndex - 1; i >= 0; i--)
        {
            Transform previousNode = nodes.GetArrayElementAtIndex(i).objectReferenceValue as Transform;

            if (previousNode != null)
            {
                position = previousNode.position + route.transform.forward * 5f;

                break;
            }
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Add Route Node");

        GameObject node = new GameObject($"Node_{nodeIndex + 1:00}");

        Undo.RegisterCreatedObjectUndo(node, "Add Route Node");
        Undo.SetTransformParent(node.transform, route.transform, "Parent Route Node");

        Undo.RecordObject(node.transform, "Position Route Node");
        node.transform.position = position;
        node.transform.localRotation = Quaternion.identity;

        // Refresh the array after the object creates or undo
        serializedObject.Update();
        nodes = serializedObject.FindProperty("nodes");

        int newIndex = nodes.arraySize;
        nodes.arraySize = newIndex + 1;

        SerializedProperty newElement = nodes.GetArrayElementAtIndex(newIndex);

        newElement.objectReferenceValue = node.transform;

        serializedObject.ApplyModifiedProperties();

        Undo.CollapseUndoOperations(undoGroup);

        EditorGUIUtility.PingObject(node);
        SceneView.RepaintAll();
    }
}