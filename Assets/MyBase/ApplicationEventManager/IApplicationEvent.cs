using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MyBase.ApplicationEventManager
{
    public class IApplicationEvent
    {

    }

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
}
