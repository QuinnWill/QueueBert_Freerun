using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtOffset : MonoBehaviour
{

    public Vector3 startOffset;

    private bool wasCrouching;

    public bool crouching;

    private RigidbodyCharacterMovement character;
    // Start is called before the first frame update

    void Start()
    {
        character = GetComponentInParent<RigidbodyCharacterMovement>();
        transform.localPosition = startOffset;
    }

    void Update()
    {
        if (character.isCrouching != wasCrouching)
            crouching = !crouching;

        if (crouching)
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.2f);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, startOffset, 0.2f);

        wasCrouching = crouching;
    }


}
