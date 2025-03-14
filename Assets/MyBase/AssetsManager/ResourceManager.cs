using System;
using System.Collections;
using System.Collections.Generic;
using MyBase.Singleton;
using UnityEngine;

namespace MyBase.AssetsManager
{
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        #region Load Asset
        /// <summary>
        /// Loads a asset from the specified path in resource folder and callback with the loaded asset.
        /// </summary>
        public void LoadAsset<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAssetCoroutine(path, onComplete));
        }
        #endregion

        #region Load Assets
        /// <summary>
        /// Loads all assets from the specified path in resource folder and callback with the loaded assets.
        /// Note: not recommended for large number of assets
        /// </summary>
        public void LoadAssets<T>(string path, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            T[] loadedAssets = Resources.LoadAll<T>(path);
            onComplete?.Invoke(loadedAssets);
        }
        #endregion

        private IEnumerator LoadAssetCoroutine<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            yield return request; // Wait until the loading is complete

            if (request.asset != null)
            {
                T loadedAsset = request.asset as T;
                callback?.Invoke(loadedAsset); // Invoke the callback with the loaded asset
            }
            else
            {
                Debug.LogError($"Failed to load asset at path: {path}");
            }
        }
    }
}


