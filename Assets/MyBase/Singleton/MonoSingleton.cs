using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        [SerializeField]
        [Tooltip("Indicate that this singleton will be KEEP on loading new Scene. Default is FALSE")]
        private bool persistAcrossScenes = false;
        
        private static T _instance = null;
        
        public static T Instance
        {
            get
            {
                // Instance requiered for the first time, we look for it
                if( _instance == null )
                {
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    // Object not found, we create a temporary one
                    if( _instance == null )
                    {
                        Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");

                        isTemporaryInstance = true;
                        _instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();

                        // Problem during the creation, this should not happen
                        if( _instance == null )
                        {
                            Debug.LogError("Problem during the creation of " + typeof(T).ToString());
                        }

                        // A temporary singleton need to be destroyed on load
                        _instance.persistAcrossScenes = true;
                    }

                    if (!_isInitialized)
                    {
                        _instance.Initialize();
                    }
                }
                return _instance;
            }
        }

        public static bool isTemporaryInstance { private set; get; }

        private static bool _isInitialized;

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                if(persistAcrossScenes)
                {
                    Debug.LogError($"Duplicate {GetType().Name} found on '{gameObject.name}'. Only one instance is allowed. Destroying duplicate.");
                    DestroyImmediate(this);
                    return;
                }
                else
                {
                    DestroyImmediate(_instance.gameObject);
                }
            }
            
            _instance = this as T;
            if(!_isInitialized) 
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if(persistAcrossScenes) DontDestroyOnLoad(gameObject);
            
            _isInitialized = true;
            _instance.OnLoad();
        }

        protected void OnDestroy()
        {
            _instance = null;
        }


        /// <summary>
        /// This function is called when the instance is used the first time
        /// Put all the initializations you need here, as you would do in Awake
        /// </summary>
        public virtual void OnLoad(){}
    
        /// Make sure the instance isn't referenced anymore when the user quit, just in case.
        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}
