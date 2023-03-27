using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmoothingAgent
{

    public static Node[,] Smooth(int startX, int startY, int tokens, Node[,] map)
    {
        Point location = new Point(startX, startY);
        int index;
        //int tokens = agent.GetTokens();
        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].SetAverageHeight(false, 3);
            index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
            location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
        }
        return map;
    }

    public static Node[,] MountainSmoothing(int startX, int startY, int tokens, Node[,] map)
    {
        Point location = new Point(startX, startY);
        List<Point> locationsVisited = new List<Point>();
        int index;
        //int tokens = agent.GetTokens();
        for (int i = 0; i < tokens; i++)
        {
            locationsVisited.Add(location);
            map[location.x, location.y].SetAverageHeight(false, 3);
            do
            {
                index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
                location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
            } while (CheckPropagation(locationsVisited, location));
            
        }
        return map;
    }

    private static bool CheckPropagation(List<Point> locationsVisited, Point loaction)
    {
        foreach (Point point in locationsVisited)
        {
            if(point == loaction)
            {
                return false;
            }
        }
        return true;
    }

    //int loopCount = 0;
    //while (squares[newStartPoint.x, newStartPoint.y].visited && loopCount < 50)
    //{
    //    if (newStartPoint.x < squares.GetLength(0) && newStartPoint.y < squares.GetLength(1) && newStartPoint.x >= 0 && newStartPoint.y >= 0)
    //        newStartPoint.Move((int)agent.GetDirectionX(), (int)agent.GetDirectionY());
    //    else
    //    {
    //        loopCount++;
    //        Debug.Log("Hit border and can't find an point in the prefered direction");
    //    }
    //}

    //int count = 0;
    //while (squares[point.x, point.y].adjacentSquares[index].visited && count < max)
    //{
    //    count++;
    //    index = (start + count) % max;
    //    if (count > max)
    //    {
    //        Debug.Log("Isolated node");
    //    }
    //}
}
