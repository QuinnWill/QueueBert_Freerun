using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float time { get; private set; }
    public bool doTimer;

    // Update is called once per frame
    void Update()
    {
        if (doTimer)
        {
            if (time < 3600)
            {
                time += Time.deltaTime;
            }
            else
            {
                StopTimer();
            }
        }
    }

    public void ResetTimer()
    {
        time = 0;
        doTimer = false;
    }

    public void StartTimer()
    {
        doTimer = true;
    }

    public void StopTimer()
    {
        doTimer = false;
    }

}
