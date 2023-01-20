using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    [SerializeField] float coastLevel;

    float[,] noiseMap;
    Node[,] squares;
    Agent agent;
    [SerializeField] int startTokens;
    [SerializeField] int startX;
    [SerializeField] int startY;
    [SerializeField] int limit;
    [SerializeField] int numberOfChildren;
    [SerializeField] int smoothTokens;
    [SerializeField] int mountainTokens;
    [SerializeField] int mountainTurnLimit;
    //[SerializeField] CoastalAgent CoastalAgent;
    private void Start()
    {
        squares = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
               squares[i, j] = new Node(i, j);
            }
        }
        ConnectSquares(width, height, true);
        ConnectSquares(width, height, false);
        agent = new Agent();
        var nodes = GetRandomNode(squares[startX, startY]);
        squares[startX, startY].visited = true;
        squares[startX, startY].SetHeight(0.5f, "start");
        agent.SetProperties(startTokens, squares[startX, startY], nodes.Item1, nodes.Item2);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private void Update() //Update for testing purposes, should be in Start
    {
        

        //offsetX -= Time.deltaTime * 5f;
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        //Debug.Log("Terrain generate called");
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights()); //Takes the height map array values to set the heights of the terrain
        return terrainData;
    }

    float[,] GenerateHeights() //Height values for the noise map
    {
        float[,] heights = new float[width, height];

        CoastalAgent.SetSquaresMap(squares);
        CoastalAgent.CoastLineGenerate(agent, numberOfChildren, limit);
        squares = CoastalAgent.ReturnCoast();
        SmoothCoast();
        RaiseMountains();
        //RiseLandmass();
        //Point mountainStart = new Point(startX, startY);

        //StartMountain:
        //    MountainAgent.MountainGenerate(mountainStart.x, mountainStart.y, mountainTokens, squares, mountainTurnLimit);
        //SmoothLandscape();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if (squares[i, j].GetHeight() > 0 && squares[i, j].GetHeight() <= 0.5f)
                //{
                //    //Debug.Log("squares smoothed successfully");
                //    Debug.Log("lifted point");
                //}
                heights[i, j] = squares[i, j].GetHeight();
            }
        }
        return heights;
    }

    private void ConnectSquares(int width, int height, bool horizontal) //Was static
    {
        if (horizontal) { width--; }
        else { height--; }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (horizontal)
                {
                    squares[i, j].SetAdjacentSquare(squares[i + 1, j ]);
                    squares[i + 1, j].SetAdjacentSquare(squares[i, j]);

                }
                else //ie vertical
                {
                    squares[i, j].SetAdjacentSquare(squares[i, j + 1]);
                    squares[i, j + 1].SetAdjacentSquare(squares[i, j]);
                }
            }
        }
    }

    private void SmoothLandscape() //Testing
    {
        for (int i = width / 4; i < 3 * width / 4; i += 40)
        {
            for (int j = height / 4; j < 3 * height / 4; j += 40)
            {
                squares = SmoothingAgent.Smooth(i, j, 10000, squares);
            }
        }
    }

    private void SmoothCoast()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(squares[i, j].visited)
                {
                    foreach (Node square in squares[i, j].adjacentSquares)
                    {
                        if(squares[i, j].GetHeight() - square.GetHeight() >= 0.5f)
                        {
                            squares = SmoothingAgent.Smooth(i, j, smoothTokens, squares);
                        }
                    }
                }
            }
        }
    }

    private void RiseLandmass() //Testing
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (j > height / 4 && j < 3 * height / 4 && i > width / 4 && i < 3 * width / 4) //Testing purposes
                {
                    //heights[i, j] = 0.5f;
                    squares[i, j].SetHeight(coastLevel, "hello");
                }
                //heights[i, j] = squares[i, j].GetHeight();
                //heights[i, j] = CalculateHeight(i, j);
            }
        }
    }

    private void RaiseMountains()
    {
        int k = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                if(squares[i, j].SameSorroundingElevation(5) && squares[i, j].GetHeight() >= 0.5f)
                {
                    if(Random.Range(0, 100) > 70)
                        squares = MountainAgent.RiseMountains(i, j, mountainTokens, squares);
                }
            }
        }
    }

    private (Node, Node) GetRandomNode(Node start) //Might have to check that it's not the one we're standing on already as a start
    {
        Node repulsor = squares[Random.Range(0, width), Random.Range(0, height)];
        Node attractor = squares[Random.Range(0, width), Random.Range(0, height)];
        return (repulsor, attractor);
    }

}
