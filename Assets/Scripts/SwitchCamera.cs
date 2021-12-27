using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{

    public CinemachineVirtualCameraBase[] cameras;
    public int cameraIndex;

    private void Start()
    {
        cameras[0].m_Priority = 10;
        for (int i = 1; i < cameras.Length; i++)
            cameras[i].m_Priority = -1;
    }

    private void OnEnable()
    {
        InputEventManager.switchCamera += SwitchCam;
    }

    private void OnDisable()
    {
        InputEventManager.switchCamera -= SwitchCam;
    }

    private void SwitchCam()
    {
        if (cameras != null)
        {
            CinemachineVirtualCameraBase cam1 = cameras[cameraIndex];
            cameraIndex = cameraIndex < cameras.Length - 1 ? cameraIndex + 1 : 0;
            CinemachineVirtualCameraBase cam2 = cameras[cameraIndex];

            cam1.m_Priority = -1;
            cam2.m_Priority = 10;
            Quaternion orientation = cam1.State.FinalOrientation;
            
            cam2.ForceCameraPosition(cam1.transform.position - cam1.transform.forward * 10, orientation);
        }
    }
}
