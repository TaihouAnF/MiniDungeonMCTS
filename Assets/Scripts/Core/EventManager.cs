using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static EventManager Instance { get; private set;}

    public static Action<Actor> OnPlayerMoved;
    public static event Action OnLevelRestart;

    public static void TriggerLevelUpdate(Actor actor)
    {
        OnPlayerMoved?.Invoke(actor);
    }

    public static void TriggerLevelRestart() 
    {
        OnLevelRestart?.Invoke();
    }
}
