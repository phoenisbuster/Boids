using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MyBase.Utils.Web;
using UnityEngine;

namespace MyBase.ApplicationEvent
{
    public class ApplicationEvent
    {
        public event Action Action = null;
        public event Action<object[]> ActionWithArgs = null;
        
        public ApplicationEvent() {}
        public ApplicationEvent(Action action) => Action = action;
        public ApplicationEvent(Action<object[]> action) => ActionWithArgs = action;

        public void AddListener(Action action)
        {
            Action += action;
        }
        public void AddListener(Action<object[]> action)
        {
            try
            {
                if(ActionWithArgs != null && !AreActionsCompatible(ActionWithArgs, action))
                {
                    throw new Exception("Actions are not compatible");
                }
                ActionWithArgs += action;
            }
            catch(Exception e)
            {
                Debug.LogWarning("Error adding listener " + e.Message);
            }
        }

        public void RemoveListener(Action action) => Action -= action;
        public void RemoveListener(Action<object[]> action) => ActionWithArgs -= action;

        public void Invoke() => Action?.Invoke();
        public void Invoke(params object[] args)
        {
            Action?.Invoke();
            ActionWithArgs?.Invoke(args);
        }

        public bool IsValid() => Action != null || ActionWithArgs != null;
        public bool HaveAction() => Action != null;
        public bool HaveActionWithArgs() => ActionWithArgs != null;

        private bool AreActionsCompatible(Action<object[]> existing, Action<object[]> newAction)
        {
            MethodInfo existingMethod = existing.Method;
            MethodInfo newMethod = newAction.Method;

            ParameterInfo[] existingParams = existingMethod.GetParameters();
            ParameterInfo[] newParams = newMethod.GetParameters();

            // ✅ Check if both actions take a single parameter of type `object[]`
            string s = $"existingParams.Length: {existingParams.Length}, newParams.Length: {newParams.Length}, existingMethod.Name: {existingMethod.Name}, newMethod.Name: {newMethod.Name}\n";
            if (existingParams.Length == newParams.Length)
            {
                for (int i = 0; i < existingParams.Length; i++)
                {
                    s += $"existingParams[{i}].Name: {existingParams[i].Name}, newParams[{i}].Name: {newParams[i].Name}\n";
                    if (existingParams[i].ParameterType != newParams[i].ParameterType)
                    {
                        return false;
                    }
                }
                s += "\n";
                Debug.LogWarning(s);
                return true;
            }
            return false;
        } 
    }
    
