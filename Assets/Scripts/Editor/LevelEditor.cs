using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    SerializedProperty busTypes;
    SerializedProperty range;
    SerializedProperty colors;

    private Vector2 scrollPosition; 

    private void OnEnable()
    {
        busTypes = serializedObject.FindProperty("busTypes");
        range = serializedObject.FindProperty("range");
        colors = serializedObject.FindProperty("colors");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Level level = (Level)target;

        
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        level.range = EditorGUILayout.FloatField("Range", level.range);

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        
        EditorGUILayout.LabelField("Passenger Colors In Level", EditorStyles.boldLabel);

        HashSet<int> usedColors = GetUsedColors(colors);

        for (int i = 0; i < colors.arraySize; i++)
        {
            SerializedProperty color = colors.GetArrayElementAtIndex(i);
            var currentColorIndex = color.enumValueIndex;

            
            List<string> availableColorNames = new List<string>();
            List<int> availableColorIndices = new List<int>();

            foreach (var colorOption in System.Enum.GetValues(typeof(Colors)))
            {
                int index = (int)colorOption;
                if (index == currentColorIndex || !usedColors.Contains(index))
                {
                    availableColorNames.Add(colorOption.ToString());
                    availableColorIndices.Add(index);
                }
            }

            GUI.backgroundColor = ((Colors)currentColorIndex).GetColor();

            EditorGUILayout.BeginVertical("box");

            
            int selectedIndex = availableColorIndices.IndexOf(currentColorIndex);
            selectedIndex = EditorGUILayout.Popup($"Color {i}", selectedIndex, availableColorNames.ToArray());

            
            color.enumValueIndex = availableColorIndices[selectedIndex];

            
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                colors.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }

        
        if (GUILayout.Button("Add Color"))
        {
            AddUniqueColor(colors);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        
        
        EditorGUILayout.LabelField("Bus Types", EditorStyles.boldLabel);
        
        float scrollHeight = 300f; 
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));

        for (int i = 0; i < busTypes.arraySize; i++)
        {
            SerializedProperty busType = busTypes.GetArrayElementAtIndex(i);
            SerializedProperty capacity = busType.FindPropertyRelative("capacity");
            SerializedProperty colors = busType.FindPropertyRelative("color");

            
            var colorValue = (Colors)colors.enumValueIndex;
            GUI.backgroundColor = colorValue.GetColor(); 

            EditorGUILayout.BeginVertical("box");
            
            
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

            
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();

        
        if (GUILayout.Button("Add Bus Type"))
        {
            busTypes.InsertArrayElementAtIndex(busTypes.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    private void AddUniqueColor(SerializedProperty colors)
    {
        var existingColors = new HashSet<int>();
        for (int i = 0; i < colors.arraySize; i++)
        {
            existingColors.Add(colors.GetArrayElementAtIndex(i).enumValueIndex);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(Colors)).Length; i++)
        {
            if (!existingColors.Contains(i))
            {
                colors.InsertArrayElementAtIndex(colors.arraySize);
                colors.GetArrayElementAtIndex(colors.arraySize - 1).enumValueIndex = i;
                return;
            }
        }

        Debug.LogWarning("All colors are already in the list!");
    }
    
    private HashSet<int> GetUsedColors(SerializedProperty colors)
    {
        HashSet<int> usedColors = new HashSet<int>();
        for (int i = 0; i < colors.arraySize; i++)
        {
            usedColors.Add(colors.GetArrayElementAtIndex(i).enumValueIndex);
        }
        return usedColors;
    }
}
