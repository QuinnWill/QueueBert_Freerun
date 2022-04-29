using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeThrow : UsableItem
{

    public float maxRopeLength;

    public Rigidbody rb;

    public bool hooked;

    public LayerMask layerMask;

    private LineRenderer lineRenderer;

    private Vector3 hookPosition;
    private float hookLength;




    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update()
    {
        if (hooked)
        {

            Debug.Log("Hook Dot: " + Vector3.Dot(rb.velocity.normalized, (rb.position - hookPosition).normalized));
            float distance = Vector3.Distance(rb.position, hookPosition);
            if (distance < hookLength)
            {
                hookLength = distance;
            }
            else if (distance > hookLength && Vector3.Dot(rb.velocity.normalized, (rb.position - hookPosition).normalized) > -0.7f)
            {
                rb.velocity += Vector3.Dot(rb.velocity, (rb.position - hookPosition).normalized) * (-rb.position + hookPosition).normalized;

                rb.velocity += (-rb.position + hookPosition).normalized * (Vector3.Distance(rb.position, hookPosition) - hookLength);


            }
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void DoHook()
    {
        if (!hooked)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, maxRopeLength, layerMask))
            {
                Debug.Log("ropeAttatchPoint: " + hitInfo.point);
                hooked = true;
                hookPosition = hitInfo.point;
                hookLength = hitInfo.distance;
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(1, hitInfo.point);
            }
            else
            {
                Debug.Log("ropeAttatchPoint: MISSED");
            }
        }
    }

    public void UnHook()
    {
        if (hooked)
        {
            lineRenderer.enabled = false;
            hookPosition = Vector3.zero;
            hooked = false;
        }
    }

    protected override void DoAction()
    {
        DoHook();
    }

    protected override void EndAction()
    {
        UnHook();
    }


}
