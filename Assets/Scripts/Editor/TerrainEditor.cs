using UnityEngine;
using UnityEditor;

/// <summary>
/// EditorTemplate description
/// </summary>
[CustomEditor(typeof(IslandGenerator))]
public class TerrainEditor : Editor
{
    public enum DisplayCategory
    {
        Coast, Mountain, Smoothing, Beach
    }

    public DisplayCategory categoryToDisplay;
    IslandGenerator generator;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);
        EditorGUILayout.Space();
        generator = (IslandGenerator)target;

        switch (categoryToDisplay)
        {
            case DisplayCategory.Coast:
                DisplayCoastAgentInfo();
                break;
            case DisplayCategory.Mountain:
                DisplayMountainAgentInfo();
                break;
            case DisplayCategory.Smoothing:
                DisplaySmoothingAgentInfo();
                break;
            case DisplayCategory.Beach:
                DisplayBeachAgentInfo();
                break;

        }

        serializedObject.ApplyModifiedProperties();
    }

    public void DisplayCoastAgentInfo()
    {
        EditorGUILayout.TextField("Map layout");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("depth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));
        EditorGUILayout.Space();

        EditorGUILayout.TextField("Coast Agent parameters");        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("limit"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfChildren"));

        
        if(GUILayout.Button("Generate terrain"))
        {
            Debug.Log("Try to Generate new terrain");
            //generator.StartTerrainGeneration();
        }
        
    }

    public void DisplayMountainAgentInfo()
    {
        EditorGUILayout.TextField("Mountain Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainTurnLimit"));

        if (GUILayout.Button("Add more mountains"))
        {
            Debug.Log("Try to Generate new terrain");
            //generator.RaiseMountains();
        }
    }

    public void DisplaySmoothingAgentInfo()
    {
        EditorGUILayout.TextField("Smoothing Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothTokens"));

        if (GUILayout.Button("Smooth coastline"))
        {
            Debug.Log("Try to Generate new terrain");
            //generator.SmoothCoast();
        }
    }

    public void DisplayBeachAgentInfo()
    {

    }
}
