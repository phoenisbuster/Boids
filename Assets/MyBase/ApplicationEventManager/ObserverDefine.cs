using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.ApplicationEventManager
{
    public interface IObserverEventData
    {

    }

    public class ObserverEvent<T> where T : IObserverEventData
    {
        public Action Signal { get; set; }
        public Action<T> Event { get; set; }

        public ObserverEvent() {}
        public ObserverEvent(Action action) => Signal = action;
        public ObserverEvent(Action<T> action) => Event = action;

        public void AddListener(Action action)
        {
            Signal += action;
        }
        public void AddListener(Action<T> action)
        {
            Event += action;
        }

        public void RemoveListener(Action action) => Signal -= action;
        public void RemoveListener(Action<T> action) => Event -= action;

        public void SendSignal() => Signal?.Invoke();
        public void Notify(T data)
        {
            Signal?.Invoke();
            Event?.Invoke(data);
        }

        public bool IsValid() => Signal != null || Event != null;
        public bool HaveAction() => Signal != null;
        public bool HaveActionWithArgs() => Event != null;
    }

    public class ObserverListener: MonoBehaviour
    {
        
    }
}
