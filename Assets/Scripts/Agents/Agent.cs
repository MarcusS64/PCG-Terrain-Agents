using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent //: MonoBehaviour
{
    [SerializeField] int tokens;
    //[SerializeField] Vector2 seedPoint;
    [SerializeField] Vector2 direction;
    [SerializeField] List<Point> ridge;
    Point currentPoint;
    Point repulsor, attractor;
    Node myNode;

    public void SetProperties(int tokens, Node startPoint, Node repulsorPoint, Node attractorPoint)
    {
        this.tokens = tokens;
        //seedPoint = new Vector2(seedX, seedY);
        do
        {
            direction.x = Random.Range(-1, 2);
            direction.y = Random.Range(-1, 2);
        } while (direction.x + direction.y == 0);

        myNode = startPoint;

        currentPoint = new Point(myNode.X(), myNode.Y());
        repulsor = new Point(repulsorPoint.X(), repulsorPoint.Y());
        attractor = new Point(attractorPoint.X(), repulsorPoint.Y());
    }
    
    public int GetTokens()
    {
        return tokens;
    }

    public void RemoveToken(int amount)
    {
        tokens -= amount;
    }

    public Point GetRandomSeed()
    {
        return ridge[Random.Range(0, ridge.Count)];
    }

    public Point GetRepolsor()
    {
        return repulsor;
    }

    public Point GetAttractor()
    {
        return attractor;
    }

    public void SetRepulsor(int xCoord, int yCoord)
    {
        repulsor.x = xCoord;
        repulsor.y = yCoord;
    }

    public void SetAttractor(int xCoord, int yCoord)
    {
        attractor.x = xCoord;
        attractor.y = yCoord;
    }

    public Point GetCurrentPoint()
    {
        return currentPoint;
    }

    public int GetXCoord()
    {
        return currentPoint.x;
    }

    public int GetYCoord()
    {
        return currentPoint.y;
    }
    
    public void SetCurrentPoint(int xCoord, int yCoord)
    {
        currentPoint.x = xCoord;
        currentPoint.y = yCoord;
    }

    public void SetCurrentPointByPoint(Point newPoint)
    {
        currentPoint.x = newPoint.x;
        currentPoint.y = newPoint.y;
    }

    public Node GetNode()
    {
        return myNode;
    }

    public Node GetRandomNeighbour() //Check if visited
    {
        return myNode.adjacentSquares[Random.Range(0, myNode.adjacentSquares.Count)];
    }

    public float GetDirectionX()
    {
        return direction.x;
    }

    public float GetDirectionY()
    {
        return direction.y;
    }

    public void GetNewDirection()
    {
        direction.x *= -1f;
        direction.y *= -1f;
    }
}

public class Point
{
    public int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void SetNew(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void Move(int x, int y)
    {
        this.x += x;
        this.y += y;
    }
}
