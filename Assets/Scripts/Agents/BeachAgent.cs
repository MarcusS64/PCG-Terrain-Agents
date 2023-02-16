using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BeachAgent
{
    public static float heightLimit = 0.5f;
    public static Node[,] Smooth(int startX, int startY, int tokens, Node[,] map)
    {
        Point location = new Point(startX, startY);
        int index;
        //int tokens = agent.GetTokens();
        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].SetAverageHeight(false);
            index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
            location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
        }
        return map;
    }
}
