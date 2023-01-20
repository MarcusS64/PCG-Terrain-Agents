using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class NoiseDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        MapGenerator mapGen = (MapGenerator)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}