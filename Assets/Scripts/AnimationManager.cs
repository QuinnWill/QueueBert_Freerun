using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    RigidbodyCharacterMovement characterMovement;

    private bool previousGrounded;
    private bool doDodge;

    private void OnEnable()
    {
        InputEventManager.dodge += DodgeInput;
    }

    private void OnDisable()
    {
        InputEventManager.dodge -= DodgeInput;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
        characterMovement = GetComponentInParent<RigidbodyCharacterMovement>();
        doDodge = false;
        previousGrounded = true;
    }

    
    void Update()
    {
        animator.applyRootMotion = animator.GetBool("IsInteracting");
        animator.SetFloat("MoveSpeed", rb.velocity.magnitude);
        animator.SetFloat("VelocityY", rb.velocity.y);
        if(characterMovement.grounded == previousGrounded)
            animator.SetBool("Grounded", characterMovement.grounded);
        animator.SetBool("WallRunning", characterMovement.isWallRunning);
        animator.SetBool("WallRunRight", Vector3.Cross(rb.velocity.normalized, characterMovement.normal).y < 0);
        animator.SetBool("IsCrouching", characterMovement.isCrouching);
        animator.SetBool("IsSliding", characterMovement.isSliding);

        if (doDodge && characterMovement.grounded && !animator.GetBool("IsInteracting"))
        {
            animator.SetBool("IsInteracting", true);
            animator.SetTrigger("Dodge");
            doDodge = false;
        }

        

        if(!animator.GetBool("IsInteracting") && rb.velocity.sqrMagnitude > 4)
            SolveRotation();


        previousGrounded = characterMovement.grounded;
    }

    private void SolveRotation()
    {
        Vector3 horizontalVelocity = rb.velocity - Vector3.Dot(rb.velocity.normalized, characterMovement.normal) * characterMovement.normal * rb.velocity.magnitude;
        Vector3 forwardVelocity = characterMovement.RotateInput(Vector3.forward, characterMovement.isWallRunning? Vector3.up: characterMovement.normal);
        Vector3 rightVelocity = characterMovement.RotateInput(Vector3.right, characterMovement.isWallRunning ? Vector3.up : characterMovement.normal);
        horizontalVelocity = new Vector3(Vector3.Dot(rightVelocity, rb.velocity), 0, Vector3.Dot(forwardVelocity, rb.velocity));

        if (horizontalVelocity != Vector3.zero)
        {
            Quaternion slerpRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(horizontalVelocity, Vector3.up), 10 * Time.deltaTime);
            transform.localRotation = slerpRotation;
        }
    }

    private void OnAnimatorMove()
    {
        if (!animator.GetBool("IsInteracting"))
            return;
        Vector3 newVel = animator.deltaPosition;
        Vector3 normal = characterMovement.normal;
        newVel.y = 0;
        float y = (normal.x * newVel.x + normal.z * newVel.z) / -normal.y;
        newVel.y = y;
        rb.velocity = newVel / Time.deltaTime;
    }

    private void DodgeInput()
    {
        doDodge = true;
    }
}
