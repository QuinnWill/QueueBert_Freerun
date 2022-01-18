using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 closestNormal = Vector3.up;
        float distance = float.MaxValue;
        foreach (ContactPoint contactPoint in collision.contacts)
        {
            float newDist = Vector3.Distance(transform.position, contactPoint.point);
            if (newDist < distance)
            {
                closestNormal = contactPoint.normal;
                distance = newDist;
            }
        }
        playerMovement.normal = closestNormal;
        playerMovement.grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        playerMovement.grounded = false;
        playerMovement.normal = Vector3.up;
    }

}
