using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitBoxEffects : MonoBehaviour
{

    bool crouching;
    bool tryUncrouch;
    RigidbodyCharacterMovement characterMovement;

    public LayerMask crouchMask;

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
            Debug.Log("trying to uncrouch");
            Vector3 point1 = transform.position + (characterMovement.normal * 0.3f) + characterMovement.normal * (fullCollider.height / 2 - fullCollider.radius);
            Vector3 point2 = transform.position + (characterMovement.normal * 0.3f) - characterMovement.normal * (fullCollider.height / 2 - fullCollider.radius);
            Debug.DrawLine(point1, point2, Color.green);
            if (!Physics.CheckCapsule(point1, point2, fullCollider.radius, crouchMask))
            {
                Debug.Log("able to stand");
                tryUncrouch = false;
                crouching = false;
                crouchCollider.enabled = false;
                fullCollider.enabled = true;
                crouch.Invoke();
            }
            
        }
    }


    private void OnCrouchPressed()
    {
        if (!crouching)
        {
            crouching = true;
            fullCollider.enabled = false;
            crouchCollider.enabled = true;
            crouch.Invoke();
        }
    }

    private void OnCrouchReleased()
    {
        tryUncrouch = true;
    }
}
