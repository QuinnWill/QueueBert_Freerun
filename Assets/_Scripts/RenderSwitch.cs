using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSwitch : MonoBehaviour
{

    public enum RenderStates
    { 
        firstPerson,
        thirdPerson,
    }

    public RenderStates state = new RenderStates();

    public List<GameObject> firstPersonRenders;
    public List<GameObject> thirdPersonRenders;

    private void OnEnable()
    {
        InputEventManager.switchCamera += switchRenderState;
    }

    private void OnDisable()
    {
        InputEventManager.switchCamera -= switchRenderState;
    }

    public void switchRenderState() 
    {
        if (firstPersonRenders.Count > 0)
        {
            foreach (GameObject obj in firstPersonRenders)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }

        if (thirdPersonRenders.Count > 0)
        {
            foreach (GameObject obj in thirdPersonRenders)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }



}
