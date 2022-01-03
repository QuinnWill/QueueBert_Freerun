using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowBlast : MonoBehaviour
{

    public GameObject blast;

    public float throwForce;

    public float rotationalForce;

    public Collider[] ignoredColliders;

    Blast current;

    Rigidbody parentRb;

    Transform camera;

    private void OnEnable()
    {
        InputEventManager.throwObject += throwBlast;
    }

    private void OnDisable()
    {
        InputEventManager.throwObject -= throwBlast;
    }

    void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        camera = Camera.main.transform;
    }

    private void throwBlast()
    {
        if (current == null)
        {
            GameObject currentBlast = Instantiate(blast, transform.position, transform.rotation);
            Rigidbody blastRb = currentBlast.GetComponent<Rigidbody>();
            current = currentBlast.GetComponent<Blast>();
            Collider colliderCurrent = current.GetComponent<Collider>();
            foreach (Collider collider in ignoredColliders)
            {
                Physics.IgnoreCollision(colliderCurrent, collider, true);
            }
            
            blastRb.velocity = parentRb.velocity;
            blastRb.AddForce(camera.forward * throwForce * 10);

            float rand = Random.value;

            blastRb.AddTorque(Vector3.up * rotationalForce * rand);
            blastRb.AddTorque(Vector3.right * rotationalForce * rand);
        }
        else
            current.DoBlast();
    }
}