    public static class ApplicationEventManager
    {
        // Event mapping: event name -> (game instanceID -> delegate)
        public static readonly Dictionary<string, Dictionary<int, ApplicationEvent>> eventMapping = new();
        private static readonly object lockObject = new();

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

            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    if(callbacks.ContainsKey(instanceId))
                    {
                        callbacks[instanceId].AddListener(callback); // Add to existing Action
                    }
                    else
                    {
                        callbacks[instanceId] = new ApplicationEvent(callback); // Start new Action
                    }           
                }
                else
                {
                    var callbacks = new Dictionary<int, ApplicationEvent>
                    {
                        [instanceId] = new ApplicationEvent(callback)
                    };
                    eventMapping[evt] = callbacks;
                }
            }
        }

        /// <summary>
        /// Registers a callback for a specified event with arguments, associated with a MonoBehaviour instance.
        /// </summary>
        public static void On<T>(string evt, Action<object[]> callback, T obj) where T : MonoBehaviour
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

            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    if(callbacks.ContainsKey(instanceId))
                    {
                        callbacks[instanceId].AddListener(callback); // Add to existing Action
                    }
                    else
                    {
                        callbacks[instanceId] = new ApplicationEvent(callback); // Start new Action
                    }           
                }
                else
                {
                    var callbacks = new Dictionary<int, ApplicationEvent>
                    {
                        [instanceId] = new ApplicationEvent(callback)
                    };
                    eventMapping[evt] = callbacks;
                }
            }
        }

        /// <summary>
        /// Registers a callback for a specified event with a GameObject.
        /// Multiple callbacks can be registered for the same object.
        /// </summary>
        /// <typeparam name="T">The type of MonoBehaviour registering the callback.</typeparam>
        /// <param name="evt">The event name to listen for.</param>
        /// <param name="callback">The callback function to invoke when the event fires, accepting a variable number of arguments.</param>
        /// <param name="obj">The MonoBehaviour instance registering the callback.</param>
        // private static void RegisterEvent<T>(string evt, Delegate callback, T obj) where T : MonoBehaviour
        // {
        //     if (obj == null)
        //     {
        //         Debug.LogError("Cannot register event listener with a null MonoBehaviour.");
        //         return;
        //     }
        //     if (callback == null)
        //     {
        //         Debug.LogError("Cannot register a null callback for an event.");
        //         return;
        //     }

        //     int instanceId = obj.GetInstanceID();
        //     if (eventMapping.ContainsKey(evt))
        //     {
        //         var callbacks = eventMapping[evt];
        //         if(callbacks.ContainsKey(instanceId))
        //         {
        //             callbacks[instanceId] = Delegate.Combine(callbacks[instanceId], callback); // Add to existing Action
        //         }
        //         else
        //         {
        //             callbacks[instanceId] = callback; // Start new Action
        //         }           
        //     }
        //     else
        //     {
        //         var callbacks = new Dictionary<int, Delegate>
        //         {
        //             [instanceId] = callback
        //         };
        //         eventMapping[evt] = callbacks;
        //     }
        // }

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

            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    if (callbacks.ContainsKey(instanceId))
                    {    
                        callbacks[instanceId].RemoveListener(callback);
                        if (callbacks[instanceId].IsValid() == false)
                        {
                            callbacks.Remove(instanceId);
                        }

                        if (callbacks.Count == 0)
                        {
                            eventMapping.Remove(evt);
                        }
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
        public static void Off<T>(string evt, Action<object[]> callback, T obj) where T : MonoBehaviour
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

            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    if (callbacks.ContainsKey(instanceId))
                    {    
                        callbacks[instanceId].RemoveListener(callback);
                        if (callbacks[instanceId].IsValid() == false)
                        {
                            callbacks.Remove(instanceId);
                        }

                        if (callbacks.Count == 0)
                        {
                            eventMapping.Remove(evt);
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
            
            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    if (callbacks.ContainsKey(instanceId))
                    {    
                        callbacks.Remove(instanceId);
                        if (callbacks.Count == 0)
                        {
                            eventMapping.Remove(evt);
                        }
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

            lock (lockObject)
            {
                int instanceId = obj.GetInstanceID();
                var newList = new List<string>(eventMapping.Keys);
                foreach (var evt in newList)
                {
                    var callbacks = eventMapping[evt];
                    if (callbacks.ContainsKey(instanceId))
                    {
                        callbacks.Remove(instanceId);
                        if (callbacks.Count == 0)
                        {
                            eventMapping.Remove(evt);
                        }
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
            lock (lockObject)
            {
                if (eventMapping.ContainsKey(evt))
                {
                    eventMapping.Remove(evt);
                }
            }
        }

        /// <summary>
        /// Unregisters events.
        /// </summary>
        /// <typeparam name="T">The type of MonoBehaviour unregistering the callback.</typeparam>
        /// <param name="evt">The event name to stop listening for.</param>
        /// <param name="callback">The specific callback to remove; if null, removes all callbacks for the instance.</param>
        /// <param name="obj">The MonoBehaviour instance unregistering the callback.</param>
        // private static void UnregisterEvent<T>(string evt, Delegate callback, T obj) where T : MonoBehaviour
        // {
        //     if (obj == null)
        //     {
        //         Debug.LogWarning("Cannot unregister event listener with a null MonoBehaviour.");
        //         return;
        //     }

        //     int instanceId = obj.GetInstanceID();
        //     if (eventMapping.ContainsKey(evt))
        //     {
        //         var callbacks = eventMapping[evt];
        //         if (callbacks.ContainsKey(instanceId))
        //         {    
        //             callbacks[instanceId] = Delegate.Remove(callbacks[instanceId], callback);
        //             if (callbacks[instanceId] == null)
        //             {
        //                 callbacks.Remove(instanceId);
        //             }
        //             if (callbacks.Count == 0)
        //             {
        //                 eventMapping.Remove(evt);
        //             }
        //         }
        //     }
        // }

        #endregion

        #region Firing Events

        /// <summary>
        /// Fires an event, invoking all registered callbacks with a variable number of arguments.
        /// </summary>
        /// <param name="evt">The event name to fire.</param>
        /// <param name="args">Variable number of arguments to pass to the callbacks.</param>
        public static void Fire(string evt, params object[] args)
        {
            lock (lockObject)
            {
                if (eventMapping.ContainsKey(evt))
                {
                    var callbacks = eventMapping[evt];
                    foreach (var callback in callbacks.Values)
                    {
                        try
                        {
                            // Debug.LogError("Fire Event: " + args.Length);
                            if(args == null || args.Length == 0)
                            {
                                callback?.Invoke();
                            }
                            else
                            {
                                callback?.Invoke(args);
                            }         
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("❌ Event Error: " + e.Message);
                        }
                    }
                }
            }
        }

        #endregion
    }
}

