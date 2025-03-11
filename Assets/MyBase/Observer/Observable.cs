using System.Collections.Generic;
using System.Reflection;
using MyBase.Common;

namespace MyBase.Observer
{
    struct EventData
    {
        public string eventType;
        public object source;
        public object data;
    }

    struct ListenerObject
    {
        public IObserver observer;
        public MethodInfo OnEvent;
        public object[] parameters;
    }

    class Observable : IObservable
    {
        //private Dictionary<string, List<ListenerObject>> _nodes = new Dictionary<string, List<ListenerObject>>();
        private MyDictionary<string, List<ListenerObject>> _nodes = new MyDictionary<string, List<ListenerObject>>();
        private bool isNotifying = false;
        private List<EventData> queueEvent = new List<EventData>();

        public void Attach(IObserver observer, string eventName = "0", string funcName = null)
        {
            #region define and parse data
            List<ListenerObject> listenerObjects;
            List<ListenerObject> allEvent;

            _nodes.TryGetValue(eventName, out listenerObjects);
            _nodes.TryGetValue("0", out allEvent);
            #endregion

            if (listenerObjects == null)
            {
                listenerObjects = new List<ListenerObject>();
                _nodes.Add(eventName, listenerObjects);
            }

            #region check exist event
            if (allEvent != null)
                for (int i = 0; i < allEvent.Count; i++)
                    if (allEvent[i].observer == observer)
                        return;

            for (int i = 0; i < listenerObjects.Count; i++)
                if (listenerObjects[i].observer == observer)
                    return;
            #endregion

            #region add new event


            ListenerObject listenerObject = new ListenerObject();

            var methodName = funcName == null ? "on" + eventName : funcName;
            MethodInfo method = observer.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (method != null)
            {
                int numParams = method.GetParameters().Length;
                listenerObject.parameters = new object[numParams];
            }
            listenerObject.observer = observer;
            listenerObject.OnEvent = method;
            listenerObjects.Add(listenerObject);
            #endregion
        }

        public void Dettach(IObserver observer, string eventName = "0")
        {
            #region define and parse data
            List<ListenerObject> listenerObjects;
            List<ListenerObject> allEvent;

            _nodes.TryGetValue(eventName, out listenerObjects);
            _nodes.TryGetValue("0", out allEvent);
            #endregion

            #region detach event
            if (eventName != "0")
            {
                if (listenerObjects != null)
                {
                    for (int i = 0; i < listenerObjects.Count; i++)
                        if (listenerObjects[i].observer == observer)
                        {
                            listenerObjects.Remove(listenerObjects[i]);
                            break;
                        }
                }
            }
            else
            {
                if (allEvent != null)
                {
                    // remove 'this' event
                    for (int i = 0; i < allEvent.Count; i++)
                    {
                        if (allEvent[i].observer == observer)
                        {
                            allEvent.Remove(allEvent[i]);
                            break;
                        }
                    }
                }

                // remove all events
                for (int i = 0; i < _nodes.Count; i++)
                {
                    var item = _nodes.ElementAt(i);
                    for (int index = item.Value.Count - 1; index >= 0; index--)
                    {
                        if (item.Value[index].observer == observer)
                        {
                            item.Value.Remove(item.Value[index]);
                        }
                    }
                }
            }
            #endregion
        }

        public void Notify(string type, object source = null, object data = null)
        {
            #region notify event
            if (isNotifying)
            {
                enqueue(type, source, data);
                return;
            }
            isNotifying = true;
            var total = _nodes.Count;
            for (int i = 0; i < total; i++)
            {
                KeyValuePair<string, List<ListenerObject>> node = _nodes.ElementAt(i);
                if (type == node.Key || node.Key == "0")
                {
                    List<ListenerObject> listenerObjects;
                    _nodes.TryGetValue(node.Key, out listenerObjects);
                    ListenerObject[] objects = listenerObjects.ToArray();
                    for (int j = 0; j < objects.Length; j++)
                    {
                        handleEvent(type, source, objects[j], data);
                    }
                }
            }
            isNotifying = false;
            if (queueEvent.Count > 0)
            {
                dequeue();
            }
            #endregion
        }

        private void handleEvent(string type, object source, ListenerObject listenerObject, object data)
        {
            if (listenerObject.OnEvent != null)
            {
                if (listenerObject.parameters.Length > 0) listenerObject.parameters[0] = data;
                if (listenerObject.parameters.Length > 1) listenerObject.parameters[1] = source;
                listenerObject.OnEvent.Invoke(listenerObject.observer, listenerObject.parameters);
            }
            else
            {
                listenerObject.observer.OnEvent(type, source, data);
            }
        }

        private void enqueue(string type, object source, object data)
        {
            EventData eventData = new EventData();
            eventData.eventType = type;
            eventData.source = source;
            eventData.data = data;
            queueEvent.Add(eventData);
        }

        private void dequeue()
        {
            EventData eventData = queueEvent[0];
            queueEvent.RemoveAt(0);
            Notify(eventData.eventType, eventData.source, eventData.data);
        }
    }
}
