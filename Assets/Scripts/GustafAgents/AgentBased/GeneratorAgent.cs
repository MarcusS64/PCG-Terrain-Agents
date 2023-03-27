using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorAgent : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject mountainAgent;
    [SerializeField] GameObject beachAgent;
    [SerializeField] GameObject riverAgent;
    [SerializeField] public GameObject smoothingAgent;

    [SerializeField] int width, height, depth;

    AgentG agent = null;
    [SerializeField] int agentOrder;
    public bool activeAgent;

    [SerializeField] public int mountainAgents, riverAgents, smoothingAgents;

    void Start()
    {
        activeAgent = false;
        terrain = GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(width, height, depth);
        ResetHeigths();
    }

    
    void Update()
    {
        if (!activeAgent)
        {
            activeAgent = true;
            activateAgents();
        }

        if(mountainAgents <= 0)
        {
            activeAgent = false;
            mountainAgents = 1;
        }
        if (riverAgents <= 0)
        {
            activeAgent = false;
            riverAgents = 1;
        }
        if (smoothingAgents <= 0)
        {
            activeAgent = false;
            smoothingAgents = 1;
        }
    }

    private void ResetHeigths()
    {
        float[,] heights = new float[width * 2, depth * 2];

        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                heights[x, y] = 0.15f;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    private void activateAgents()
    {
        if (agentOrder == 0)
        {
            for (int i = 0; i < mountainAgents; i++)
            {
                agent = Instantiate(mountainAgent, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform).GetComponent<AgentG>();
                agent.active = true;
            }
        }
        if (agentOrder == 1)
        {
            for (int i = 0; i < riverAgents; i++)
            {
                agent = Instantiate(riverAgent, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform).GetComponent<AgentG>();
                agent.active = true;
            }
        }
        if (agentOrder == 2)
        {
            for (int i = 0; i < smoothingAgents; i++)
            {
                agent = Instantiate(smoothingAgent, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform).GetComponent<AgentG>();
                if (i % 2 == 0) agent.MovementDirection(true);
                agent.Offset(i * 2);
            }
        }

        agentOrder++;
    }
}
