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
        Map, Coast, Mountain, Smoothing, Hill, River, Test
        //Beach
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
            case DisplayCategory.Map:
                DisplayMapInfo();
                break;
            case DisplayCategory.Coast:
                DisplayCoastAgentInfo();
                break;
            case DisplayCategory.Mountain:
                DisplayMountainAgentInfo();
                break;
            case DisplayCategory.Smoothing:
                DisplaySmoothingAgentInfo();
                break;
            //case DisplayCategory.Beach:
            //    DisplayBeachAgentInfo();
            //    break;
            case DisplayCategory.Hill:
                DisplayHillAgentInfo();
                break;
            case DisplayCategory.River:
                DisplayRiverAgentInfo();
                break;
            case DisplayCategory.Test:
                DisplayTestFunctions();
                break;

                
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Reset terrain"))
        {
            Debug.Log("Terrain reset");
            generator.ResetTerrain();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Noise"))
        {
            Debug.Log("Noise added");
            generator.AddNoise();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    public void DisplayMapInfo()
    {
        EditorGUILayout.TextField("Map properties");
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("depth"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoise"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoise"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vonNeumanNeighbourhood"));
        EditorGUILayout.Space();

        if (GUILayout.Button("Apply map properties"))
        {
            Debug.Log("Map parameters saved");
            generator.ApplyNewMapParameters();
        }
    }

    public void DisplayCoastAgentInfo()
    {                   
        EditorGUILayout.TextField("Coast Agent parameters");        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("childTokenLimit"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfChildren"));
        
        if(GUILayout.Button("Generate coast"))
        {
            Debug.Log("Try to Generate new coast");
            generator.ResetTerrain();
            generator.GenerateCoast();
        }
        
    }

    public void DisplayMountainAgentInfo()
    {
        EditorGUILayout.TextField("Mountain Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainHeightWeight"));
        EditorGUILayout.Space();
        EditorGUILayout.TextField("Generate mountains over whole coast");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainProbability"));
        if (GUILayout.Button("Add mountains over whole coast"))
        {
            generator.RaiseMountains();
            Debug.Log("Try to Generate mountains");
        }

        EditorGUILayout.TextField("Add single mountain chain");
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainStartX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mountainStartY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useStartGiven"));

        if (GUILayout.Button("Add mountain chain"))
        {           
            generator.GenerateMountains();
            Debug.Log("Try to Generate mountains");
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("alowHillValleys"));
        EditorGUILayout.Space();

        EditorGUILayout.TextField("Wave parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxlambda"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minlambda"));        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxPhaseShift"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minPhaseShift"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfWaves"));

        if (GUILayout.Button("Generate Hills"))
        {
            Debug.Log("Make hills on area");
            generator.BuildHills();
        }
    }

    public void DisplayRiverAgentInfo()
    {
        EditorGUILayout.TextField("River Agent parameters");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("riverTokens"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heightLimit"));

        if (GUILayout.Button("Add River"))
        {            
            generator.AddRiver();
            Debug.Log("River added to the landmass");
        }
    }

    public void DisplayTestFunctions()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("depth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastLevel"));
        if (GUILayout.Button("Raise area"))
        {            
            generator.RiseLandmass();
            Debug.Log("Lifted square landmass");
        }
    }
}
