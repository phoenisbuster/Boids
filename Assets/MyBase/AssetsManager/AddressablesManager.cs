using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyBase.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyBase.AssetsManager
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        private readonly Dictionary<string, UnityEngine.Object> cache = new();
        
        #region Load Single Asset
        public void LoadAsset<T>(string address, Action<T> onComplete) where T : UnityEngine.Object
        {
            if(string.IsNullOrEmpty(address))
            {
                return;
            }

            if(cache.TryGetValue(address, out UnityEngine.Object cachedObject))
            {
                onComplete?.Invoke(cachedObject as T);
                return;
            }

            StartCoroutine(LoadAssetCoroutine<T>(address, onComplete));
        }
        
        public async Task<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            if(string.IsNullOrEmpty(address))
            {
                return null;
            }

            if (cache.TryGetValue(address, out UnityEngine.Object cachedObject))
            {
                return cachedObject as T;
            }


            AsyncOperationHandle<T> async = Addressables.LoadAssetAsync<T>(address);
            T gameObjAsset = await async.Task;
            if (gameObjAsset != null)
            {
                cache[address] = gameObjAsset;
                return gameObjAsset;
            }

            Debug.LogError($"Failed to load asset async at path: {address}");
            return null;
        }

        private IEnumerator LoadAssetCoroutine<T>(string address, Action<T> callback) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> async = Addressables.LoadAssetAsync<T>(address);
            yield return async; // Wait until the loading is complete

            if (async.Status == AsyncOperationStatus.Succeeded && async.Result != null)
            {
                T loadedAsset = async.Result;
                cache[address] = loadedAsset;
                callback?.Invoke(loadedAsset); // Invoke the callback with the loaded asset
            }
            else
            {
                Debug.LogError($"Failed to load asset at path: {address}");
            }
        }
        #endregion

        #region Load Multiple Assets
        public void LoadAssets<T>(List<string> addresses, Action<List<T>> onComplete) where T : UnityEngine.Object
        {
            List<T> results = new();
            List<string> addressList = new();
            foreach(var address in addresses)
            {
                if(cache.TryGetValue(address, out UnityEngine.Object cachedObject))
                {
                    results.Add(cachedObject as T);
                }
                else
                {
                    addressList.Add(address);
                }
            }

            if(results.Count == addresses.Count && addressList.Count <= 0)
            {
                onComplete?.Invoke(results);
                return;
            }

            StartCoroutine(LoadAssetsCoroutine<T>(
                addressList, 
                (list) => 
                {
                    results.AddRange(list);
                    onComplete?.Invoke(results);
                }
            ));
        }

        private IEnumerator LoadAssetsCoroutine<T>(List<string> addresses, Action<List<T>> onComplete) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<T>> async = Addressables.LoadAssetsAsync<T>(addresses, null, Addressables.MergeMode.Union);
            yield return async; // Wait until the loading is complete

            if (async.Status == AsyncOperationStatus.Succeeded && async.Result != null)
            {
                IList<T> loadedAsset = async.Result;
                onComplete?.Invoke(new List<T>(loadedAsset)); // Convert IList<T> to List<T> and invoke the callback

                foreach(string address in addresses)
                {
                    cache[address] = loadedAsset[0];
                }
            }
            else
            {
                Debug.LogError($"Failed to load asset at path: {addresses}");
            }
        }
        #endregion
    }
}


