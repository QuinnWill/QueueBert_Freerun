using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blast : MonoBehaviour
{
    protected List<Rigidbody> rigidbodies;

    public float maxForce;
    public float minForce;
    public float minRadius;
    public float maxRadius;

    public LayerMask layerMask;

    public AnimationCurve fallOffCurve;

    public void Start()
    {
        rigidbodies = new List<Rigidbody>();
    }

    public void DoBlast()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxRadius);
        foreach (var collider in colliders)
        {
            if (collider.attachedRigidbody != null)
            {
                Debug.Log(collider.attachedRigidbody);
                rigidbodies.Add(collider.attachedRigidbody);
            }
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                continue;

            RigidbodyCharacterMovement playerMovement = rb.GetComponent<RigidbodyCharacterMovement>();
            if (playerMovement)
            {
                if (playerMovement.isWallRunning)
                {
                    playerMovement.isWallRunning = false;
                    playerMovement.grounded = false;
                    playerMovement.normal = Vector3.up;
                }
            }
            ClimbHandler climbHandler = rb.GetComponent<ClimbHandler>();
            if (climbHandler)
            {
                if (climbHandler.isClimbing)
                    climbHandler.StopClimbing();

            }

            float distance = Vector3.Distance(rb.position, transform.position);
            if (distance <= minRadius)
            {
                rb.velocity -= rb.velocity.y / 2 * Vector3.up;
                Vector3 force = (rb.position - transform.position).normalized;
                force.y *= 1.5f;
                force.Normalize();
                rb.AddForce(force * maxForce);
            }
            else if (distance < maxRadius)
            {
                Debug.Log(rb.name);
                float fallOff = fallOffCurve.Evaluate((distance - minRadius) / maxRadius);
                Vector3 force = rb.position - transform.position;
                force.y *= 1.5f;
                force = (force).normalized * maxForce * fallOff;
                rb.velocity -= rb.velocity.y / 2 * Vector3.up;
                rb.AddForce(force);
            }
        }

        Destroy(this.gameObject);
    }

}
