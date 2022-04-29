using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UsableItem))]
public class ResetItemCount : MonoBehaviour
{

    private RigidbodyCharacterMovement characterMovement;
    private UsableItem usableObject;

    public enum ResetMode
    { 
        Grounded,
        Time,
    }

    [SerializeField]
    private ResetMode mode;

    // Start is called before the first frame update
    void Start()
    {
        characterMovement = GetComponentInParent<RigidbodyCharacterMovement>();
        usableObject = GetComponent<UsableItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == ResetMode.Grounded && usableObject.uses < usableObject.maxUses)
        {
            if (characterMovement.grounded)
            { 
                usableObject.ResetUses();
            }
        }
    }
}
