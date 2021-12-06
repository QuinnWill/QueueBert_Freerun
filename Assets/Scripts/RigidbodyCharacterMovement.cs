using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterMovement : MonoBehaviour
{

    private Rigidbody rb;

    private Transform mainCamera;

    private Vector2 playerInput;

    private bool jump;
    private bool cancelGrounded;
    private bool doBoost;

    private float maxSpeed;

    [HideInInspector]
    public int stepsSinceLastGrounded, stepsSinceLastWallRun, stepsSinceLastJump;

    public LayerMask layerMask;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    [SerializeField, Range(0f, 1f)]
    float normalGravityPercentage;

    [SerializeField, Range(0, 100)]
    float sprintSpeed;

    [SerializeField, Range(0, 100)]
    float runSpeed;

    [SerializeField, Range(0, 100)]
    float crouchSpeed;

    public float accelerationSpeed = 5000;

    Vector3 previousNormal = Vector3.down;
    Vector3 previousVelocity;

    public bool grounded;
    public bool isSprinting;
    public bool isWallRunning;
    public bool isCrouching;
    public bool isSliding;

    
    public float gravity;
    public float minJumpHeight = 0.1f;
    public float maxJumpHeight = 1f;


    public Vector3 normal;



    private void OnEnable()
    {
        InputEventManager.jump += OnJumpInput;
        InputEventManager.move += OnMoveInput;
        InputEventManager.sprint += OnSprint;
        HitBoxEffects.crouch += OnCrouch;
    }

    private void OnDisable()
    {
        InputEventManager.jump -= OnJumpInput;
        InputEventManager.move -= OnMoveInput;
        InputEventManager.sprint -= OnSprint;
        HitBoxEffects.crouch -= OnCrouch;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normal = Vector3.up;
        mainCamera = Camera.main.transform;
    }

    void Update()
    {

        IncrementSteps();

        if (grounded && doBoost)
        {
            rb.AddForce(rb.velocity.normalized * 300);
            doBoost = false;
        }

        if (isCrouching)
            maxSpeed = crouchSpeed;
        else if (isSprinting)
            maxSpeed = sprintSpeed;
        else
            maxSpeed = runSpeed;

        if (isWallRunning)
        {

            SnapToWall();

            CheckForWall();

            if (jump)
            {
                Vector3 jumpVel = rb.velocity - Vector3.up * rb.velocity.y;
                jumpVel += normal * 20f * 0.5f;
                jumpVel += Vector3.up * 20f * 0.8f;
                
                jumpVel += (mainCamera.forward - Vector3.up * mainCamera.forward.y) * 20f * 0.2f;
                rb.velocity = jumpVel;
                isWallRunning = false;
                grounded = false;
                normal = Vector3.up;
                jump = false;
                stepsSinceLastJump = 0;
            }

        }
        else if(isSliding)
        {

            //SnapToGround();

            if (!grounded)
            {
                normal = Vector3.up;
            }
            else
                stepsSinceLastGrounded = 0;

            Quaternion camRotation = Quaternion.Euler(0, 0, 0);
            Quaternion slopeRotationMoving = Quaternion.FromToRotation(Vector3.up, normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotationMoving * camRotation, 10 * Time.deltaTime);

            if (jump)
            {
                Vector3 jumpVel = rb.velocity - Vector3.Dot(rb.velocity.normalized, normal) * normal * rb.velocity.magnitude;
                jumpVel += normal * 20f * 0.8f;
                jumpVel += Vector3.up * 20f * 0.2f;
                if (rb.velocity.sqrMagnitude > 79)
                    jumpVel += (mainCamera.forward - Vector3.up * mainCamera.forward.y) * 20f * 0.2f;
                rb.velocity = jumpVel;
                grounded = false;
                jump = false;
                isSliding = false;
                stepsSinceLastJump = 0;
            }

            if (rb.velocity.sqrMagnitude < 16 || !isCrouching)
                isSliding = false;

        }
        else
        {

            SnapToGround();

            if (!grounded)
            {
                normal = Vector3.up;
            }
            else
                stepsSinceLastGrounded = 0;

            Quaternion camRotation = Quaternion.Euler(0, 0, 0);
            Quaternion slopeRotationMoving = Quaternion.FromToRotation(Vector3.up, normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotationMoving * camRotation, 10 * Time.deltaTime);


            GetComponentInChildren<Renderer>().material.SetColor(
                "_Color", grounded ? Color.red : Color.white
            );

            if (jump)
            {
                Vector3 jumpVel = rb.velocity - Vector3.Dot(rb.velocity.normalized, normal) * normal * rb.velocity.magnitude;
                jumpVel += normal * 20f * 0.8f;
                jumpVel += Vector3.up * 20f * 0.2f;
                if(rb.velocity.sqrMagnitude > 79)
                    jumpVel += (mainCamera.forward - Vector3.up * mainCamera.forward.y) * 20f * 0.2f;
                rb.velocity = jumpVel;
                grounded = false;
                jump = false;
                stepsSinceLastJump = 0;
            }

        }
        

        previousVelocity = rb.velocity;
        previousNormal = normal;
    }

    private void FixedUpdate()
    {
        if (isWallRunning)
            WallRunMovement();
        else if (isSliding)
            SlideMovement();
        else
            Movement();
    }

    private void Movement()
    {
        Vector3 horizontalVelocity = rb.velocity - Vector3.Dot(rb.velocity.normalized, normal) * normal * rb.velocity.magnitude;
        if (grounded)
        {
            /*Vector3 tempVel = rb.velocity;
            tempVel.x /= 1 + 7f * Time.deltaTime;
            tempVel.z /= 1 + 7f * Time.deltaTime;
            rb.velocity = tempVel;*/
            // don't forget to fix the inconsistancy with the jump due to the application of the friction here
            if (playerInput.Equals(Vector2.zero))
                rb.velocity /= 1 + 10f * Time.deltaTime;
            else if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
                rb.velocity /= 1 + 2f * Time.deltaTime;
        }

        rb.AddForce(-normal * Time.deltaTime * 2000 * normalGravityPercentage);
        rb.AddForce(Vector3.down * Time.deltaTime * 2000 * (1 - normalGravityPercentage));

        

        Vector3 forward = mainCamera.forward;
        forward.y = 0;
        forward.Normalize();
        forward = RotateInput(forward, normal);
        Vector3 right = mainCamera.right;
        right.y = 0;
        right.Normalize();
        right = RotateInput(right, normal);
        Vector3 moveDir = (forward * playerInput.y + right * playerInput.x).normalized;

        //Debug.DrawLine(transform.position, transform.position + right, Color.green);
        //Debug.DrawLine(transform.position, transform.position + rb.velocity.normalized - Vector3.Dot(rb.velocity.normalized, normal) * normal, Color.red);

        float dot = Vector3.Dot(moveDir.normalized, horizontalVelocity.normalized);
        Vector3 cross = Vector3.Cross(horizontalVelocity.normalized, normal);

        if (!grounded)
        {
            moveDir /= 3;
            cross /= 3;
        }

        if (horizontalVelocity.sqrMagnitude <= maxSpeed * maxSpeed)
            rb.AddForce(moveDir * Time.deltaTime * accelerationSpeed);
        else if(dot <= 0)
            rb.AddForce(moveDir * Time.deltaTime * accelerationSpeed);
        else if (dot != 1)
            rb.AddForce(cross * Vector3.Dot(moveDir.normalized, cross) * Time.deltaTime * accelerationSpeed);

    }

    private void WallRunMovement()
    {
        Vector3 horizontalVelocity = rb.velocity - Vector3.Dot(rb.velocity.normalized, normal) * normal * rb.velocity.magnitude;

        if (playerInput.Equals(Vector2.zero))
        {
            Vector3 tempVel = rb.velocity;
            tempVel.x /= 1 + 1f * Time.deltaTime;
            tempVel.z /= 1 + 1f * Time.deltaTime;
            rb.velocity = tempVel;
        }

        rb.AddForce(Vector3.down * Time.deltaTime * 600);

        Vector3 forward = mainCamera.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = mainCamera.right;
        right.y = 0;
        right.Normalize();
        Vector3 moveDir = (forward * playerInput.y + right * playerInput.x * 0.5f).normalized;
        if (Mathf.Abs(Vector3.Dot(moveDir, normal)) < 0.6)
            moveDir = Vector3.ProjectOnPlane(moveDir, normal).normalized;
        else
            moveDir = Vector3.ProjectOnPlane(moveDir, normal);

        float dot = Vector3.Dot(moveDir.normalized, horizontalVelocity.normalized);
        Vector3 cross = Vector3.Cross(horizontalVelocity.normalized, normal);

        if (horizontalVelocity.sqrMagnitude <= maxSpeed * maxSpeed)
            rb.AddForce(moveDir * Time.deltaTime * accelerationSpeed * 0.2f);
        else if (dot <= 0)
            rb.AddForce(moveDir * Time.deltaTime * accelerationSpeed * 0.2f);
        else if (dot != 1)
            rb.AddForce(cross * Vector3.Dot(moveDir.normalized, cross) * Time.deltaTime * accelerationSpeed * 0.2f);


    }

    private void SlideMovement()
    {
        if(grounded)
            rb.velocity /= 1 + 12f * Time.deltaTime / rb.velocity.magnitude;

        rb.AddForce(Vector3.down * Time.deltaTime * 2000);

    }

    private void TryStartWallRun(Collision collision)
    {
        if (!isCrouching)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 wallNormal = collision.contacts[i].normal;
                if (Mathf.Abs(wallNormal.y) > 0.4f)
                {
                    isWallRunning = false;
                    normal = wallNormal;
                    return;
                }

                if (Mathf.Abs(wallNormal.y) < 0.4f)
                {
                    Vector3 horizontalVelocity = previousVelocity - Vector3.up * previousVelocity.y;
                    float dot = Vector3.Dot(horizontalVelocity.normalized, wallNormal);
                    if (dot < -0.1f)
                    {
                        normal = wallNormal.normalized;
                        isWallRunning = true;
                        Vector3 camForward = mainCamera.forward;
                        camForward.y = 0;
                        dot = Vector3.Dot(camForward.normalized, normal);

                        Debug.Log(dot < -0.8f);
                        if (dot > -0.8f)
                        {
                            Vector3 newVel = Vector3.ProjectOnPlane(previousVelocity, normal);
                            newVel.y /= 1.5f;
                            rb.velocity = newVel;
                        }
                        else
                            rb.velocity = Vector3.ProjectOnPlane(Vector3.up, normal).normalized * (previousVelocity.y + ((Mathf.Abs(previousVelocity.x) + Mathf.Abs(previousVelocity.z)) * 0.01f));

                    }
                }
            }
        }
    }

    private void CheckForWall()
    {
        Vector3 lastNormal = normal;
        stepsSinceLastWallRun = 2;
        normal = Vector3.up;
        isWallRunning = false;

        if (Mathf.Abs(rb.velocity.x) < 3f && Mathf.Abs(rb.velocity.z) < 3f && rb.velocity.y < -0.5f)
        {
            return;
        }

        if (Physics.Raycast(rb.position, -lastNormal, out RaycastHit hit, probeDistance, layerMask))
        {
            normal = hit.normal;
            if (Mathf.Abs(hit.normal.y) > 0.3f)
            {
                return;
            }
        }
        else
        {
            return;
        }

        normal = lastNormal;
        stepsSinceLastWallRun = 0;
        isWallRunning = true;
    }

    public Vector3 RotateInput(Vector3 input, Vector3 inputNormal)
    {
        input.y = 0;
        input.Normalize();
        float y = (inputNormal.x * input.x + inputNormal.z * input.z) / -inputNormal.y;
        input.y = y;
        
        Quaternion rotated = new Quaternion(input.z, input.y, input.x, 0);
       // Quaternion rotation = new Quaternion(normal.x * Mathf.Sin(Mathf.PI / 2), normal.y * Mathf.Sin(Mathf.PI / 2), normal.z * Mathf.Sin(Mathf.PI / 2), Mathf.Cos(Mathf.PI / 2));
        //rotated *= rotation;
        //Debug.DrawLine(transform.position, transform.position + input.normalized);
        return input.normalized;
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (true)
        {
            if ((transform.GetChild(0) != collision.gameObject))
            {
                if (collision.contacts[0].normal.y >= 0.4f)
                {
                    isWallRunning = false;
                    normal = Vector3.up;
                }
                Vector3 tempNormal = Vector3.zero;
                float normalCount = 0;
                for (int i = 0; i < collision.contactCount; i++)
                {
                    Vector3 newNormal = collision.contacts[i].normal.normalized;
                    if (Vector3.Angle(normal, newNormal) < 50)
                    {
                        tempNormal += newNormal;
                        normalCount++;
                        grounded = true;
                        cancelGrounded = false;
                        CancelInvoke(nameof(StopGrounded));
                    }
                }
                if (tempNormal != Vector3.zero)
                {
                    
                    normal = tempNormal / normalCount;
                }

                float delay = 1f;
                if (!cancelGrounded)
                {
                    cancelGrounded = true;
                    Invoke(nameof(StopGrounded), Time.deltaTime * delay);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!grounded)
        {
            TryStartWallRun(collision);
        }

    }

    private void StopGrounded()
    {
        grounded = false;
    }

    private void SnapToGround()
    {
        if (stepsSinceLastGrounded != 1 || stepsSinceLastJump <= 20)
            return;

        float speed = rb.velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return;
        }

        if (!Physics.Raycast(transform.position, -previousNormal, out RaycastHit hit, probeDistance))
        {
            return;
        }

        if (hit.rigidbody == rb)
        {
            return;
        }

        normal = hit.normal.normalized;
        grounded = true;
        float dot = Vector3.Dot(rb.velocity, hit.normal);
        if (dot > 0f)
        {
            rb.velocity = (rb.velocity - hit.normal * dot).normalized * speed;
        }
    }
    private void SnapToWall()
    {
        if (stepsSinceLastWallRun != 1 || stepsSinceLastJump <= 20)
        {
            return;
        }

        float speed = rb.velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return;
        }

        if (!Physics.Raycast(transform.position, -previousNormal, out RaycastHit hit, probeDistance))
        {
            return;
        }

        if (hit.rigidbody == rb)
        {
            return;
        }

        normal = hit.normal.normalized;
        isWallRunning = true;
        float dot = Vector3.Dot(rb.velocity, hit.normal);
        if (dot != 0f)
        {
            rb.velocity = (rb.velocity - hit.normal * dot).normalized * speed;
        }
    }

    private void IncrementSteps()
    {
        stepsSinceLastWallRun++;
        stepsSinceLastJump++;
        stepsSinceLastGrounded++;
    }

    private void OnJumpInput()
    {
        if (grounded || isWallRunning)
        {
            jump = true;
        }
    }

    private void OnMoveInput(Vector2 input)
    {
        playerInput = input;
    }

    private void OnSprint()
    {
        isSprinting = !isSprinting;
    }

    private void OnCrouch()
    {
        if (rb.velocity.sqrMagnitude > runSpeed * runSpeed / 2 && !isCrouching)
        {
            isSliding = true;
            doBoost = true;
        }
        isCrouching = !isCrouching;
    }

}
