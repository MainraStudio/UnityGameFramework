using System;
using System.Collections.Generic;
using MainraFramework;

public class EventManager
{
    private Dictionary<string, List<Delegate>> eventDictionary = new Dictionary<string, List<Delegate>>();

    public void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = new List<Delegate>();
        }
        eventDictionary[eventName].Add(listener);
    }

    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName].Remove(listener);
            if (eventDictionary[eventName].Count == 0)
            {
                eventDictionary.Remove(eventName);
            }
        }
    }

    public void Publish<T>(string eventName, T data)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            foreach (var listener in eventDictionary[eventName].ToArray())
            {
                var action = listener as Action<T>;
                action?.Invoke(data);
            }
        }
    }

    public void ClearAllEvents()
    {
        eventDictionary.Clear();
    }
}