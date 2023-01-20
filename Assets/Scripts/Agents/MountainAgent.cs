using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MountainAgent
{
    static Vector2 direction;
    static float minIncrease = 0.5f;
    static float maxIncrease = 0.7f;
    static float rotation = 45f;
    public static Node[,] MountainGenerate(int startX, int startY, int tokens, Node[,] map, int turnLimit) 
    {
        Point location = new Point(startX, startY);

        GetNewDirection();
        //int index;
        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].SetHeight(Random.Range(minIncrease, maxIncrease), "Elevated");
            //map[location.x, location.y].SetAverageHeight();
            //foreach (Node node in map[location.x, location.y].adjacentSquares)
            //{
            //    SmoothingAgent.Smooth(node.X(), node.Y(), 5, map);
            //}
            
            
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

    public static Node[,] RiseMountains(int startX, int startY, int tokens, Node[,] map)
    {
        Point location = new Point(startX, startY);
        int index;
        //int tokens = agent.GetTokens();
        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].SetAverageHeight(true);
            index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
            location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
        }
        return map;
    }
}
