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
    protected float fallOffRadius;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = FindObjectsOfType<Rigidbody>();
        fallOffRadius = Mathf.Sqrt(maxForce / minForce) + minRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rKey.isPressed)
            DoBlast();
    }

    public void DoBlast()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            float distance = Vector3.Distance(rb.position, transform.position);
            if (distance < minRadius)
            {
                rb.AddForce((rb.position - transform.position).normalized * maxForce);
            }
            else if (distance < fallOffRadius)
            {
                Debug.Log(rb.name);
                rb.AddForce((rb.position - transform.position).normalized * maxForce / (distance - minRadius));
            }
        }
    }
}
