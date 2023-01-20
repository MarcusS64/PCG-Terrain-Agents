using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 256;
    public int mapHeight = 256;
    public float noiseScale = 20f;

    public void GenerateMap()
    {
        float[,] noiseMap = FindObjectOfType<Terrain>().GetComponent<TerrainGenerator>().GetNoiseMap();
        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawNoiseMap(noiseMap);
    }
}