using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ThrowBlast : UsableItem
{

    public static event Action sendBlast;

    public GameObject blast;

    public float throwForce;

    public float rotationalForce;

    public Collider[] ignoredColliders;

    private GameObject current;

    Rigidbody parentRb;

    Transform currentCamera;

    protected void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        currentCamera = Camera.main.transform;
    }

    private void throwObject()
    {
        if (current == null)
        {
            current = Instantiate(blast, transform.position, transform.rotation);
            Rigidbody blastRb = current.GetComponent<Rigidbody>();
            Collider colliderCurrent = current.GetComponent<Collider>();
            foreach (Collider collider in ignoredColliders)
            {
                Physics.IgnoreCollision(colliderCurrent, collider, true);
            }
            
            blastRb.velocity = parentRb.velocity;
            blastRb.AddForce(currentCamera.forward * throwForce * 10);

            float rand = UnityEngine.Random.value;

            blastRb.AddTorque(Vector3.up * rotationalForce * rand);
            blastRb.AddTorque(Vector3.right * rotationalForce * rand);
        }

    }

    protected override void DoAction()
    {
        if (current != null)
        {
            sendBlast.Invoke();
            return;
        }
        if (uses <= 0 && limitUses)
        {
            return;
        }
        uses--;
        throwObject();
    }
}
