using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class StickToGround : MonoBehaviour
{

    public Vector3[] stickAxis;


    private void OnCollisionEnter(Collision collision)
    {
        //get average normal
        Vector3 normal = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            normal += contact.normal;
        }
        normal /= collision.contactCount;

        normal.Normalize();

        Vector3 axis = Vector3.up;
        if (stickAxis.Length > 1)
        {
            //find closest axis within stickAxis
            int index = stickAxis.Length;
            float closestDot = 0;

            for (int i = 0; i < stickAxis.Length; i++)
            {
                float dot = Vector3.Dot((transform.rotation * stickAxis[i]).normalized, normal);
                if (dot > closestDot)
                {
                    index = i;
                    closestDot = dot;
                }
            }
            axis = stickAxis[index];
        }
        else if (stickAxis.Length == 1)
            axis = stickAxis[0];

        //change rotation and disable rigidbody

        transform.rotation = Quaternion.FromToRotation(axis, normal);
        Collider col = GetComponent<Collider>();
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, -normal);
        if (collision.collider.Raycast(ray, out hitInfo, 1))
        {
            transform.position = hitInfo.point + normal * col.bounds.extents.y / 2;
        }

        GetComponent<Rigidbody>().isKinematic = true;

    }
}
