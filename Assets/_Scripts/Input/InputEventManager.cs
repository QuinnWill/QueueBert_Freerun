using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputEventManager : MonoBehaviour
{

    public static InputEventManager instance;

    private PlayerControls controls;

    public static event Action<Vector2> move;
    public static event Action jump;
    public static event Action dodge;
    public static event Action sprint;
    public static event Action crouchDown;
    public static event Action crouchUp;
    public static event Action<Vector2> cameraDelta;
    public static event Action switchCamera;
    public static event Action throwObject;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Movement.Move.performed += context => OnMove(context.ReadValue<Vector2>());
        controls.Movement.Jump.performed += _ => OnJump();
        controls.Movement.Sprint.started += _ => OnSprint();
        controls.Movement.Sprint.canceled += _ => OnSprint();
        controls.Movement.Crouch.started += _ => OnCrouchDown();
        controls.Movement.Crouch.canceled += _ => OnCrouchUp();
        controls.Actions.Dodge.performed += _ => OnDodge();
        controls.Actions.ThrowObject.started += _ => OnThrowObject();
        controls.Camera.SwitchCamera.started += _ => OnSwitchCamera();
        //controls.Camera.LookDelta.performed += context => OnCameraMove(context.ReadValue<Vector2>());

    }

    private void OnDisable()
    {
        
        controls.Movement.Move.performed -= context => OnMove(context.ReadValue<Vector2>());
        controls.Movement.Jump.performed -= _ => OnJump();
        controls.Movement.Sprint.started -= _ => OnSprint();
        controls.Movement.Sprint.canceled -= _ => OnSprint();
        controls.Movement.Crouch.started -= _ => OnCrouchDown();
        controls.Movement.Crouch.canceled -= _ => OnCrouchUp();
        controls.Actions.Dodge.performed -= _ => OnDodge();
        controls.Actions.ThrowObject.started -= _ => OnThrowObject();
        controls.Camera.SwitchCamera.started -= _ => OnSwitchCamera();
        //controls.Camera.LookDelta.performed -= context => OnCameraMove(context.ReadValue<Vector2>());
        controls.Disable();
    }

    private void OnMove(Vector2 direction)
    {
        move.Invoke(direction);
    }

    private void OnJump()
    {
        jump.Invoke();
    }

    private void OnDodge()
    {
        dodge.Invoke();
    }

    private void OnSprint()
    {
        sprint.Invoke();
    }
    private void OnCrouchDown()
    {
        crouchDown.Invoke();
    }

    private void OnCrouchUp()
    {
        crouchUp.Invoke();
    }

    private void OnCameraMove(Vector2 delta)
    {
        cameraDelta.Invoke(delta);
    }

    private void OnSwitchCamera()
    {
        switchCamera.Invoke();
    }

    private void OnThrowObject()
    {
        throwObject.Invoke();
    }


}
