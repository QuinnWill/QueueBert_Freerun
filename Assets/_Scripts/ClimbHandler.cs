using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbHandler : MonoBehaviour
{

    private Rigidbody rb;
    public RigidbodyCharacterMovement playerMovement;

    public Animator animator;

    public List<GameObject> visualTransforms;

    private Collider climbObject;

    private bool canClimb;
    private bool jump;

    private Vector3 rightHandPos;
    private Vector3 leftHandPos;

    private Vector2 moveInput;

    public float distanceFromWall;

    public float hangOffset;
    public float hangHeight;
    [Range(0.1f, 10)]
    public float hangSpeed;

    public float climbSpeed;

    public bool isClimbing { get; private set; }

    private void OnEnable()
    {
        InputEventManager.move += HandleMoveInput;
        InputEventManager.crouchDown += StopClimbing;
        InputEventManager.jump += OnJump;
    }

    private void OnDisable()
    {
        InputEventManager.move -= HandleMoveInput;
        InputEventManager.crouchDown -= StopClimbing;
        InputEventManager.jump -= OnJump;
    }

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        canClimb = true;

        rightHandPos = leftHandPos = Vector3.zero;
    }

    private void Update()
    {
        if (jump && isClimbing)
        {
            Vector3 jumpVel = Vector3.up * 15f;

            rb.velocity = jumpVel;
            StopClimbing();
            jump = false;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            Vector3 hangPosition = transform.position;
            hangPosition.y = climbObject.transform.position.y;

            hangPosition = climbObject.ClosestPoint(hangPosition);

            RaycastHit hitInfo;

            Vector3 normal = climbObject.transform.forward;

            if (Physics.Raycast(hangPosition, climbObject.transform.position - hangPosition, out hitInfo))
            {
                Debug.Log("normal on climbable: " + hitInfo.normal);
                normal = hitInfo.normal;
            }

            Vector3 climbRight = Vector3.Cross(Vector3.up, normal);

            rb.velocity /= 1.5f;

            rb.AddForce(Vector3.Dot(normal, Camera.main.transform.forward) * climbRight * moveInput.x * 10 * climbSpeed);

            float bodyPosition = climbObject.transform.position.y - hangOffset;

            Vector3 bodyPos = rb.position;

            if (moveInput.y > 0)
            {
                bodyPos.y = Mathf.Lerp(bodyPos.y, bodyPosition + hangHeight, hangSpeed / 20);
            }
            else if (moveInput.y < 0)
            {
                bodyPos.y = Mathf.Lerp(bodyPos.y, bodyPosition - hangHeight / 2, hangSpeed / 20);
            }
            else
            {
                bodyPos.y = Mathf.Lerp(bodyPos.y, bodyPosition, hangSpeed / 40);
            }

            rb.position = bodyPos;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (climbObject != other && other.tag == "Climbable")
        {
            if (rb.velocity.y < 0.5f && canClimb && !isClimbing)
            {
                isClimbing = true;
                animator.SetBool("IsClimbing", true);
                rb.velocity = Vector3.zero;
                playerMovement.isClimbing = true;
                //playerMovement.enabled = false;

                climbObject = other;

                Vector3 hangPosition = transform.position;
                hangPosition.y = climbObject.transform.position.y;

                hangPosition = climbObject.ClosestPoint(hangPosition);

                RaycastHit hitInfo;

                Vector3 normal = climbObject.transform.forward;

                if (Physics.Raycast(hangPosition, climbObject.transform.position - hangPosition, out hitInfo))
                {
                    Debug.Log("normal on climbable: " + hitInfo.normal);
                    normal = hitInfo.normal;
                }

                foreach (GameObject obj in visualTransforms)
                {
                    if (obj.activeSelf)
                    {
                        obj.transform.forward = -normal;
                    }
                }

                

                

                /*leftHandPos = other.bounds.ClosestPoint(transform.position + Vector3.up * 2 + transform.right * 0.3f);
                rightHandPos = other.bounds.ClosestPoint(transform.position + Vector3.up * 2 - transform.right * 0.3f);

                animator.applyRootMotion = true;*/
            }
        }
    }

    public void AnimatorIK(int layerIndex)
    {
        if (isClimbing && layerIndex == 5)
        {
            leftHandPos = climbObject.bounds.ClosestPoint(transform.position + Vector3.up * 2 + transform.right * 0.3f);
            rightHandPos = climbObject.bounds.ClosestPoint(transform.position + Vector3.up * 2 - transform.right * 0.3f);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos);


            //transform.position = CalculateHangDistance(leftHandPos, rightHandPos, hangHeight, distanceFromWall);
        }
    }

    public void AnimatorMove(Animator moveAnim)
    {
        if (moveAnim.applyRootMotion)
        {
            Vector3 rootVelocity = moveAnim.deltaPosition / Time.deltaTime;

            rootVelocity.x *= 1.2f;
            rootVelocity.z *= 1.2f;

            rb.velocity = rootVelocity;

        }
    }

    private void StopClimbing()
    {
        if (isClimbing)
        {
            isClimbing = false;
            animator.SetBool("IsClimbing", false);
            playerMovement.isClimbing = false;
            climbObject = null;
            canClimb = false;
            Invoke("ResetCanClimb", 0.5f);
            animator.applyRootMotion = false;
        }
    }

    private void ResetCanClimb()
    {
        canClimb = true;
    }

    private void HandleMoveInput(Vector2 input)
    {
        moveInput = input.normalized;
    }

    private void OnJump()
    {
        jump = true;
        Invoke("CancelJump", 0.15f);
    }

    private void CancelJump()
    {
        jump = false;
    }
}
