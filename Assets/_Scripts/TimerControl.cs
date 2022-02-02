using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerControl : MonoBehaviour
{
    public enum TimerEffectMode
    { 
        Start,
        Stop,
    }

    public Timer timer;

    public TimerEffectMode mode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;
        if (mode == TimerEffectMode.Stop)
        {
            timer.StopTimer();
        }
        else if (mode == TimerEffectMode.Start)
        {
            timer.ResetTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;
        if (mode == TimerEffectMode.Start)
        {
            timer.StartTimer();
        }
    }
}
