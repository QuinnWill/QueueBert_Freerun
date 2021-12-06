using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CapsuleCollider capsule;

    BoxCollider groundedCollider;

    private CollisionDetection collisionDetection;

    protected Rigidbody rb;

    bool objectInTrigger;

    public Vector3 velocity;
    public Vector3 acceleration;
    private Vector3 prevVelocity;
    private float groundCheckPercent = 0.2f;
    public bool grounded;
    public bool prevGrounded;
    public Vector3 normal;
    public Vector3 inputVelocity;
    public float friction = 1.1f;
    public float airFriction = 1.02f;
    public float moveSpeed;
    public bool jump;

    public LayerMask layermask;

    // Start is called before the first frame update
    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        collisionDetection = GetComponent<CollisionDetection>();
        groundedCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        normal = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {

        CalculateVelocity();

        if (collisionDetection.collisionInfo.below)
        {
            velocity -= Vector3.ProjectOnPlane(normal, Vector3.up) * velocity.y;
            //velocity.y = 0;
        }


        if (grounded)
        {

            velocity = Vector3.ProjectOnPlane(velocity, normal).normalized * velocity.magnitude;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump = true;
            }
        }

        
        if (normal.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.05f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, normal) * transform.rotation, 0.05f);
        }
        //transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;

        acceleration = Vector3.Lerp(acceleration, velocity - prevVelocity, 0.001f);

        prevGrounded = grounded;
        prevVelocity = velocity;

        
    }

    private void FixedUpdate()
    {
        //collisionDetection.Move(velocity * Time.deltaTime, normal, ref velocity);
        rb.velocity = velocity * Time.deltaTime * 100;
    }

    private void CalculateVelocity()
    {
        if (!grounded)
        {
            velocity /= airFriction;
            velocity.y -= 20 * Time.deltaTime;

            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            float y = (normal.x * forward.x + normal.z * forward.z) / -normal.y;
            forward.y = y;

            Quaternion rotated = new Quaternion(forward.x, forward.y, forward.z, 0);
            Quaternion rotation = new Quaternion(normal.x * Mathf.Sin(Mathf.PI / 2), normal.y * Mathf.Sin(Mathf.PI / 2), normal.z * Mathf.Sin(Mathf.PI / 2), Mathf.Cos(Mathf.PI / 2));
            rotated *= rotation;
            Vector3 left = new Vector3(rotated.x, rotated.y, rotated.z).normalized;

            inputVelocity = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                inputVelocity += forward.normalized;
            if (Input.GetKey(KeyCode.S))
                inputVelocity += -forward.normalized;
            if (Input.GetKey(KeyCode.A))
                inputVelocity += left.normalized;
            if (Input.GetKey(KeyCode.D))
                inputVelocity += -left.normalized;

            velocity += inputVelocity.normalized * moveSpeed * 0.04f;
        }
        else
        {
            velocity /= friction;

            //velocity -= Vector3.ProjectOnPlane(Vector3.up, normal) * 4;

            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            float y = (normal.x * forward.x + normal.z * forward.z) / -normal.y;
            forward.y = y;
            Debug.DrawLine(transform.position, transform.position + new Vector3(forward.x, y, forward.z), Color.black);
            Debug.DrawLine(transform.position, transform.position + forward * 2);
            Quaternion rotated = new Quaternion(forward.x, forward.y, forward.z, 0);
            Quaternion rotation = new Quaternion(normal.x * Mathf.Sin(Mathf.PI / 2), normal.y * Mathf.Sin(Mathf.PI / 2), normal.z * Mathf.Sin(Mathf.PI / 2), Mathf.Cos(Mathf.PI / 2));
            rotated *= rotation;
            Vector3 left = new Vector3(rotated.x, rotated.y, rotated.z).normalized;
            if (normal.y < 0)
            {
                forward = -forward;
                left = -left;
            }

            inputVelocity = Vector3.zero;


            if (Input.GetKey(KeyCode.W))
                inputVelocity += forward.normalized;
            if (Input.GetKey(KeyCode.S))
                inputVelocity += -forward.normalized;
            if (Input.GetKey(KeyCode.A))
                inputVelocity += left.normalized;
            if (Input.GetKey(KeyCode.D))
                inputVelocity += -left.normalized;

            velocity += inputVelocity.normalized * moveSpeed;

            Debug.DrawLine(transform.position, transform.position + forward.normalized, Color.red);
            Debug.DrawLine(transform.position, transform.position + -forward.normalized, Color.green);
            Debug.DrawLine(transform.position, transform.position + left.normalized, Color.blue);
            Debug.DrawLine(transform.position, transform.position + -left.normalized, Color.yellow);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contacts[0].separation);
        foreach (ContactPoint contact in collision.contacts)
        {
            rb.position += contact.separation * contact.normal;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.tag.Equals("Player"))
        {
            objectInTrigger = true;
            if(collisionDetection.collisionInfo.below)
                grounded = true;
            if (grounded)
            {
                RaycastHit hitInfo;
                Vector3 averagedRay = new Vector3((-normal.x * (1 - groundCheckPercent) + velocity.normalized.x * groundCheckPercent) / 2, (-normal.y * (1 - groundCheckPercent) + velocity.normalized.y * groundCheckPercent) / 2, (-normal.z * (1 - groundCheckPercent) + velocity.normalized.z * groundCheckPercent) / 2);
                Debug.DrawLine(transform.position, transform.position + averagedRay, Color.cyan);
                Ray ray = new Ray(transform.position + -normal * (capsule.height / 2 - capsule.radius), averagedRay);
                if (Physics.Raycast(ray, out hitInfo, 3, layermask))
                {
                    float angle = Vector3.Angle(hitInfo.normal, normal);
                    if (angle < 70 && angle != 0)
                    {
                        normal = hitInfo.normal;
                        Debug.Log("normal: " + angle);
                    }

                }
            }


        }
    }

    private void OnTriggerExit(Collider other)
    {
        grounded = false;
        normal = Vector3.up;
    }

    public void DoJump()
    {
        grounded = false;
        Vector3 jumpDir = normal;
        jumpDir.y += 1;
        velocity += jumpDir.normalized * 10;
    }
}
