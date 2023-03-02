using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentType { Coast, SmoothCoast, Mountain, Beach }
public class IslandGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    [SerializeField] float coastLevel;

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
    [SerializeField] int beachStartX;
    [SerializeField] int beachStartY;
    [SerializeField] int beachTokens;
    [SerializeField] int hillStartX = 100;
    [SerializeField] int hillStartY = 100;
    [SerializeField] int hillTokens = 10;
    [SerializeField] float maxHillHeight = 0.8f;
    [SerializeField] float minHillHeight = 0.6f;
    [SerializeField] float hillscale = -20f;
    [SerializeField] int hillAreaWidth = 100;
    [SerializeField] int hillAreaLength = 100;
    [SerializeField] float lambda = 2.0f;
    [SerializeField] int waveTokens = 2;

    Terrain terrain;
    private void Start()
    {       
        terrain = GetComponent<Terrain>();
        //GenerateCoast();
    }

    public void GenerateCoast()
    {
        //Generate Graph map
        if (squares == null)
        {
            ApplyNewMapParameters();
        }

        agent = new Agent();
        var nodes = GetRandomNode(squares[startX, startY]);
        squares[startX, startY].visited = true;
        squares[startX, startY].AddHeight(coastLevel, "start");
        agent.SetProperties(startTokens, squares[startX, startY], nodes.Item1, nodes.Item2);

        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    public void ApplyNewMapParameters()
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
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        //Debug.Log("Terrain generate called");
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        CoastalAgent.SetSquaresMap(squares);
        CoastalAgent.CoastLineGenerate(agent, numberOfChildren, limit);
        squares = CoastalAgent.ReturnCoast();

        terrainData.SetHeights(0, 0, GenerateHeights()); //Takes the height map array values to set the heights of the terrain
        return terrainData;
    }

    float[,] GenerateHeights() //Height values for the map
    {
        float[,] heights = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) //Could check for changed values to make it faster perhaps
            {
                heights[i, j] = squares[i, j].GetHeight();
            }
        }
        return heights;
    }

    private void ConnectSquares(int width, int height, bool horizontal)
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

    public void SmoothCoast()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(squares[i, j].visited)
                {
                    foreach (Node square in squares[i, j].adjacentSquares)
                    {
                        if(squares[i, j].GetHeight() - square.GetHeight() >= coastLevel)
                        {
                            squares = SmoothingAgent.Smooth(i, j, smoothTokens, squares);
                        }
                    }
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    public void RaiseMountains()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                if(squares[i, j].SameSorroundingElevation(5) && squares[i, j].GetHeight() >= coastLevel)
                {
                    if(Random.Range(0, 100) > 70)
                        squares = MountainAgent.RiseMountains(i, j, mountainTokens, squares);
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    public void MakeCoastBeach()
    {
        squares = BeachAgent.GenerateBeach(beachStartX, beachStartY, beachTokens, squares);

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    public void BuildHills()
    {
        //squares = HillAgent.PerlinHills(hillStartX, hillStartY, squares, hillAreaWidth, hillAreaLength, scale);
        squares = HillAgent.GenerateHills(hillStartX, hillStartY, squares, maxHillHeight, minHillHeight, hillTokens, lambda, waveTokens);
        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    private (Node, Node) GetRandomNode(Node start) //Might have to check that it's not the one we're standing on already as a start
    {
        Node repulsor = squares[Random.Range(0, width), Random.Range(0, height)];
        Node attractor = squares[Random.Range(0, width), Random.Range(0, height)];
        return (repulsor, attractor);
    }

    public void RiseLandmass() //Testing
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (j > height / 4 && j < 3 * height / 4 && i > width / 4 && i < 3 * width / 4) //Testing purposes
                {
                    //heights[i, j] = coastLevel;
                    squares[i, j].AddHeight(coastLevel, "hello");
                }
                //heights[i, j] = squares[i, j].GetHeight();
                //heights[i, j] = CalculateHeight(i, j);
            }
        }
    }

}
