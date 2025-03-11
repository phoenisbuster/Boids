using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.ApplicationEventManager
{   
    public static class ApplicationEventManager
    {
        // Event mapping: event name -> (game instanceID -> delegate)
        static readonly Dictionary<string, Dictionary<int, Action>> signals = new();
        static readonly Dictionary<string, Dictionary<int, Action<IApplicationEvent>>> events = new();
        static readonly Dictionary<Delegate, Action<IApplicationEvent>> eventsBackup = new();

        #region Registration Methods

        /// <summary>
        /// Registers a callback for a specified event with no arguments, associated with a MonoBehaviour instance.
        /// </summary>
        public static void On<T>(string evt, Action callback, T obj) where T : MonoBehaviour
        {
            if (obj == null)
            {
                Debug.LogError("Cannot register event listener with a null MonoBehaviour.");
                return;
            }
            if (callback == null)
            {
                Debug.LogError("Cannot register a null callback for an event.");
                return;
            }

            int instanceId = obj.GetInstanceID();
            if (signals.ContainsKey(evt))
            {
                var allActions = signals[evt];
                if(allActions.ContainsKey(instanceId))
                {
                    allActions[instanceId] += callback; // Add to existing Action
                }
                else
                {
                    allActions[instanceId] = callback; // Start new Action
                }           
            }
            else
            {
                var callbacks = new Dictionary<int, Action>
                {
                    [instanceId] = callback
                };
                signals[evt] = callbacks;
            }
        }

        /// <summary>
        /// Registers a callback for a specified event with arguments, associated with a MonoBehaviour instance.
        /// </summary>
        public static void On<T, E>(string evt, Action<T> callback, E obj) where T : IApplicationEvent where E: MonoBehaviour
        {
            if (obj == null)
            {
                Debug.LogError("Cannot register event listener with a null MonoBehaviour.");
                return;
            }
            if (callback == null)
            {
                Debug.LogError("Cannot register a null callback for an event.");
                return;
            }

            int instanceId = obj.GetInstanceID();
            Action<IApplicationEvent> newAction = (e) => callback((T) e);
            eventsBackup[callback] = newAction;
            if (events.ContainsKey(evt))
            {
                var allActions = events[evt];
                if(allActions.TryGetValue(instanceId, out Action<IApplicationEvent> internalAction))
                {
                    // Add to existing Action
                    allActions[instanceId] = internalAction += newAction; 
                }
                else
                {
                    // Start new Action
                    allActions[instanceId] = newAction; 
                }           
            }
            else
            {
                var callbacks = new Dictionary<int, Action<IApplicationEvent>>
                {
                    [instanceId] = newAction
                };
                events[evt] = callbacks;
            }
        }

        #endregion
        
        #region Unregistration Methods

        /// <summary>
        /// Unregisters a callback for a specified event, associated with a MonoBehaviour instance.
        /// If callback is null, removes all callbacks for the instance; otherwise, removes only the matching callback.
        /// </summary>
        /// <typeparam name="T">The type of MonoBehaviour unregistering the callback.</typeparam>
        /// <param name="evt">The event name to stop listening for.</param>
        /// <param name="callback">The specific callback to remove; if null, removes all callbacks for the instance.</param>
        /// <param name="obj">The MonoBehaviour instance unregistering the callback.</param>
        public static void Off<T>(string evt, Action callback, T obj) where T : MonoBehaviour
        {
            if(callback == null)
            {
                UnregisterEventFromObject(evt, obj);
                return;
            }
            
            if (obj == null)
            {
                Debug.LogWarning("Cannot unregister event listener with a null MonoBehaviour.");
                return;
            }

            int instanceId = obj.GetInstanceID();
            if (signals.ContainsKey(evt))
            {
                var allAcions = signals[evt];
                if (allAcions.ContainsKey(instanceId))
                {    
                    allAcions[instanceId] -= callback;
                    if (allAcions[instanceId] == null)
                    {
                        allAcions.Remove(instanceId);
                    }

                    if (allAcions.Count == 0)
                    {
                        signals.Remove(evt);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters a callback for a specified event, associated with a MonoBehaviour instance.
        /// If callback is null, removes all callbacks for the instance; otherwise, removes only the matching callback.
        /// </summary>
        /// <typeparam name="T">The type of MonoBehaviour unregistering the callback.</typeparam>
        /// <param name="evt">The event name to stop listening for.</param>
        /// <param name="callback">The specific callback to remove; if null, removes all callbacks for the instance.</param>
        /// <param name="obj">The MonoBehaviour instance unregistering the callback.</param>
        public static void Off<T, E>(string evt, Action<T> callback, E obj) where T: IApplicationEvent where E : MonoBehaviour
        {
            if(callback == null)
            {
                UnregisterEventFromObject(evt, obj);
                return;
            }
            
            if (obj == null)
            {
                Debug.LogWarning("Cannot unregister event listener with a null MonoBehaviour.");
                return;
            }
           
            int instanceId = obj.GetInstanceID();
            if (events.ContainsKey(evt))
            {
                var allActions = events[evt];
                if(eventsBackup.TryGetValue(callback, out var action))
                {
                    if(allActions.TryGetValue(instanceId, out Action<IApplicationEvent> tempAction))
                    {    
                        tempAction -= action;
                        if (tempAction == null)
                            allActions.Remove(instanceId);
                        else
                            allActions[instanceId] = tempAction;

                        if(allActions.Count == 0)
                        {
                            events.Remove(evt);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters all callbacks for a specified event, associated with a MonoBehaviour instance.
        /// </summary>
        /// <typeparam name="T">The type of MonoBehaviour unregistering the callback.</typeparam>
        /// <param name="evt">The event name to stop listening for.</param>
        /// <param name="obj">The MonoBehaviour instance unregistering the callback.</param>
        public static void UnregisterEventFromObject<T>(string evt, T obj) where T : MonoBehaviour
        {
            if (obj == null)
            {
                Debug.LogWarning("Cannot unregister event listener with a null MonoBehaviour.");
                return;
            }
            
            int instanceId = obj.GetInstanceID();
            if (events.ContainsKey(evt))
            {
                var allActions = events[evt];
                if (allActions.ContainsKey(instanceId))
                {    
                    allActions.Remove(instanceId);
                    if (allActions.Count == 0)
                    {
                        events.Remove(evt);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters all callbacks for all events, associated with a MonoBehaviour instance.
        /// </summary>
        /// <param name="evt">The event name to stop listening for.</param>
        public static void UnregisterAllEventsForObject<T>(T obj) where T : MonoBehaviour
        {
            if (obj == null)
            {
                Debug.LogWarning("Cannot unregister event listener with a null MonoBehaviour.");
                return;
            }

            int instanceId = obj.GetInstanceID();
            var newList = new List<string>(events.Keys);
            foreach (var evt in newList)
            {
                var callbacks = events[evt];
                if (callbacks.ContainsKey(instanceId))
                {
                    callbacks.Remove(instanceId);
                    if (callbacks.Count == 0)
                    {
                        events.Remove(evt);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters all callbacks for a specified event.
        /// </summary>
        /// <param name="evt">The event name to stop listening for.</param>
        public static void UnregisterAllEvents(string evt)
        {
            if(events.ContainsKey(evt))
            {
                events.Remove(evt);
            }
        }

        #endregion

        #region Firing Events

        /// <summary>
        /// Fires an signal, invoking all registered callbacks with no arguments.
        /// </summary>
        /// <param name="evt">The event name to fire.</param>
        public static void BroadcastSignal(string evt)
        {
            if(signals.ContainsKey(evt))
            {
                var allActions = signals[evt];
                foreach (var action in allActions.Values)
                {
                    try
                    {
                        action?.Invoke();       
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("❌ Signal Error: " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Fires an event, invoking all registered callbacks with a variable number of arguments.
        /// </summary>
        /// <param name="evt">The event name to fire.</param>
        /// <param name="args">Variable number of arguments to pass to the callbacks.</param>
        public static void BroadcastEvent(string evt, IApplicationEvent args)
        {
            if(events.ContainsKey(evt))
            {
                var allActions = events[evt];
                foreach (var action in allActions.Values)
                {
                    try
                    {
                        action?.Invoke(args);       
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("❌ Event Error: " + e.Message);
                    }
                }
            }

            if(signals.ContainsKey(evt))
            {
                var allActions = signals[evt];
                foreach (var action in allActions.Values)
                {
                    try
                    {
                        action?.Invoke();       
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("❌ Signal Error: " + e.Message);
                    }
                }
            }
        }

        #endregion
    }
}

