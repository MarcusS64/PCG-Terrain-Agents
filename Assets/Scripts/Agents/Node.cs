using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Properties
    int x, y;
    Node parent;
    Point point;
    int score;
    float height;
    public List<Node> adjacentSquares;
    public bool visited;
    public bool queued;

    public void SetParent(Node _parent) { parent = _parent; }
    public Node GetParent() { return parent; }
    public Point GetCoordinates() { return point; }
    #endregion

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        point = new Point(x, y); //Could remove/replace this
        adjacentSquares = new List<Node>();
        height = 0f;
        visited = false;
        queued = false;
    }

    public int X()
    {
        return x;
    }

    public int Y()
    {
        return y;
    }

    public void SetAdjacentSquare(Node square)
    {
        adjacentSquares.Add(square);
    }

    public float Score(Point attractorP, Point repulsorP, Point borderP) //Why do we have to square the distance to each objective?
    {
        float distanceToAttractor = Mathf.Pow(GetDistance(attractorP), 2);
        float distanceToRepulsor = Mathf.Pow(GetDistance(repulsorP), 2);
        float distanceToEdge = Mathf.Pow(GetDistance(borderP), 2);
        //Debug.Log("distAtt: " + distanceToAttractor + " distRep: " + distanceToRepulsor + " distBor: " + distanceToEdge);
        return distanceToRepulsor - distanceToAttractor + 3 * distanceToEdge;
    }

    public float GetDistance(Point point)
    {
        return Mathf.Sqrt(Mathf.Pow(this.point.x - point.x, 2) + Mathf.Pow(this.point.y - point.y, 2));
    }

    public void SetAverageHeight(bool includeParent)
    {
        float totalHeight = 3 * height; //Bigger weight to the square we're on
        float numberOfSorroundingSquares = 1;
        foreach (Node square in adjacentSquares)
        {
            totalHeight += square.height + square.GetSorroundingHeight(this, includeParent);
            numberOfSorroundingSquares += 1 + square.adjacentSquares.Count;
        }
        height = totalHeight / numberOfSorroundingSquares;
    }

    public float GetSorroundingHeight(Node parent, bool includeParent)
    {
        float totalSorroundingHeight = 0f;
        foreach (Node square in adjacentSquares)
        {
            if (!includeParent)
            {
                if (square != parent) //Remove this for the cool effect
                    totalSorroundingHeight += square.height;
            }
            else
            {
                totalSorroundingHeight += square.height;
            }
            
        }
        return totalSorroundingHeight;
    }

    public bool SameSorroundingElevation(int radius)
    {
        bool sameElevation = true;
        radius--;
        foreach (Node node in adjacentSquares)
        {
            if (height != node.GetHeight())
            {
                sameElevation = false;
                return sameElevation;
            }
        }
        if(radius > 0)
        {
            foreach (Node node in adjacentSquares)
            {
                sameElevation = node.SameSorroundingElevation(radius);
                if (!sameElevation)
                {
                    return sameElevation;
                }

            }
        }

        return sameElevation;
    }

    public float GetHeight()
    {
        return height;
    }

    public void AddHeight(float newHeight)
    {
        if (height > 0.5f)
        {
            Debug.Log("Higher than 0.5f");
        }
        height += newHeight;
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
    }

    public bool Sorrounded()
    {
        foreach (Node neighbour in adjacentSquares)
        {
            if (!neighbour.visited)
            {
                return false;
            }
        }
        return true;
    }
}
