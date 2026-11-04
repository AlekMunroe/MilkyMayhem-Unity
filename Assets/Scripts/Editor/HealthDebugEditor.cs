#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class HealthDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerController playerController = (PlayerController)target;
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Health Debug Options");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dealing Damage");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Deal 10 Damage"))
        {
            playerController.ModifyHealth(10);
        }
        if (GUILayout.Button("Deal 25 Damage"))
        {
            playerController.ModifyHealth(25);
        }
        if (GUILayout.Button("Deal 50 Damage"))
        {
            playerController.ModifyHealth(50);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Healing Damage");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Heal 10 Damage"))
        {
            playerController.ModifyHealth(-10);
        }
        if (GUILayout.Button("Heal 25 Damage"))
        {
            playerController.ModifyHealth(-25);
        }
        if (GUILayout.Button("Heal 50 Damage"))
        {
            playerController.ModifyHealth(-50);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif