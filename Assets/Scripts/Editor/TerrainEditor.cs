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
        Coast, Mountain, Smoothing, Beach, Hill
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
            case DisplayCategory.Hill:
                DisplayHillAgentInfo();
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

        if(GUILayout.Button("Apply map properties"))
        {
            Debug.Log("Map parameters saved");
            generator.ApplyNewMapParameters();
        }
            
        EditorGUILayout.TextField("Coast Agent parameters");        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("limit"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfChildren"));

        
        if(GUILayout.Button("Generate coast"))
        {
            Debug.Log("Try to Generate new coast");
            generator.GenerateCoast();
        }
        
    }

    public void DisplayMountainAgentInfo()
    {
        EditorGUILayout.TextField("Mountain Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainTurnLimit"));

        if (GUILayout.Button("Add more mountains"))
        {
            Debug.Log("Try to Generate mountains");
            generator.RaiseMountains();
        }
    }

    public void DisplaySmoothingAgentInfo()
    {
        EditorGUILayout.TextField("Smoothing Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothTokens"));

        if (GUILayout.Button("Smooth coastline"))
        {
            Debug.Log("Smooth area");
            generator.SmoothCoast();
        }
    }

    public void DisplayBeachAgentInfo()
    {
        EditorGUILayout.TextField("Beach Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("beachStartX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("beachStartY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("beachTokens"));

        if (GUILayout.Button("Smooth coastline"))
        {
            Debug.Log("Make coast beach");
            generator.MakeCoastBeach();
        }
    }

    public void DisplayHillAgentInfo()
    {
        EditorGUILayout.TextField("Hill Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillStartX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillStartY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHillHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minHillHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillscale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillAreaWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hillAreaLength"));
        EditorGUILayout.Space();

        EditorGUILayout.TextField("Wave parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lambda"));

        if (GUILayout.Button("Generate Hills"))
        {
            Debug.Log("Make hills on area");
            generator.BuildHills();
        }
    }
}
