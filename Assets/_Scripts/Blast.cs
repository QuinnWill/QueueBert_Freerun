using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blast : MonoBehaviour
{
    protected Rigidbody[] rigidbodies;

    public float maxForce;
    public float minForce;
    public float minRadius;
    public float maxRadius;

    public LayerMask layerMask;

    public AnimationCurve fallOffCurve;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbodies = FindObjectsOfType<Rigidbody>();
    }


    public void DoBlast()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.gameObject == this.gameObject)
                continue;
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
