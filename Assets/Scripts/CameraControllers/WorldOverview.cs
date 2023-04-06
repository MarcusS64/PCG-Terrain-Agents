using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WorldOverview : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera freeLook;
    [SerializeField] CinemachineVirtualCamera overview;
    [SerializeField] CinemachineVirtualCamera lookAtObj;

    private void OnEnable()
    {
        CameraSwitcher.Register(freeLook);
        CameraSwitcher.Register(overview);
        CameraSwitcher.Register(lookAtObj);
        CameraSwitcher.SwitchCamera(freeLook);
    }

    private void OnDisable()
    {
        CameraSwitcher.UnRegister(freeLook);
        CameraSwitcher.UnRegister(overview);
        CameraSwitcher.UnRegister(lookAtObj);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Read key press");
            if (CameraSwitcher.IsActiveCamera(freeLook))
            {
                //Debug.Log("Switch to overview");
                CameraSwitcher.SwitchCamera(overview);
            }
            else if (CameraSwitcher.IsActiveCamera(overview))
            {
                //Debug.Log("Switch to free look");
                CameraSwitcher.SwitchCamera(lookAtObj);
            }
            else if (CameraSwitcher.IsActiveCamera(lookAtObj))
            {
                //Debug.Log("Switch to follow obj");
                CameraSwitcher.SwitchCamera(freeLook);
            }
        }
    }
}
