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
    public float pitch = 0;
    public float yaw = 0;

    bool isFirstPerson;
    bool canSwitch;

    void Start()
    {
        canSwitch = true;
        distance = thirdPersonDistance;
    }

    private void Update()
    {
        yaw += Mouse.current.delta.x.ReadValue() * 0.1f;
        pitch -= Mouse.current.delta.y.ReadValue() * 0.05f;

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
        Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.rotation = Quaternion.Euler(targetRotation);

        transform.position = player.position - transform.forward * distance;

    }
}
