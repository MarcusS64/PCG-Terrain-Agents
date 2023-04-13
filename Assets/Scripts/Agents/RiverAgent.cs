using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RiverAgent
{
    static List<Node> pathList = new List<Node>();
    static Queue<Node> tabooList = new Queue<Node>();
    static float maxDistance;
    public static (int x, int y)[] directions = new (int, int)[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
    public static Node[,] GenerateRiver(int tokens, Node[,] map, float coastLimit) //int minimumLength
    {
        maxDistance = Mathf.Sqrt(Mathf.Pow(map.GetLength(0), 2) + Mathf.Pow(map.GetLength(1), 2));
        bool done = false;
        int safetyCounter = 0;
        do
        {
            pathList.Clear();
            tabooList.Clear();
            Point location = GetRandomBorderPoint(map, coastLimit);
            Point goal = new Point(map.GetLength(0) / 2, map.GetLength(1) / 2);
            //Debug.Log(goal.x + " : " + goal.y);

            if (location.x != -1 && location.y != -1)
            {
                pathList.Add(map[location.x, location.y]);
                tabooList.Enqueue(map[location.x, location.y]);
                int index;
                done = true;
                for (int i = 0; i < tokens; i++)
                {
                    //map[location.x, location.y].SetAverageHeight(true, 3);
                    //map[location.x, location.y].SetHeight(1f);            
                    index = GetNextNodeIndex(map[location.x, location.y], goal, tabooList, coastLimit);
                    if (index < 0)
                    {
                        break;
                    }
                    location.SetNew(map[location.x, location.y].adjacentSquares[index].X(), map[location.x, location.y].adjacentSquares[index].Y());
                    pathList.Add(map[location.x, location.y]);
                    if (goal.x == location.x && goal.y == location.y)
                    {
                        done = true;
                        break;
                    }

                    //tabooList.Dequeue();
                    tabooList.Enqueue(map[location.x, location.y]);                    
                }

                if(pathList.Count == tokens + 1)
                {
                    done = true;
                }

                if (done)
                {
                    for (int i = pathList.Count - 1; i > -1; i--) //Do this for all the neighbours too
                    {
                        pathList[i].SetHeight(0.5f);
                        pathList[i].AddHeight(-0.1f);
                        foreach (Node neighbour in pathList[i].adjacentSquares)
                        {
                            neighbour.SetHeight(0.5f);
                            neighbour.AddHeight(-0.1f);
                        }
                    }

                    foreach (Node node in pathList)
                    {
                        SmoothingAgent.Smooth(node.X(), node.Y(), 4, map);
                    }
                    Debug.Log("Number of nodes in path:" + pathList.Count);
                }
                
            }
            safetyCounter++;
        } while (!done && safetyCounter < 5);
        
        if(safetyCounter == 5)
        {
            Debug.Log("Path for River failed 5 times!");
        }

        return map;
    }

    private static int GetNextNodeIndex(Node currentNode, Point goal, Queue<Node> tabooList, float coastLimit) //Could check for a height limit for the river to climb and stop it sooner
    {
        int index = -1;
        float minDistance = maxDistance;
        float minHeight = 100f;
        bool isTaboo = false;
        for (int i = 0; i < currentNode.adjacentSquares.Count; i++)
        {
            foreach(Node node in tabooList)
            {
                if(node == currentNode.adjacentSquares[i])
                {
                    isTaboo = true;
                }
            }
            if(currentNode.adjacentSquares[i].GetHeight() <= minHeight && !isTaboo && currentNode.adjacentSquares[i].GetHeight() >= 0.25f && currentNode.adjacentSquares[i].GetHeight() < 0.85f) //Check node with the smallest height
            {
                float distance = Mathf.Sqrt(Mathf.Pow(goal.x - currentNode.adjacentSquares[i].X(), 2) + Mathf.Pow(goal.y - currentNode.adjacentSquares[i].Y(), 2)); //Check node closest to the goal point
                //Debug.Log(distance);
                if (distance < minDistance)
                {
                    minHeight = currentNode.adjacentSquares[i].GetHeight();
                    minDistance = distance;
                    index = i;
                }
            }
            isTaboo = false;
        }

        return index;
    }

    private static Point GetRandomBorderPoint(Node[,] map, float coastLimit)
    {
        int borderCoordiante = Random.Range(0, map.GetLength(0));
        int axisIndex = Random.Range(0, 4);
        int directionIndex;
        Point start;
        if (axisIndex == 0) { start = new Point(map.GetLength(0) - 1, borderCoordiante); directionIndex = 0; }
        else if (axisIndex == 1) { start = new Point(borderCoordiante, map.GetLength(1) - 1); directionIndex = 1; }
        else if (axisIndex == 2) { start = new Point(borderCoordiante, 0); directionIndex = 2; }
        else { start = new Point(0, borderCoordiante); directionIndex = 3; }

        bool done = false;
        int breakPointCounter = 0;
        while (!done)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                //Debug.Log(start.x + " : " + start.y);
                if (map[start.x, start.y].GetHeight() >= coastLimit)
                {
                    return start;
                }
                start.Move(directions[directionIndex].x, directions[directionIndex].y);
                
            } 

            breakPointCounter++;
            start.Reset();
            if(directions[directionIndex].x == 0)
            {
                start.x += breakPointCounter * (map.GetLength(0) / 5);
                start.x %= map.GetLength(0);
            }
            else
            {
                start.y += breakPointCounter * (map.GetLength(1) / 5);
                start.y %= map.GetLength(1);
            }


            if (breakPointCounter > 5) //
            {
                done = true;
            }
        }

        start.SetNew(-1, -1);
        Debug.Log("No coast found to make a river");
        return start;
                        
    }
}
