using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeThrow : MonoBehaviour
{

    public float maxRopeLength;

    public Rigidbody rb;

    public bool hooked;

    public LayerMask layerMask;

    private LineRenderer lineRenderer;

    private Vector3 hookPosition;
    private float hookLength;



    private void OnEnable()
    {
        InputEventManager.OnSecondaryStart += DoHook;
        InputEventManager.onSecondaryEnd += UnHook;
    }

    private void OnDisable()
    {
        InputEventManager.OnSecondaryStart -= DoHook;
        InputEventManager.onSecondaryEnd -= UnHook;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    void LateUpdate()
    {
        if (hooked)
        {
            if (Vector3.Distance(rb.position, hookPosition) >= hookLength)
            {
                rb.velocity += Vector3.Dot(rb.velocity, (rb.position - hookPosition).normalized) * (-rb.position + hookPosition).normalized;

                rb.velocity += (-rb.position + hookPosition).normalized * (Vector3.Distance(rb.position, hookPosition) - hookLength);


            }
            //rb.AddForce((-rb.position + hookPosition).normalized * 10);
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void DoHook()
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

    public void UnHook()
    {
        lineRenderer.enabled = false;
        hookPosition = Vector3.zero;
        hooked = false;
    }


}
