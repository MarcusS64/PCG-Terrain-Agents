using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class DrawRectangle : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI areaText;
    private LineRenderer lineRend;
    private Vector2 initialMousePosition, currentMousePosition;
    private float area;

    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 0;
        areaText.text = "Area = " + area;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraSwitcher.IsActiveCamera(topDownCamera))
        {
            //Debug.Log("topDown is acite");
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("register press");
                lineRend.positionCount = 4;
                initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                lineRend.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
                lineRend.SetPosition(1, new Vector2(initialMousePosition.x, initialMousePosition.y));
                lineRend.SetPosition(2, new Vector2(initialMousePosition.x, initialMousePosition.y));
                lineRend.SetPosition(3, new Vector2(initialMousePosition.x, initialMousePosition.y));
                //Debug.Log("Initial mouse pos: " + initialMousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                //Debug.Log("register helt M-button");
                //Debug.Log("Initial mouse pos: " + Input.mousePosition);
                currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                lineRend.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
                lineRend.SetPosition(1, new Vector2(initialMousePosition.x, currentMousePosition.y));
                lineRend.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
                lineRend.SetPosition(3, new Vector2(currentMousePosition.x, initialMousePosition.y));

                area = Mathf.Abs((initialMousePosition.x - currentMousePosition.x) * (initialMousePosition.y - currentMousePosition.y));
                //Debug.Log("Area to draw: " + area);
                //Debug.Log("Current mouse pos: " + currentMousePosition);
                areaText.text = "Area = " + area;
            }
        }
        
    }
}
