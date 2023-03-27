using System.Collections.Generic;
using UnityEngine;

public class MountainAgentG : AgentG
{
    [Range(-1f, 1f)]
    [SerializeField] float elevation;
    List<Vector3> pathList = new List<Vector3>();
    int index = 0;

    private void Start()
    {
        SetStartPosition();
        CreatePath();
    }

    protected override void Update()
    {
        base.Update();

        if (active)
        {
            DoEffect();
            Move();
        }
    }

    public override void DoEffect()
    {
        float[,] heights = terrain.terrainData.GetHeights((int)transform.position.x, (int)transform.position.z, (int)effectRadius, (int)effectRadius);

        for (int i = 0; i < effectRadius; i++)
        {
            for (int j = 0; j < effectRadius; j++)
            {
                heights[i, j] += Random.Range(elevation / 5, elevation);
            }
        }

        terrain.terrainData.SetHeights((int)transform.position.x, (int)transform.position.z, heights);
    }
    public override void Move()
    {
        Vector3 point = pathList[index];

        if (position.x <= point.x) position.x++;
        if (position.z <= point.z) position.z++;
        if (position.x >= point.x) position.x--;
        if (position.z >= point.z) position.z--;

        if (Vector3.Distance(position, point) < 2) index++;
        if (index >= pathList.Count)
        {
            active = false;
            generator.mountainAgents--;
            Destroy(gameObject);
        }
    }

    public override void SetStartPosition()
    {
        position = new Vector3(Random.Range(0 + effectRadius, width * 2 - effectRadius), 0, Random.Range(0 + effectRadius, depth * 2 - effectRadius));
    }

    private void CreatePath()
    {
        Vector3 startPos = new Vector3(position.x, position.y, position.z);
        pathList.Add(startPos);

        Vector3 goalPoint = new Vector3(Random.Range(width, width * 2 - effectRadius), 0, Random.Range(depth, depth * 2 - effectRadius));

        for (int i = tokens; i > 0; i--)
        {
            Vector3 pathPoint = new Vector3(Random.Range(startPos.x / i, goalPoint.x / i), 0, Random.Range(startPos.z / i, goalPoint.z / i));

            pathList.Add(pathPoint);
        }

        pathList.Add(goalPoint);
    }
}
