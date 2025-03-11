using System;
using System.Collections.Generic;

namespace MyBase.ApplicationEventManager
{
    public class ObserverEvent {}

    // A simple Event System that can be used for remote systems communication
    public static class GlobalObserver
    {
        static readonly Dictionary<Type, Action<ObserverEvent>> s_Events = new Dictionary<Type, Action<ObserverEvent>>();

        static readonly Dictionary<Delegate, Action<ObserverEvent>> s_EventLookups =
            new Dictionary<Delegate, Action<ObserverEvent>>();

        public static void AddListener<T>(Action<T> evt) where T : ObserverEvent
        {
            if (!s_EventLookups.ContainsKey(evt))
            {
                Action<ObserverEvent> newAction = (e) => evt((T) e);
                s_EventLookups[evt] = newAction;

                if (s_Events.TryGetValue(typeof(T), out Action<ObserverEvent> internalAction))
                    s_Events[typeof(T)] = internalAction += newAction;
                else
                    s_Events[typeof(T)] = newAction;
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : ObserverEvent
        {
            if (s_EventLookups.TryGetValue(evt, out var action))
            {
                if (s_Events.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                        s_Events.Remove(typeof(T));
                    else
                        s_Events[typeof(T)] = tempAction;
                }

                s_EventLookups.Remove(evt);
            }
        }

        public static void Broadcast(ObserverEvent evt)
        {
            if (s_Events.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }

        public static void Clear()
        {
            s_Events.Clear();
            s_EventLookups.Clear();
        }
    }
}