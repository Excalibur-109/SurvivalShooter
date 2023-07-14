using System;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;

public delegate void EventHandler (params object[] @params);

public sealed class EventManager : Singleton<EventManager>
{
    private Dictionary<string, EventHandler> _eventDic = new Dictionary<string, EventHandler> ();

    public void StartListen (string eventName, EventHandler eventHandler)
    {
        if (_eventDic.ContainsKey (eventName))
        {
            Debug.LogError (string.Format ("{0}Ѿ", eventName));
            return;
        }
        _eventDic.Add (eventName, eventHandler);
    }

    public void StopListen (string eventName)
    {
        if (!_eventDic.ContainsKey (eventName))
        {
            Debug.LogError (string.Format ("¼{0}", eventName));
            return;
        }
        _eventDic.Remove (eventName);
    }

    public void Broadcast (string eventName, params object[] parameters)
    {
        if (!_eventDic.ContainsKey (eventName))
        {
            throw new Exception (string.Format ("¼{0}", eventName));
        }

        _eventDic[eventName].Invoke (parameters);
    }
}
