using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoastalAgent 
{
    static int limit = 8;
    static Node[,] squares;

    public static void CoastLineGenerate(Agent agent, int numberOfChildren, int limit)
    {
        List<Agent> children = new List<Agent>();
        if (agent.GetTokens() > limit)
        {
            for (int i = 0; i < numberOfChildren; i++)
            {
                children.Add(new Agent());
            }
            //Debug.Log("Made two children");
            foreach (Agent child in children)
            {
                //Might need to return the index of the neighbour                
                var nodes = GetRandomNode();
                child.SetProperties(agent.GetTokens() / numberOfChildren, GetRandomParentBorderNode(agent.GetCurrentPoint()), nodes.Item1, nodes.Item2);
                child.GetNode().SetParent(agent.GetNode());
                //Tokens was removed here and thuss twice before second call of child
                if(squares[child.GetXCoord(), child.GetYCoord()].visited)
                {
                    child.SetCurrentPointByPoint(GetNewStartPoint(child));
                    //Debug.Log("Start point fix executed");
                }
                squares[child.GetXCoord(), child.GetYCoord()].visited = true;
                squares[child.GetXCoord(), child.GetYCoord()].AddHeight(0.5f); //, "child lifted"
                CoastLineGenerate(child, numberOfChildren, limit);
            }
            agent.RemoveToken(agent.GetTokens() / 2);
        }
        else
        {
            CoastWalk(agent);
        }
        //CoastWalk(agent);
        
    }

    public static Node[,] ReturnCoast()
    {
        return squares;
    }

    private static Point ClosestBorder(Point point)
    {
        Point border = new Point(0, 0);
        float distanceX;
        float distanceY;
        int limitX = squares.GetLength(0) - 1;
        int limitY = squares.GetLength(1) - 1;
        if(limitX / 2 - point.x >= 0)
        {
            distanceX = limitX / 2 - point.x;
            border.x = 0;
        }
        else
        {
            distanceX = point.x - limitX / 2;
            border.x = limitX;
        }

        if (limitY / 2 - point.y >= 0)
        {
            distanceY = limitY / 2 - point.y;
            border.y = 0;
        }
        else
        {
            distanceY = point.y - limitY / 2;
            border.y = limitY;
        }

        if (distanceX < distanceY)
        {
            border.x = point.x;
        }
        else
        {
            border.y = point.y;
        }

        return border;
    }

    private static Node GetRandomParentBorderNode(Point point)
    {
        int max = squares[point.x, point.y].adjacentSquares.Count;
        int start = Random.Range(0, max);      
        int index = start;
        
        for (int i = 0; i < max; i++)
        {
            index = (start + i) % max;
            if (!squares[point.x, point.y].adjacentSquares[index].visited)
            {
                return squares[point.x, point.y].adjacentSquares[index];
            }
        }        
        //Debug.Log("Could not find a new node for child");
        //squares[point.x, point.y].adjacentSquares[index].visited = true;
        return squares[point.x, point.y].adjacentSquares[index];
        
    }

    public static void SetSquaresMap(Node[,] nodes)
    {
        //squares = new Node[nodes.GetLength(0), nodes.GetLength(1)];
        //for (int i = 0; i < nodes.GetLength(0); i++)
        //{
        //    for (int j = 0; j < nodes.GetLength(1); j++)
        //    {
        //        squares[i, j] = nodes[i, j];
        //    }
        //}
        squares = nodes;
    }

    public static Point GetNewStartPoint(Agent agent)
    {
        Point startPoint = new Point(agent.GetXCoord(), agent.GetYCoord());
        Point newStartPoint = new Point(agent.GetXCoord(), agent.GetYCoord());        
        for (int i = 0; i < squares.GetLength(0); i++)
        {
            if(newStartPoint.x >= squares.GetLength(0) || newStartPoint.x < 0 || newStartPoint.y >= squares.GetLength(1) || newStartPoint.y < 0)
            {
                agent.GetNewDirection();
                newStartPoint = startPoint;
            }

            if(!squares[newStartPoint.x, newStartPoint.y].visited)
            {
                return newStartPoint;
            }
            else
            {
                newStartPoint.Move((int)agent.GetDirectionX(), (int)agent.GetDirectionY());
            }
            //Debug.Log("Something went wrong and could not find a new point when sorrounded. Tokens: " + agent.GetTokens());
        }
        //Debug.Log("New start not found");
        //Debug.Log("Direction: " + agent.GetDirectionX() + " " + agent.GetDirectionY());
        return newStartPoint;
    }

    private static (Node, Node) GetRandomNode() //Might have to check that it's not the one we're standing on already as a start
    {
        Node repulsor = squares[Random.Range(0, squares.GetLength(0)), Random.Range(0, squares.GetLength(1))];
        Node attractor = squares[Random.Range(0, squares.GetLength(0)), Random.Range(0, squares.GetLength(1))];

        return (repulsor, attractor);
    }

    public static void CoastWalk(Agent agent)
    {
        //Debug.Log("Started working with tokens");
        for (int i = 0; i < agent.GetTokens(); i++)
        {        
            
            if (squares[agent.GetCurrentPoint().x, agent.GetCurrentPoint().y].Sorrounded())
            {
                //Debug.Log("Sorrounded");
                agent.SetCurrentPointByPoint(GetNewStartPoint(agent));
                
            }
            
            float maxScore = -100;
            foreach (Node square in squares[agent.GetCurrentPoint().x, agent.GetCurrentPoint().y].adjacentSquares)
            {
                //There is an issue here if the attractor is the point it is already standing on
                if (square.Score(agent.GetAttractor(), agent.GetRepolsor(), ClosestBorder(agent.GetCurrentPoint())) > maxScore && !square.visited)
                {
                    agent.SetCurrentPoint(square.X(), square.Y());                    
                }
            }
            
            if(!squares[agent.GetXCoord(), agent.GetYCoord()].visited)
            {
                squares[agent.GetXCoord(), agent.GetYCoord()].visited = true;
                squares[agent.GetXCoord(), agent.GetYCoord()].AddHeight(0.5f); //, "coastal walk lifted"
            }
            //else
            //{
            //    Debug.Log(agent.GetCurrentPoint().x + " " + agent.GetCurrentPoint().y + " :|||: " + agent.GetAttractor().x + " " + agent.GetAttractor().y);
            //}
            
            //agent.RemoveToken();
        }
    }

}
