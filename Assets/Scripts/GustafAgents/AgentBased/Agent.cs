using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public abstract class AgentG : MonoBehaviour
{
    [SerializeField] protected int tokens;
    [SerializeField] protected Terrain terrain;
    protected float width, depth, height;

    [SerializeField] protected Vector3 position;
    [SerializeField] protected Vector3 velocity;

    [Min(0)]
    [SerializeField] protected float effectRadius;
    protected GeneratorAgent generator;
    public bool active { get; set; }

    void Awake()
    {
        terrain = GameObject.FindObjectOfType<Terrain>().GetComponent<Terrain>();
        width = terrain.terrainData.size.x;
        depth = terrain.terrainData.size.z;
        height = terrain.terrainData.size.y;
        generator = FindObjectOfType<Terrain>().GetComponent<GeneratorAgent>();
    }

    protected virtual void Update()
    {
        transform.position = position;
    }

    public void SetTokens(int amount)
    {
        tokens = amount;
    }

    public abstract void Move();

    public abstract void DoEffect();

    public abstract void SetStartPosition();

    public virtual void MovementDirection(bool b) { }

    public virtual void Offset(int offset) { }
}
