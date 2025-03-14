using System;
using System.Collections.Generic;

namespace MyBase.ApplicationEventManager
{
    // A simple Event System that can be used for remote systems communication
    public static class SimpleEventManager
    {
        static readonly Dictionary<Type, Action<IApplicationEvent>> s_Events = new Dictionary<Type, Action<IApplicationEvent>>();

        static readonly Dictionary<Delegate, Action<IApplicationEvent>> s_EventLookups =
            new Dictionary<Delegate, Action<IApplicationEvent>>();

        public static void AddListener<T>(Action<T> evt) where T : IApplicationEvent
        {
            if (!s_EventLookups.ContainsKey(evt))
            {
                Action<IApplicationEvent> newAction = (e) => evt((T) e);
                s_EventLookups[evt] = newAction;

                if (s_Events.TryGetValue(typeof(T), out Action<IApplicationEvent> internalAction))
                    s_Events[typeof(T)] = internalAction += newAction;
                else
                    s_Events[typeof(T)] = newAction;
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : IApplicationEvent
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

        public static void Broadcast(IApplicationEvent evt)
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