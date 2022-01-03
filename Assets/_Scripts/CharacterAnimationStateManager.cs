using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationStateManager : MonoBehaviour
{

    public Animator animator;
    public PlayerMovement Player;

    [Range(0,1)]
    public float DistanceToGround;
    public LayerMask layerMask;

    private float blendVal;

    public AnimationCurve curve;



    // Update is called once per frame
    void Update()
    {
        if (Player.velocity != Vector3.zero && Player.grounded)
        {
            Quaternion rotation = Quaternion.LookRotation(Player.velocity);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 0.03f);
        }

        if (Player.grounded)
            animator.SetBool("Grounded", true);

        if (Player.grounded)
        {
            blendVal = blendVal > 1 ? 0 : blendVal + Player.inputVelocity.magnitude * 0.003f + Player.acceleration.sqrMagnitude * 2;
        }
        else
        {
            animator.SetBool("Grounded", false);
        }

        animator.SetFloat("Blend", curve.Evaluate(blendVal));

        if (Player.jump)
        {
            animator.SetBool("Grounded", false);
            animator.SetTrigger("Jump");
            Player.jump = false;
        }

        animator.SetFloat("MoveSpeed", Player.velocity.magnitude);

        /*if (Player.acceleration != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(Player.acceleration);
            transform.localRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }*/
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator) {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("RightFootIKWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("LeftFootIKWeight"));

            RaycastHit hitInfo;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Player.normal, -Player.normal);
            if (Physics.Raycast(ray, out hitInfo, DistanceToGround + 1f, layerMask))
            {
                Vector3 footPos = hitInfo.point;
                footPos.y += DistanceToGround;
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
            }

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, animator.GetFloat("RightFootIKWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat("LeftFootIKWeight"));

            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Player.normal, -Player.normal);
            if (Physics.Raycast(ray, out hitInfo, DistanceToGround + 1f, layerMask))
            {
                Vector3 footPos = hitInfo.point;
                footPos.y += DistanceToGround;
                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
            }
        }
    }

    public void OnJump()
    {
        Player.DoJump();
        
    }
}
