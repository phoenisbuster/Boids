using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyBase.Utils.Web
{

    public class BaseCdnLoader : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Configuration for CDN scripts and modules to load.")]
        private CdnLoaderData cdns;

        [SerializeField]
        [Tooltip("Called when each script or module is loaded.")]
        public UnityEvent<string> onLoaded = new UnityEvent<string>();

        [SerializeField]
        [Tooltip("Called when all loading is complete.")]
        public UnityEvent onFinished = new UnityEvent();

        /// <summary>
        /// Event invoked when an error occurs during loading.
        /// Passes the error message as a parameter.
        /// </summary>
        [SerializeField]
        [Tooltip("Called when a loading error occurs.")]
        public UnityEvent<string> onError = new UnityEvent<string>();

        /// <summary>
        /// If true, starts loading scripts automatically on load.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether to load scripts automatically when the component loads.")]
        private bool runOnLoad = false;

        private Action _onComplete;
        private int _cdnsLoadedCount = 0;
        private int _cdnsTotalCount = 0;

        private void Awake()
        {
            if (runOnLoad)
            {
                LoadScript();
            }
        }

        public void LoadScript(Action callback = null)
        {
            if (cdns == null)
            {
                Debug.LogWarning("CdnLoaderData is not assigned. Please assign a valid asset.");
                return;
            }
            
            #if !UNITY_WEBGL || UNITY_EDITOR
                Debug.LogWarning("Load Script Error: This feature is only supported in WebGL builds.");
                return;
            #else
                _onComplete = callback;
                _cdnsTotalCount = 0;
                _cdnsLoadedCount = 0;
                if (cdns.allowLoadScript)
                {
                    _cdnsTotalCount += cdns.ScriptList.Count;
                    foreach (var url in cdns.ScriptList)
                    {
                        LoadCdnScript(url, () => OnLoadingFinish(url));
                    }
                }

                if (cdns.allowLoadModule)
                {
                    _cdnsTotalCount += cdns.ModuleList.Count;
                    foreach (var url in cdns.ModuleList)
                    {
                        LoadCdnModule(url, () => OnLoadingFinish(url));
                    }
                }
            #endif
        }

        private void OnLoadingFinish(string url)
        {
            onLoaded?.Invoke(url);
            _cdnsLoadedCount++;
            if (_cdnsLoadedCount == _cdnsTotalCount)
            {
                OnComplete();
            }
        }

        private void OnComplete()
        {
            onFinished?.Invoke();
            _onComplete?.Invoke();
            _onComplete = null;
            _cdnsTotalCount = 0;
            _cdnsLoadedCount = 0;
        }

        private void OnLoadedError(string error)
        {
            onError?.Invoke(error);
        }

        private void LoadCdnScript(string url, System.Action callback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                WebGLCdnHelper.LoadScript(url, false, callback, OnLoadedError); // keepDOM not supported yet
            #else
                Debug.LogWarning($"Cannot load CDN script '{url}' outside WebGL runtime.");
                callback?.Invoke(); // Simulate completion in Editor
            #endif
        }

        private void LoadCdnModule(string url, System.Action callback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                WebGLCdnHelper.LoadModule(url, false, callback, OnLoadedError); // keepDOM not supported yet
            #else
                Debug.LogWarning($"Cannot load CDN module '{url}' outside WebGL runtime.");
                callback?.Invoke(); // Simulate completion in Editor
            #endif
        }
    }

}
