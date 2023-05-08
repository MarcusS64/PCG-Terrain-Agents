using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMovement : MonoBehaviour
{
    [SerializeField] GameObject myTerrain;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(myTerrain.GetComponent<IslandGenerator>().hillStartX, transform.position.y, myTerrain.GetComponent<IslandGenerator>().hillStartY);
        Debug.Log("Marker moved to: " + myTerrain.GetComponent<IslandGenerator>().hillStartX + " | " + myTerrain.GetComponent<IslandGenerator>().hillStartY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        IslandGenerator.OnAttributeChanged += MoveMarker;
    }

    private void OnDisable()
    {
        IslandGenerator.OnAttributeChanged -= MoveMarker;
    }

    public void MoveMarker()
    {
        Debug.Log("Merker moved");
        transform.position = new Vector3(myTerrain.GetComponent<IslandGenerator>().hillStartY, transform.position.y, myTerrain.GetComponent<IslandGenerator>().hillStartX);
    }
}
