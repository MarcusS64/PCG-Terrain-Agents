using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MountainAgent
{
    static Vector2 direction;
    static float minIncrease = 0.8f;
    static float maxIncrease = 1f;
    static float rotation = 45f;
    static List<Point> mountainPath;
    static Queue<Point> pathQueue;
    public static Node[,] MountainGenerate(int startX, int startY, int tokens, Node[,] map, int turnLimit) 
    {
        Point location = new Point(startX, startY);
        mountainPath = new List<Point>();
        pathQueue = new Queue<Point>();
        GetNewDirection();
        //int index;
        for (int i = 0; i < tokens; i++)
        {
            mountainPath.Add(location);
            map[location.x, location.y].AddHeight(Random.Range(minIncrease, maxIncrease)); //, "Elevated"
            //Debug.Log(location.x + ", " + location.y);

            LiftSorounding(location, map);


            location.Move((int)Mathf.Ceil(direction.x), (int)Mathf.Ceil(direction.y));
            //index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
            //location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
            if ((i % turnLimit == 0 && i != 0)) //map[location.x + (int)direction.x, location.y + (int)direction.y].GetHeight() < 0.4f ||
            {               
                if(Random.Range(0, 100) >= 50)
                {
                    rotation *= -1;
                }
                direction = Rotate(direction, rotation);
                Debug.Log("Rotated");
            }
        }
        return map;
    }

    private static void LiftSorounding(Point location, Node[,] map)
    {
        //map[location.x, location.y].SetAverageHeight(false, 2);
        foreach (Node node in map[location.x, location.y].adjacentSquares)
        {
            SmoothingAgent.Smooth(node.X(), node.Y(), 4, map);
        }
    }

    private static void GetNewDirection()
    {
        //Vector2 oldDirection = direction;
        do
        {
            direction.x = Random.Range(-1, 2);
            direction.y = Random.Range(-1, 2);
        } while (direction.x + direction.y == 0);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Node[,] RiseMountains(int startX, int startY, int tokens, Node[,] map, int heightWeight) //Actually used
    {
        Point location = new Point(startX, startY);
        int index;
        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].SetAverageHeight(true, heightWeight);
            index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
            location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
        }
        return map;
    }

    public static Node[,] StartMountainChain(int startX, int startY, int tokens, Node[,] map, int heightWeight, float coastLevel, float maxNoise, bool useStartGiven, int chainTokens) //Actually used
    {
        Point location = new Point(startX, startY);
        if (!useStartGiven)
        {
            location = RiverAgent.FindCentreOfHeights(map);
        }

        
        int index;
        List<Node> removeQueue = new List<Node>();

        while(chainTokens > 0)
        {
            chainTokens--;
            removeQueue.Clear();
            for (int i = 0; i < tokens; i++)
            {
                map[location.x, location.y].SetAverageHeight(true, heightWeight);
                index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
                location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
            }

            Stack<Node> myStack = new Stack<Node>();

            myStack.Push(map[location.x, location.y]);
            int breakPoint = 0;
            while (myStack.Count > 0 && breakPoint < 50)
            {
                Node currentTile = myStack.Pop();
                currentTile.queued = true;
                removeQueue.Add(currentTile);
                if (currentTile.GetHeight() <= coastLevel + 0.1f + maxNoise && currentTile.SameSorroundingElevation(5, maxNoise)) 
                {
                    location.SetNew(currentTile.X(), currentTile.Y());
                    //RetracePath(start, goal);
                    break;
                }

                foreach (Node tile in currentTile.adjacentSquares)
                {
                    if (tile.queued)
                    {
                        continue;
                    }
                    if (tile.GetHeight() >= 0.5f)
                    {
                        //tile.SetParent(currentTile);
                        myStack.Push(tile);
                    }
                }
                breakPoint++;
                if(breakPoint >= 50)
                {
                    Debug.Log("Mountain while loop breakPoint triggered!");
                }
            }
            foreach (Node item in removeQueue)
            {
                item.queued = false;
            }
        }
        
        return map;
    }
}
