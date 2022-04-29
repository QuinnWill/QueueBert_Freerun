using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{


    public bool isPrimary;

    public bool limitUses = false;

    public int maxUses;

    public int uses;
    public Sprite UISprite;

    protected virtual void OnEnable()
    {
        if (isPrimary){
            InputEventManager.primaryStart += DoAction;
            InputEventManager.primaryEnd += EndAction;
        }
        else {
            InputEventManager.secondaryStart += DoAction;
            InputEventManager.SecondaryEnd += EndAction;
        }
    }

    protected virtual void OnDisable()
    {
        InputEventManager.primaryStart -= DoAction;
        InputEventManager.primaryEnd -= EndAction;
        InputEventManager.secondaryStart -= DoAction;
        InputEventManager.SecondaryEnd -= EndAction;
    }

    protected virtual void DoAction()
    {
        
    }

    protected virtual void EndAction()
    { 
        
    }

    public virtual void ResetUses()
    {
        uses = maxUses;
    }


}
