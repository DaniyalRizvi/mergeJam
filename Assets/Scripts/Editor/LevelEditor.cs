using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    SerializedProperty busTypes;

    private void OnEnable()
    {
        busTypes = serializedObject.FindProperty("busTypes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Bus Types", EditorStyles.boldLabel);
        
        for (int i = 0; i < busTypes.arraySize; i++)
        {
            SerializedProperty busType = busTypes.GetArrayElementAtIndex(i);
            SerializedProperty capacity = busType.FindPropertyRelative("capacity");
            SerializedProperty colors = busType.FindPropertyRelative("colors");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Bus Type {i + 1}", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(capacity, new GUIContent("Capacity"));
            EditorGUILayout.PropertyField(colors, new GUIContent("Colors"));

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                busTypes.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Bus Type"))
        {
            busTypes.InsertArrayElementAtIndex(busTypes.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}