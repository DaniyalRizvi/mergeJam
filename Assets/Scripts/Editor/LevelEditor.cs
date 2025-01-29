using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    SerializedProperty busTypes;
    SerializedProperty range;
    SerializedProperty colors;
    private Vector2 busTypesScrollPosition;

    private void OnEnable()
    {
        busTypes = serializedObject.FindProperty("busTypes");
        range = serializedObject.FindProperty("range");
        colors = serializedObject.FindProperty("colors");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Display Level Settings
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);

        Level level = (Level)target;
        level.range = 7.5f;
        level.range = EditorGUILayout.FloatField("Range", level.range);
        
        level.isHard = EditorGUILayout.Toggle("Hard Level", level.isHard);

        EditorGUILayout.Space();

        // Display Passenger Colors
        EditorGUILayout.LabelField("Passenger Colors In Level", EditorStyles.boldLabel);
        HashSet<Colors> usedColors = GetUsedColors(colors);
        
        for (int i = 0; i < colors.arraySize; i++)
        {
            SerializedProperty colorElement = colors.GetArrayElementAtIndex(i);
            SerializedProperty color = colorElement.FindPropertyRelative("color");
            SerializedProperty count = colorElement.FindPropertyRelative("count");

            Colors currentColor = (Colors)color.enumValueIndex;

            // Prepare dropdown options excluding used colors except the current one
            //List<Colors> availableColors = GetAvailableColors(usedColors, currentColor);
            List<Colors> availableColors = System.Enum.GetValues(typeof(Colors)).Cast<Colors>().ToList();
            List<string> availableColorNames = availableColors.ConvertAll(c => c.ToString());

            GUI.backgroundColor = currentColor.GetColor(); // Set background color

            EditorGUILayout.BeginVertical("box");

            // Dropdown for color selection
            int selectedIndex = availableColors.IndexOf(currentColor);
            int newIndex = EditorGUILayout.Popup($"Color {i + 1}", selectedIndex, availableColorNames.ToArray());
            if (newIndex >= 0 && newIndex < availableColors.Count)
            {
                color.enumValueIndex = (int)availableColors[newIndex];
            }

            // Count Field
            count.intValue = EditorGUILayout.IntField("Count", count.intValue);

            // Remove Button
            if (GUILayout.Button("Remove"))
            {
                colors.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white; // Reset background color
        }

        // Add Color Button
        if (GUILayout.Button("Add Color"))
        {
            //AddUniqueColor(colors, usedColors);
            colors.InsertArrayElementAtIndex(colors.arraySize);
        }


        EditorGUILayout.Space();

        // Bus Types
        EditorGUILayout.LabelField($"Bus Types. Count: {level.busTypes.Count}", EditorStyles.boldLabel);

        float scrollHeight = 300f; // Adjust this height as needed
        busTypesScrollPosition =
            EditorGUILayout.BeginScrollView(busTypesScrollPosition, GUILayout.Height(scrollHeight));

        for (int i = 0; i < busTypes.arraySize; i++)
        {
            SerializedProperty busType = busTypes.GetArrayElementAtIndex(i);
            SerializedProperty capacity = busType.FindPropertyRelative("capacity");
            SerializedProperty color = busType.FindPropertyRelative("color");

            var currentColor = (Colors)color.enumValueIndex;
            GUI.backgroundColor = currentColor.GetColor();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Bus Type {i + 1}", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(capacity, new GUIContent("Capacity"));
            EditorGUILayout.PropertyField(color, new GUIContent("Color"));

            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                busTypes.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Bus Type"))
        {
            busTypes.InsertArrayElementAtIndex(busTypes.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private List<Colors> GetAvailableColors(HashSet<Colors> usedColors, Colors currentColor)
    {
        var allColors = System.Enum.GetValues(typeof(Colors)).Cast<Colors>();
        return allColors.Where(color => color == currentColor || !usedColors.Contains(color)).ToList();
    }

    private HashSet<Colors> GetUsedColors(SerializedProperty colors)
    {
        HashSet<Colors> usedColors = new HashSet<Colors>();
        for (int i = 0; i < colors.arraySize; i++)
        {
            SerializedProperty color = colors.GetArrayElementAtIndex(i).FindPropertyRelative("color");
            usedColors.Add((Colors)color.enumValueIndex);
        }

        return usedColors;
    }

    private void AddUniqueColor(SerializedProperty colors, HashSet<Colors> usedColors)
    {
        foreach (Colors color in System.Enum.GetValues(typeof(Colors)))
        {
            if (!usedColors.Contains(color))
            {
                colors.InsertArrayElementAtIndex(colors.arraySize);
                SerializedProperty newElement = colors.GetArrayElementAtIndex(colors.arraySize - 1);

                // Initialize new element
                newElement.FindPropertyRelative("color").enumValueIndex = (int)color;
                newElement.FindPropertyRelative("count").intValue = 0;

                return;
            }
        }

        Debug.LogWarning("All colors are already in the list!");
    }
}
