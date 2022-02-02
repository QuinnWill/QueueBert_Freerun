    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraBasic : MonoBehaviour
{

    public Transform player;

    public float thirdPersonDistance = 0.03f;
    public float firstPersonDistance = 7;

    float distance;

    public CameraBounds boundsX;
    public CameraBounds boundsY;

    public float pitch = 0;
    public float yaw = 0;

    bool isFirstPerson;
    bool canSwitch;

    private void OnEnable()
    {
        InputEventManager.cameraDelta += OnCameraDelta;
        InputEventManager.switchCamera += SwitchCamera;
    }

    private void OnDisable()
    {
        InputEventManager.cameraDelta -= OnCameraDelta;
        InputEventManager.switchCamera -= SwitchCamera;
    }

    void Start()
    {
        canSwitch = true;
        distance = thirdPersonDistance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        if (Keyboard.current.gKey.IsPressed() && canSwitch)
        {
            canSwitch = false;
            isFirstPerson = !isFirstPerson;
            if (isFirstPerson)
                distance = firstPersonDistance;
            else
                distance = thirdPersonDistance;
        }
        else if (!Keyboard.current.gKey.IsPressed())
            canSwitch = true;


    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetRotation = new Vector3(-pitch, yaw);
        transform.rotation = Quaternion.Euler(targetRotation);

        transform.position = player.position - transform.forward * distance;

    }

    private void OnCameraDelta(Vector2 delta)
    {
        Debug.Log("moveCamera");
        yaw += delta.x;
        pitch += delta.y;

        if (yaw > boundsX.max)
            yaw = boundsX.wrap ? boundsX.min : boundsX.max;
        else if (yaw < boundsX.min)
            yaw = boundsX.wrap ? boundsX.max : boundsX.min;

        if (pitch > boundsY.max)
            pitch = boundsY.wrap ? boundsY.min : boundsY.max;
        else if (pitch < boundsY.min)
            pitch = boundsY.wrap ? boundsY.max : boundsY.min;



    }

    private void SwitchCamera()
    { 
        
    }
}

[System.Serializable]
public class CameraBounds
{
    public float max;
    public float min;

    public bool wrap;
}
