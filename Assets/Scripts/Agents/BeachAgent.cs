using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BeachAgent
{
    public static float heightLimit = 0.5f;
    public static float flattenUnit = 0.1f;
    public static float bottomLimit = 0.3f;
    public static (int x, int y)[] directions = new (int, int)[] { (-1, 0), (0, -1), (0, 1), (1, 0)  };
    public static (int x, int y)[] coords = new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    public static Node[,] GenerateBeach(int startX, int startY, int tokens, Node[,] map)
    {
        Point location = new Point(startX, startY);
        int index;

        //int tokens = agent.GetTokens();
        if (map[location.x, location.y].GetHeight() >= heightLimit)
        {
            location = FindShorelinePoint(location, map);
        }

        for (int i = 0; i < tokens; i++)
        {
            map[location.x, location.y].AddHeight(-1 * flattenUnit, "");
            map[location.x, location.y].SetAverageHeight(false);
            
            index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);

            location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
        }
        return map;
    }

    private static int WalkToNextPoint(Point location, Node[,] map)
    {
        int index = Random.Range(0, map[location.x, location.y].adjacentSquares.Count);
        int nrOfNeighbours = map[location.x, location.y].adjacentSquares.Count;
        int loopCount = 0;

        while (map[location.x, location.y].adjacentSquares[index].GetHeight() <= bottomLimit )
        {
            index++;
            index = index % nrOfNeighbours;
            loopCount++;
            if(loopCount >= nrOfNeighbours)
            {
                break;
            }
        }

        return index;
    }

    private static Point FindShorelinePoint(Point startingPoint, Node[,] map)
    {
        Point newStartingPoint = new Point(startingPoint.x, startingPoint.y);
        //Node newStaringPoint = map[startingPoint.x, startingPoint.y];
        int closestShoreDistance = map.GetLength(0); //Should be max with of map
        foreach (var direction in directions)
        {
            int distance = 0;
            do
            {
                startingPoint.Move(direction.x, direction.y);
                distance++;
                if(startingPoint.x > map.GetLength(0) || startingPoint.x < 0)
                {
                    break;
                }
                if(startingPoint.y > map.GetLength(1) || startingPoint.y < 0)
                {
                    break;
                }

            } while ((map[startingPoint.x, startingPoint.y].GetHeight() > 0f && map[startingPoint.x, startingPoint.y].GetHeight() < heightLimit));

            if(closestShoreDistance > distance)
            {
                newStartingPoint.SetNew(startingPoint.x, startingPoint.y);
            }
            startingPoint.Reset();
        }

        return newStartingPoint;
    }
}
