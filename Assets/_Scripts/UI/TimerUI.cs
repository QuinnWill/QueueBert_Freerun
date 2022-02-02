using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Timer timer;

    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int dec = (int)(timer.time % 1 * 100);

        int seconds = (int)(timer.time % 60);

        int minutes = (int)(timer.time / 60);


        textMesh.text = "" + string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", seconds) + "." + string.Format("{0:00}", dec);
    }
}
