using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    Rigidbody rb;

    CapsuleCollider capsule;

    public LayerMask layerMask;

    public CollisionInfo collisionInfo;

    private float skinwidth = 0.08f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    public void Move(Vector3 velocity, Vector3 normal, ref Vector3 refVelocity)
    {
        collisionInfo.reset();
        normal.Normalize();

        if (checkY(ref velocity, normal))
        {
            refVelocity.y = 0;
            //velocity.y = 0;
        }
        if (checkX(ref velocity, normal))
        {
            refVelocity.x = 0;
            //velocity.x = 0;
        }
        if (checkZ(ref velocity, normal))
        {
            refVelocity.z = 0;
            //velocity.z = 0;
        }



        transform.position += velocity;

    }


    private bool checkY(ref Vector3 velocity, Vector3 normal)
    {
        float direction = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinwidth;
        RaycastHit hitInfo;
        if (Physics.CapsuleCast(transform.position + normal * (capsule.height / 2 - capsule.radius), transform.position - normal * (capsule.height / 2 - capsule.radius), capsule.radius, Vector3.up * direction, out hitInfo, rayLength, layerMask))
        {
            velocity.y = (hitInfo.distance - skinwidth) * direction;
            collisionInfo.below = direction == -1;
            collisionInfo.above = direction == 1;
            return true;
        }
        return false;

    }
    private bool checkX(ref Vector3 velocity, Vector3 normal)
    {
        float direction = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinwidth;
        RaycastHit hitInfo;
        if (Physics.CapsuleCast(transform.position + normal * (capsule.height / 2 - capsule.radius), transform.position - normal * (capsule.height / 2 - capsule.radius), capsule.radius, Vector3.right * direction, out hitInfo, rayLength, layerMask))
        {
            velocity.x = (hitInfo.distance - skinwidth) * direction;
            collisionInfo.left = direction == -1;
            collisionInfo.right = direction == 1;
            return true;
        }
        return false;
    }
    private bool checkZ(ref Vector3 velocity, Vector3 normal)
    {
        float direction = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + skinwidth;
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position, transform.position + normal * (capsule.height / 2 - capsule.radius));
        Debug.DrawLine(transform.position, transform.position - normal * (capsule.height / 2 - capsule.radius));
        if (Physics.CapsuleCast(transform.position + normal * (capsule.height / 2 - capsule.radius), transform.position - normal * (capsule.height / 2 - capsule.radius), capsule.radius, Vector3.forward * direction, out hitInfo, rayLength, layerMask))
        {
            velocity.z = (hitInfo.distance - skinwidth) * direction;
            collisionInfo.behind = direction == -1;
            collisionInfo.ahead = direction == 1;
            return true;
        }
        return false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool ahead, behind;
        public bool right, left;

        public void reset()
        {
            above = below = false;
            ahead = behind = false;
            right = left = false;
        }
    }
}


