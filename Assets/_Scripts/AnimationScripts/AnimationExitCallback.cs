using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationExitCallback : StateMachineBehaviour
{

    [SerializeField]
    private UnityEvent<Animator, AnimatorStateInfo, int> stateExitCallback;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stateExitCallback?.Invoke(animator, stateInfo, layerIndex);
    }
}
