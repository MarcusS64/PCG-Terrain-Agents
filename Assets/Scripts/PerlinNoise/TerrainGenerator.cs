using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;

    public int width = 256;
    public int height = 256;

    public float scale = -20f;

    public float offsetX; //To create a randomized map every time the terrain is generated
    public float offsetY;

    float[,] noiseMap;
    bool stopGenerating;

    private void Start()
    {
        offsetX = Random.Range(0f, -50f);
        offsetY = Random.Range(0f, -50f);
    }

    private void Update() //Update for testing purposes, should be in Start
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        //offsetX -= Time.deltaTime * 5f;
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights()); //Takes the height map array values to set the heights of the terrain
        return terrainData;
    }

    float[,] GenerateHeights() //Height values for the noise map
    {
        float[,] heights = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heights[i, j] = CalculateHeight(i, j);
            }
        }
        if (!stopGenerating)
        {
            noiseMap = heights;
            stopGenerating = true;
        }
        //Debug.Log(heights[100, 100]);
        return heights;
    }

    float CalculateHeight(int i, int j)
    {
        float xCoord = (float)i / width * scale + offsetX;
        float yCoord = (float)j / height * scale + offsetY;

        return NoiseFunction.perlin(xCoord, yCoord);
        //return Mathf.PerlinNoise(xCoord, yCoord);
    }

    public float[,] GetNoiseMap()
    {
        return noiseMap;
    }
}
