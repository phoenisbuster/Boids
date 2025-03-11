using System;
using UnityEngine;

namespace MyBase.Observer
{
    public class EventListener : MonoBehaviour, IObserver
    {
        public Action<EventListener> RequestDestroy;

        protected void Attach(string mask = "0", string funcName = null)
        {
            //Debug.Log("type: " + mask);
            EventManager.Attach(this, mask, funcName);
        }

        protected void Detach(string mask = "0")
        {
            EventManager.Detach(this, mask);
        }

        protected void Notify(string type, params object[] data)
        {
            //Debug.Log("type: " + type);
            EventManager.Notify(type, data, this);
        }

        protected virtual void OnDestroy()
        {
            Detach();
        }

        public virtual void OnEvent(string type, object source, object data = null)
        {

        }
    }
}
