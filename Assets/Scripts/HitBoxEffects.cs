using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitBoxEffects : MonoBehaviour
{

    bool crouching;
    bool tryUncrouch;
    RigidbodyCharacterMovement characterMovement;

    [HideInInspector]
    public static event Action crouch;

    [SerializeField]
    CapsuleCollider fullCollider;
    [SerializeField]
    CapsuleCollider crouchCollider;

    private void Start()
    {
        characterMovement = GetComponentInParent<RigidbodyCharacterMovement>();
    }

    private void OnEnable()
    {
        InputEventManager.crouchDown += OnCrouchPressed;
        InputEventManager.crouchUp += OnCrouchReleased;
    }

    private void OnDisable()
    {
        InputEventManager.crouchDown -= OnCrouchPressed;
        InputEventManager.crouchUp -= OnCrouchReleased;
    }

    private void Update()
    {
        if (tryUncrouch)
        {
            fullCollider.enabled = true;
            Ray ray = new Ray(transform.position, Vector3.up);
            if (!Physics.Raycast(ray, 1.1f))
            {
                tryUncrouch = false;
                crouching = false;
                crouchCollider.enabled = false;
                crouch.Invoke();
            }
            
        }
    }


    private void OnCrouchPressed()
    {
        crouching = true;
        fullCollider.enabled = false;
        crouchCollider.enabled = true;
        crouch.Invoke();
    }

    private void OnCrouchReleased()
    {
        tryUncrouch = true;
    }
}
