using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum AgentType { Coast, SmoothCoast, Mountain, Beach }
public class IslandGenerator : MonoBehaviour
{
    [Range(0, 20)]
    public int depth = 20; //Only used in one place and might be omitted
    [Range(10, 512)]
    public int width = 256;
    [Range(10, 512)]
    public int height = 256;
    [Range(0, 20)]
    public float scale = 20f; //Not used but could set the scale for the map
    [Range(0.0f, 0.9f)]
    [SerializeField] float coastLevel;
    [Range(0.0f, 0.2f)]
    [SerializeField] float maxNoise = 0.01f;
    [Range(-0.2f, 0.0f)]
    [SerializeField] float minNoise = - 0.01f;
    [SerializeField] bool vonNeumanNeighbourhood = true;

    Node[,] squares;
    public static (int x, int y)[] coords = new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    public static (int x, int y)[] NeumanCoords = new (int, int)[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
    Agent agent;
    [Range(10, 20000)]
    [SerializeField] int startTokens;
    [Range(0, 512)]
    [SerializeField] int startX;
    [Range(0, 512)]
    [SerializeField] int startY;
    [Range(1, 20)]
    [SerializeField] int childTokenLimit = 8;
    [Range(2, 2)]
    [SerializeField] int numberOfChildren = 2;
    [Range(0, 8)]
    [SerializeField] int smoothTokens;
    [SerializeField] bool useStartGiven = true;
    [Range(0, 512)]
    [SerializeField] int mountainStartX;
    [Range(0, 512)]
    [SerializeField] int mountainStartY;
    [Range(0, 512)]
    [SerializeField] int mountainTokens;
    [Range(1, 10)]
    [SerializeField] int mountainChainTokens;
    [Range(1, 99)]
    [SerializeField] int mountainProbability;
    [Range(1, 9)]
    [SerializeField] int mountainHeightWeight;
    [Range(0, 5)]
    [SerializeField] int RadiusCheckLimit;
    [SerializeField] bool alowHillValleys = true;
    [SerializeField] int beachStartX;
    [SerializeField] int beachStartY;
    [SerializeField] int beachTokens;
    [SerializeField]
    [OnChangedCall("onSerializedPropertyChange")]
    [Range(0, 512)]
    public int hillStartX = 100;
    [SerializeField]
    [OnChangedCall("onSerializedPropertyChange")]
    [Range(0, 512)]
    public int hillStartY = 100;
    [Range(1, 100)]
    [SerializeField] int hillTokens = 10;
    [Range(0.0f, 0.8f)]
    [SerializeField] float maxHillHeight = 0.8f;
    [Range(0.0f, 0.8f)]
    [SerializeField] float minHillHeight = 0.6f;
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxlambda = 20.0f;
    [Range(0.0f, 100.0f)]
    [SerializeField] float minlambda = 10.0f;
    //[Tooltip("Health value between 0 and 100.")]
    [Range(1, 5)]
    [SerializeField] int numberOfWaves = 2;
    [Range(0.0f, 1.0f)]
    [SerializeField] float maxPhaseShift = 0.5f;
    [Range(-1.0f, 0.0f)]
    [SerializeField] float minPhaseShift = -0.5f;
    [Range(1, 200)]
    [SerializeField] int riverTokens = 10;
    [Range(0.0f, 1.0f)]
    [SerializeField] float heightLimit = 0.6f;

    Terrain terrain;
    public static event Action OnAttributeChanged;
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
        startX = Mathf.Clamp(startX, 0, width - 1);
        startY = Mathf.Clamp(startY, 0, height - 1);
        var nodes = GetRandomNode(squares[startX, startY]);
        squares[startX, startY].visited = true;
        squares[startX, startY].AddHeight(coastLevel); //, "start"
        agent.SetProperties(startTokens, squares[startX, startY], nodes.Item1, nodes.Item2);

        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        GetComponent<AssignSplatMap>().UpdateSplatMap();
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

        if (vonNeumanNeighbourhood)
        {
            ConnectSquares(width, height, true);
            ConnectSquares(width, height, false);
            Debug.Log("used von Neuman neighbourhood");
        }
        else
        {
            ConnectEverySquare(width, height);
            Debug.Log("used More neighbourhood");
        }
              
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        //Debug.Log("Terrain generate called");
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        CoastalAgent.SetSquaresMap(squares);
        CoastalAgent.CoastLineGenerate(agent, numberOfChildren, childTokenLimit);
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

    private void ConnectEverySquare(int width, int height) //Does connect the diagonal
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < coords.Length; k++)//Loops through all the squares in the 8 directions around the square
                {
                    int x = i - coords[k].x;
                    int y = j - coords[k].y;
                    if (x < width && y < height && x >= 0 && y >= 0) //If not then there is no square to connect
                    {
                        squares[i, j].SetAdjacentSquare(squares[x, y]);
                    }
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
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void RaiseMountains()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                if(squares[i, j].SameSorroundingElevation(RadiusCheckLimit, maxNoise) && squares[i, j].GetHeight() >= coastLevel) //squares[i, j].SameSorroundingElevation(RadiusCheckLimit, maxNoise) && 
                {
                    if(UnityEngine.Random.Range(0, 100) > mountainProbability)
                        squares = MountainAgent.RiseMountains(i, j, mountainTokens, squares, mountainHeightWeight);
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void GenerateMountains()
    {
        //squares = MountainAgent.MountainGenerate(mountainStartX, mountainStartY, mountainTokens, squares, mountainTurnLimit);
        mountainStartX = Mathf.Clamp(mountainStartX, 0, width - 1);
        mountainStartY = Mathf.Clamp(mountainStartY, 0, height - 1);
        squares = MountainAgent.StartMountainChain(mountainStartX, mountainStartY, mountainTokens, squares, mountainHeightWeight, coastLevel, maxNoise, useStartGiven, mountainChainTokens, RadiusCheckLimit);
        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void MakeCoastBeach()
    {
        squares = BeachAgent.GenerateBeach(beachStartX, beachStartY, beachTokens, squares);

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
    }

    public void BuildHills()
    {
        //squares = HillAgent.PerlinHills(hillStartX, hillStartY, squares, hillAreaWidth, hillAreaLength, scale);
        hillStartX = Mathf.Clamp(hillStartX, 0, width - 1);
        hillStartY = Mathf.Clamp(hillStartY, 0, height - 1);
        squares = HillAgent.GenerateHills(hillStartX, hillStartY, squares, maxHillHeight, minHillHeight, hillTokens, maxlambda, minlambda, numberOfWaves, maxPhaseShift, minPhaseShift, coastLevel, alowHillValleys);
        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    private (Node, Node) GetRandomNode(Node start) //Might have to check that it's not the one we're standing on already as a start
    {
        Node repulsor = squares[UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height)];
        Node attractor = squares[UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height)];
        return (repulsor, attractor);
    }

    public void AddRiver()
    {
        squares = RiverAgent.GenerateRiver(riverTokens, squares, coastLevel, heightLimit);
        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
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
                    squares[i, j].SetHeight(coastLevel); //, "hello"
                    //Debug.Log("Landmass set to coast level");
                }
                //heights[i, j] = squares[i, j].GetHeight();
                //heights[i, j] = CalculateHeight(i, j);
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void ResetTerrain()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                squares[i, j].visited = false;
                squares[i, j].SetHeight(0.0f);
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void AddNoise()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(squares[i, j].GetHeight() >= coastLevel)
                {
                    squares[i, j].AddHeight(UnityEngine.Random.Range(minNoise, maxNoise));
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, GenerateHeights());
        GetComponent<AssignSplatMap>().UpdateSplatMap();
    }

    public void onSerializedPropertyChange()
    {
        Debug.Log("Marker moved");
        OnAttributeChanged?.Invoke();
    }
}
