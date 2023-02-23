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
        map[startX, startY].SetHeight(2.0f);
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
    public static void PathFinding(Node[,] graph, Node start, Node goal, int tokens)
    {
        myQueue.Clear();
        myQueue.Enqueue(start);
        Debug.Log("Queue count is: " + myQueue.Count);
        while (myQueue.Count > 0)
        {
            Node currentTile = myQueue.Dequeue();
            for (int i = 0; i < currentTile.adjacentSquares.Count; i++)
            {
                if (!currentTile.adjacentSquares[i].visited) //&& !currentTile.adjacentSquares[i].blocked
                {
                    myQueue.Enqueue(currentTile.adjacentSquares[i]);                    
                    //currentTile.adjacentSquares[i].SetParent(currentTile);

                    if (tokens <= 0) //currentTile.adjacentSquares[i] == goal
                    {
                        //RetracePath(start, goal);
                        return;
                    }
                    currentTile.adjacentSquares[i].visited = true; //Need to untag the children somehow otherwise they will be blocked
                    currentTile.SetHeight(hillHeight);
                    //currentTile.SetAverageHeight(false);
                    Debug.Log("Set average height");
                    tokens--;
                    //currentTile.adjacentSquares[i].color = Color.Blue;
                }


            }
        }
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
