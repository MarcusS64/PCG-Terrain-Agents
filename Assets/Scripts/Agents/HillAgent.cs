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
    public static float lambda;

    public static Node[,] GenerateHills(int startX, int startY, Node[,] map, float maxHeight, float minHeight, int hillTokens, float maxlambda, float minlambda, int waveTokens, float maxPhaseShift, float minPhaseShift) 
    {
        hillHeight = Random.Range(minHeight, maxHeight);
        Point location = new Point(startX, startY);
        //(int x, int y) travelDirection = directions[Random.Range(0, directions.Length)];
        //Vector3 direction = new Vector3(travelDirection.x, 0, travelDirection.y);
        //map[startX, startY].SetHeight(2.0f); //We set this in PathFinding() during the first loop
        Debug.Log("Hill peak lifted. Hill tokens count is: " + hillTokens);
        //Do Pathfinding for a number of peaks specified by the user around the area.
        //Take a random point around the hill and do antoher hill there 
        //Sidenote: stop the hill egeration when the height hits the same height as it already has (somehow)
        PathFinding(map, map[startX, startY], hillTokens, maxHeight, minHeight, waveTokens, maxlambda, minlambda, maxPhaseShift, minPhaseShift); //map[startX + radius, startY + radius]
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
                child.SetAverageHeight(true, 3);
            }
        }
    }

    private static void InterferenceHill(Node[,] graph, Node start, int tokens,float maxHeight, float minHeight,
        int waveTokens, float maxlambda, float minlambda, float maxPhaseShift, float minPhaseShift)
    {
        float amplitude = Random.Range(minHeight, maxHeight); //max Height of the wave
        float lambda = Random.Range(minlambda, maxlambda);
        float phaseshift = Random.Range(minPhaseShift, maxPhaseShift);

        myQueue.Clear();
        myQueue.Enqueue(start);
        start.queued = true;
        removeQueue.Clear();
        removeQueue.Add(start);

        while (myQueue.Count > 0)
        {
            Node currentTile = myQueue.Dequeue();
            tokens--;
            for (int j = 0; j < currentTile.adjacentSquares.Count; j++)
            {
                if (!currentTile.adjacentSquares[j].queued)
                {
                    myQueue.Enqueue(currentTile.adjacentSquares[j]);

                    if (tokens <= 0)
                    {
                        foreach (Node node in removeQueue)
                        {
                            node.queued = false;
                        }
                        return;
                    }
                    currentTile.adjacentSquares[j].queued = true;
                    removeQueue.Add(currentTile.adjacentSquares[j]);

                    //currentTile.SetHeight(0.5f + WaveFunction(start, currentTile, waveTokens, amplitude, lambda, phaseshift));
                    //Debug.Log("Set average height");
                }
            }
        }


    }

    public static Queue<Node> myQueue = new Queue<Node>(); //First in first out
    public static List<Node> removeQueue = new List<Node>();

    public static void PathFinding(Node[,] graph, Node start, int tokens, float maxHeight, float minHeight, 
        int waveTokens, float maxlambda, float minlambda, float maxPhaseShift, float minPhaseShift)
    {
        myQueue.Clear();
        myQueue.Enqueue(start);
        start.queued = true;
        removeQueue.Clear();
        removeQueue.Add(start);
        Debug.Log("Queue count is: " + myQueue.Count);

        List<float> amplitudes = new List<float>();
        List<float> lambdas = new List<float>();
        List<float> phaseshifts = new List<float>();
        List<Node> startingPoints = new List<Node>();
        for (int i = 0; i < waveTokens; i++)
        {
            amplitudes.Add(Random.Range(minHeight, maxHeight)); //max Height of the wave
            lambdas.Add(Random.Range(minlambda, maxlambda));
            phaseshifts.Add(Random.Range(minPhaseShift, maxPhaseShift));
            startingPoints.Add(graph[start.X() + Random.Range(-50, 50), start.Y() + Random.Range(-50, 50)]); //Could change to Range(-50, 50)
        }

        while (tokens > 0)
        {
            Node currentTile = myQueue.Dequeue();            
            tokens--;
            for (int i = 0; i < currentTile.adjacentSquares.Count; i++)
            {
                if (!currentTile.adjacentSquares[i].queued) 
                {
                    myQueue.Enqueue(currentTile.adjacentSquares[i]);                    
                    //currentTile.adjacentSquares[i].SetParent(currentTile);

                    if (tokens <= 0) //currentTile.adjacentSquares[i] == goal
                    {
                        //RetracePath(start, goal);
                        
                        break; //Break the for loop
                    }
                    currentTile.adjacentSquares[i].queued = true; //Need to untag the children somehow otherwise they will be blocked
                    removeQueue.Add(currentTile.adjacentSquares[i]);                   
                }
            }
            float hillvalue = WaveFunction(start, currentTile, waveTokens, amplitudes, lambdas, phaseshifts, startingPoints);
            if(hillvalue > 0)
            {
                currentTile.SetHeight(0.5f + hillvalue);
            }
            
            //currentTile.AddHeight(WaveFunction(start, currentTile, waveTokens, amplitudes, lambdas)); 
            //Adding heigh required flatten it back out therwide it will just keep growing
            //Debug.Log("Set average height");
        }

        //BlendBorder(myQueue, removeQueue, waveTokens, amplitudes, lambdas, phaseshifts, startingPoints);
        SmoothBorder(myQueue, removeQueue, graph);
        foreach (Node node in removeQueue)
        {
            node.queued = false;
        }

    }

    private static void SmoothBorder(Queue<Node> myQueue, List<Node> removeQueue, Node[,] map)
    {
        int count = 2000;
        while(myQueue.Count > 0 && count > 0)
        {
            Node currentTile = myQueue.Dequeue();
            float oldTileHeight = currentTile.GetHeight();
            //currentTile.SetHeight(1);
            currentTile.SetAverageHeight2(false, 1);
            //SmoothingAgent.Smooth(currentTile.X(), currentTile.Y(), 3, map);
            count--;
            if(Mathf.Abs(oldTileHeight - currentTile.GetHeight()) > 0.0001f) //Height delta
            {
                for (int i = 0; i < currentTile.adjacentSquares.Count; i++)
                {
                    if (!currentTile.adjacentSquares[i].queued)
                    {
                        myQueue.Enqueue(currentTile.adjacentSquares[i]);

                        currentTile.adjacentSquares[i].queued = true; //Need to untag the children somehow otherwise they will be blocked
                        removeQueue.Add(currentTile.adjacentSquares[i]);
                    }
                }
            }            
        }

    }

    private static void BlendBorder(Queue<Node> myQueue, List<Node> removeQueue, int waveTokens, List<float> amplitudes, List<float> lambdas, List<float> phaseshifts, List<Node> startingPoints)
    {
        int count = 4000;
        while (myQueue.Count > 0 && count > 0)
        {
            Node currentTile = myQueue.Dequeue();
            currentTile.SetHeight(0.5f + WaveFunction(currentTile, currentTile, waveTokens, amplitudes, lambdas, phaseshifts, startingPoints));
            count--;
           
            if(currentTile.GetHeight() - 0.5f > 0.0001f)
            {
                for (int i = 0; i < currentTile.adjacentSquares.Count; i++)
                {
                    if (!currentTile.adjacentSquares[i].queued)
                    {
                        myQueue.Enqueue(currentTile.adjacentSquares[i]);

                        currentTile.adjacentSquares[i].queued = true; //Need to untag the children somehow otherwise they will be blocked
                        removeQueue.Add(currentTile.adjacentSquares[i]);
                    }
                }
            }
            

            
        }

        Debug.Log("loop count at: " + count);
    }

    //Use the Queue to add the points by BFS
    //Loop through the queue and calculate the distance from the start
    //Using the period and the distance we should be able to set an apropriate height using the wave equation
    //Don't forget to decrease the elevation (aplitude) as we move away from the centre
    public static float WaveFunction(Node start, Node current, int waveTokens, List<float> amplitudes, List<float> lambdas, List<float> phaseShifts, List<Node> startingPoints)
    {
        //float lambda = 2f; //period normalize the distance to fit the 2*Mathf.PI 

        //float distance = Mathf.Sqrt(Mathf.Pow(start.X() - current.X(), 2) + Mathf.Pow(start.Y() - current.Y(), 2));
        //Send in a list of multiple starting points and calculate the distance from each of these to add up the height in that position

        float height = 0;
        
        for (int i = 0; i < waveTokens; i++) //Note each position will only have one wave from its origin
        {
            float distance = Mathf.Sqrt(Mathf.Pow(startingPoints[i].X() - current.X(), 2) + Mathf.Pow(startingPoints[i].Y() - current.Y(), 2));
            if (i % 2 == 0)
            {
                height += amplitudes[i] * Mathf.Cos((2 * Mathf.PI / lambdas[i]) * distance + phaseShifts[i]);
            }
            else
            {
                height += amplitudes[i] * Mathf.Sin((2 * Mathf.PI / lambdas[i]) * distance + phaseShifts[i]);
            }
        }
        return height;
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
