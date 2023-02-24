using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HillAgent 
{
    public static float hillSlope;
    public static int radius = 100;
    public static float offsetX; //Should be an input parameter perhaps to avoid static 
    public static float offsetY;
    public static (int x, int y)[] directions = new (int, int)[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
    //public static int hillTokens = 100;
    public static float hillHeight = 0.5f;

    public static Node[,] GenerateHills(int startX, int startY, Node[,] map, int width, int length, float maxHeight, float minHeight, int hillTokens) 
    {
        hillHeight = Random.Range(minHeight, maxHeight);
        Point location = new Point(startX, startY);
        //(int x, int y) travelDirection = directions[Random.Range(0, directions.Length)];
        //Vector3 direction = new Vector3(travelDirection.x, 0, travelDirection.y);
        //map[startX, startY].SetHeight(2.0f); //We set this in PathFinding() during the first loop
        Debug.Log("Hill peak lifted. Hill tokens count is: " + hillTokens);
        PathFinding(map, map[startX, startY], map[startX + radius, startY + radius], hillTokens);
        //map[location.x, location.y].SetHeight(hillHeight);
        
        return map;
    }

    private static void ExpandHill(Node[,] map, Point location, int hillTokens)
    {
        hillTokens--;
        for (int i = 0; i < hillTokens; i++)
        {
            foreach (Node child in map[location.x, location.y].adjacentSquares)
            {
                child.SetAverageHeight(true);
            }
        }
    }

    public static Queue<Node> myQueue = new Queue<Node>(); //First in first out
    public static List<Node> removeQueue = new List<Node>();
    public static void PathFinding(Node[,] graph, Node start, Node goal, int tokens)
    {
        myQueue.Clear();
        myQueue.Enqueue(start);
        start.queued = true;
        removeQueue.Clear();
        removeQueue.Add(start);
        Debug.Log("Queue count is: " + myQueue.Count);
        while (myQueue.Count > 0)
        {
            Node currentTile = myQueue.Dequeue();            
            tokens--;
            for (int i = 0; i < currentTile.adjacentSquares.Count; i++)
            {
                if (!currentTile.adjacentSquares[i].queued) //&& !currentTile.adjacentSquares[i].blocked
                {
                    myQueue.Enqueue(currentTile.adjacentSquares[i]);                    
                    //currentTile.adjacentSquares[i].SetParent(currentTile);

                    if (tokens <= 0) //currentTile.adjacentSquares[i] == goal
                    {
                        //RetracePath(start, goal);
                        foreach (Node node in removeQueue)
                        {
                            node.queued = false;
                        }
                        return;
                    }
                    currentTile.adjacentSquares[i].queued = true; //Need to untag the children somehow otherwise they will be blocked
                    removeQueue.Add(currentTile.adjacentSquares[i]);
                    currentTile.SetHeight(hillHeight); //Replace this with the WaveFunction
                    //currentTile.SetAverageHeight(false);
                    Debug.Log("Set average height");
                    
                    //currentTile.adjacentSquares[i].color = Color.Blue;
                }
            }
        }

       
    }

    public static void WaveFunction()
    {
        float lambda; //period normalize the distance to fit the 2*Mathf.PI 
        float amplitude; //Height of the hill
        float distance; //Mathf.sqrt(Mathf.Pow(startX - currentX, 2) + Mathf.Pow(startY - currentY, 2));
        //Use the Queue to add the points by BFS
        //Loop through the queue and calculate the distance from the start
        //Using the period and the distance we should be able to set an apropriate height using the wave equation
        //Don't forget to decrease the elevation (aplitude) as we move away from the centre
    }

    public static Node[,] PerlinHills(int startX, int startY, Node[,] map, int width, int length, float scale)
    {
        offsetX = Random.Range(0f, -50f);
        offsetY = Random.Range(0f, -50f);
        for (int i = startX; i < startX + width; i++)
        {
            for (int j = startY; j < startY + length; j++)
            {
                map[i, j].SetHeight(CalculateHeight(i, j, width, length, scale));
            }
        }
        Debug.Log("New hills calculated starting at: " + startX + " and " + startY);
        return map;
    }

    private static float CalculateHeight(int i, int j, int width, int length, float scale)
    {
        float xCoord = (float)i / width * scale; // + offsetX;
        float yCoord = (float)j / length * scale; // + offsetY;

        return NoiseFunction.perlin(xCoord, yCoord);
    }

    private static float TrigCardinal(float x, int N)
    {
        if(x == 0f)
        {
            return 1;
        }
        //float tau = 0;
        if(N % 2 == 1)
        {
            return Mathf.Sin(N * Mathf.PI * x / 2) / (N * Mathf.Sin(Mathf.PI * x / 2));
        }
        else
        {
            return Mathf.Sin(N * Mathf.PI * x / 2) / (N * Mathf.Tan(Mathf.PI * x / 2));
        }

    }


}
