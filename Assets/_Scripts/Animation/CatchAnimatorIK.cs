using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class CatchAnimatorIK : MonoBehaviour
{

    public Animator animator;

    [SerializeField]
    private UnityEvent<int> animatorIKEvent;

    [SerializeField]
    private UnityEvent<Animator> animatorMoveEvent;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animatorIKEvent?.Invoke(layerIndex);
    }

    private void OnAnimatorMove()
    {
        animatorMoveEvent?.Invoke(animator);
    }
}
