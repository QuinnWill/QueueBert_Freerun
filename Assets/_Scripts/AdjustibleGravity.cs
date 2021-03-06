using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdjustibleGravity : MonoBehaviour
{

    [Range(0, 50)]
    public float gravity;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity * Time.deltaTime * 10);

        float velocityY = rb.velocity.y;

        rb.velocity = new Vector3(rb.velocity.x / 1.02f, velocityY, rb.velocity.z / 1.02f);
    }
}
