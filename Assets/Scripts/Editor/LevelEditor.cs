using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    SerializedProperty busTypes;
    SerializedProperty range;

    private Vector2 scrollPosition; // For managing the scroll position

    private void OnEnable()
    {
        busTypes = serializedObject.FindProperty("busTypes");
        range = serializedObject.FindProperty("range");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Level level = (Level)target;

        // Level Settings
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        level.range = EditorGUILayout.FloatField("Range", level.range);

        EditorGUILayout.Space();

        // Bus Types Section with Scroll View
        EditorGUILayout.LabelField("Bus Types", EditorStyles.boldLabel);

        // Scrollable area for Bus Types
        float scrollHeight = 600f; // Fixed height for the scrollable region
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));

        for (int i = 0; i < busTypes.arraySize; i++)
        {
            SerializedProperty busType = busTypes.GetArrayElementAtIndex(i);
            SerializedProperty capacity = busType.FindPropertyRelative("capacity");
            SerializedProperty colors = busType.FindPropertyRelative("color");

            // Set the background color based on the bus color
            var colorValue = (Colors)colors.enumValueIndex;
            GUI.backgroundColor = colorValue.GetColor(); // Using the extension method to get the color

            EditorGUILayout.BeginVertical("box");
            
            // Reset the background color after drawing the bus type
            EditorGUILayout.LabelField($"Bus Type (Color: {colorValue}) (Capacity: {capacity.intValue})", EditorStyles.miniBoldLabel);

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

            // Reset the background color to default
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();

        // Add Bus Type Button below the scroll view
        if (GUILayout.Button("Add Bus Type"))
        {
            busTypes.InsertArrayElementAtIndex(busTypes.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
