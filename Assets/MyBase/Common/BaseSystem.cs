using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.Common
{
    public class BaseSystem : MonoBehaviour
    {
        public virtual void Initialize() { }

        public virtual void OnLoaded() { }

        public virtual void OnUnloaded() { }

        public virtual void OnPreUnloaded() { }
        
        public virtual void OnPause() {}
        
        public virtual void OnResume() {}
    }
}
