using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{


    public enum PlayerState { 
        Moving,
        Loading,
        Cutscene
    }

    public PlayerState currentState;

    public Dictionary<PlayerState, List<Component>> StateContainer;


}
