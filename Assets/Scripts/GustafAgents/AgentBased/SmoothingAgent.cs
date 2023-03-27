using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class SmoothingAgentG : AgentG
{

    List<Vector3> pathList = new List<Vector3>();
    [SerializeField] float verticalMovment;
    [SerializeField] int horizontalMovement;
    float timer;
    bool alternativeDirection;

    private void Start()
    {
        timer = Random.value;
        SetStartPosition();
    }

    protected override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
        if (timer <= 0) active = true;

        if (active)
        {
            DoEffect();
            Move();
        }
    }
    public override void DoEffect()
    {
        float[,] heights = terrain.terrainData.GetHeights((int)transform.position.x, (int)transform.position.z, (int)effectRadius, (int)effectRadius);

        float average = 0;

        foreach (float h in heights)
        {
            average += h;
        }

        average /= heights.Length;

        for (int i = 0; i < effectRadius; i++)
        {
            for (int j = 0; j < effectRadius; j++)
            {
                heights[i, j] = average;
            }
        }

        terrain.terrainData.SetHeights((int)transform.position.x, (int)transform.position.z, heights);
    }
    public override void Move()
    {
        if (!alternativeDirection)
        {
            if (position.x >= width * 2 - effectRadius && position.z >= depth * 2 - effectRadius)
            {
                active = false;
                generator.smoothingAgents--;
                Destroy(gameObject);
            }

            if (position.x >= width * 2 - effectRadius)
            {
                position.z += 1 * horizontalMovement;
                position.x = 0;
            }

            position.x += 1 * verticalMovment;
        }
        if (alternativeDirection)
        {
            if (position.z >= width * 2 - effectRadius && position.x >= depth * 2 - effectRadius)
            {
                active = false;
                generator.smoothingAgents--;
                Destroy(gameObject);
            }

            if (position.z >= width * 2 - effectRadius)
            {
                position.x += 1 * verticalMovment;
                position.z = 0;
            }

            position.z += 1 * horizontalMovement;
        }
    }

    public override void SetStartPosition()
    {
        position = new Vector3(0, 0, 0);
    }

    public override void MovementDirection(bool b)
    {
        alternativeDirection = b;
    }

    public override void Offset(int offset)
    {
        if(!alternativeDirection) position.z += offset;
        if (alternativeDirection) position.x += offset;
    }
}
