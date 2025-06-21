using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    Observer trigger = new Observer();
    public void ClearEvents()
    {
        trigger.ClearEvents();
    }
    public void RegisterEvent(string eventName, Action callback)
    {
        trigger.RegisterEvent(eventName,callback);
    }
    public void UnregisterEvent(string eventName, Action callback)
    {
        trigger.UnregisterEvent(eventName, callback);
    }
    public void PatchEvent(string eventName)
    {
        trigger.PatchEvent(eventName);
    }
}

public class Observer
{
    Dictionary<string, Action> events = new Dictionary<string, Action>();
    public void ClearEvents()
    {
        events.Clear();
    }
    public void RegisterEvent(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] += callback;
        }
        else
        {
            events[eventName] = callback;
        }
    }
    public void UnregisterEvent(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] -= callback;
        }
        else
        {
        }
    }
    public void PatchEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName]?.Invoke();
        }
    }

}
