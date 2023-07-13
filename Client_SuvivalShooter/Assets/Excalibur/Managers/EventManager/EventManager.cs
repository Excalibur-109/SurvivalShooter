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
            Debug.LogError (string.Format ("{0}已经监听", eventName));
            return;
        }
        _eventDic.Add (eventName, eventHandler);
    }

    public void StopListen (string eventName)
    {
        if (!_eventDic.ContainsKey (eventName))
        {
            Debug.LogError (string.Format ("事件{0}不存在", eventName));
            return;
        }
        _eventDic.Remove (eventName);
    }

    public void Broadcast (string eventName, params object[] parameters)
    {
        if (!_eventDic.ContainsKey (eventName))
        {
            throw new Exception (string.Format ("触发的事件{0}不存在", eventName));
        }

        _eventDic[eventName].Invoke (parameters);
    }
}
