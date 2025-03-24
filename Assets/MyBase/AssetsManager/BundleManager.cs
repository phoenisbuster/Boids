using System;
using System.Collections;
using System.Collections.Generic;
using MyBase.Common;
using MyBase.Singleton;
using UnityEngine;
using UnityEngine.Networking;

namespace MyBase.AssetsManager
{
    [Serializable]
    public class BundleConfig
    {
        public string BundleName;
        public string BundlePath;
        public bool IsRemote;

        public BundleConfig(string _name, string _path, bool _isRemote = false) 
        { 
            BundleName = _name;
            BundlePath = _path;
            IsRemote = _isRemote;
        }
    }
    
    public class BundleManager : MonoSingleton<BundleManager>
    {
        private readonly Dictionary<string, BundleConfig> configs = new();
        private readonly Dictionary<string, AssetBundle> loadedBundles = new();

        public override void OnLoad() 
        { 
            
        }

        #region Load Bundle
        public void LoadBundle(BundleConfig config, Action onComplete)
        {
            if (config.IsRemote)
            {
                StartCoroutine(LoadRemoteBundle(config, onComplete));
                return;
            }
            StartCoroutine(LoadLocalBundle(config, onComplete));
        }

        public BundleConfig GetBundleConfig(string bundleName) => configs[bundleName];

        private IEnumerator LoadLocalBundle(BundleConfig config, Action onComplete)
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(config.BundlePath);
            yield return bundleRequest;

            AssetBundle bundle = bundleRequest.assetBundle;
            if (bundle != null)
            {
                loadedBundles.Add(config.BundleName, bundle);
                configs.Add(config.BundleName, config);
                onComplete?.Invoke();
            }
        }

        private IEnumerator LoadRemoteBundle(BundleConfig config, Action onComplete)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(config.BundlePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                if (bundle != null)
                {
                    loadedBundles.Add(config.BundleName, bundle);
                    configs.Add(config.BundleName, config);
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to extract bundle from remote URL: {config.BundlePath}");
                }
            }
        }
        #endregion

        #region Remove Bundle
        public void RemoveBundle(string bundleName)
        {
            if (loadedBundles.ContainsKey(bundleName))
            {
                loadedBundles[bundleName].Unload(true);
                loadedBundles.Remove(bundleName);
                configs.Remove(bundleName);
            }
        }
        #endregion

        #region Load Asset From Bundle
        public void LoadAsset<T>(string bundleName, string assetName, Action<T> onComplete) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAssetCoroutine(bundleName, assetName, onComplete));
        }

        public void LoadAssets<T>(string bundleName, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAssetsCoroutine(bundleName, onComplete));
        }

        private IEnumerator LoadAssetCoroutine<T>(string bundleName, string assetName, Action<T> onComplete) where T : UnityEngine.Object
        {
            if(!loadedBundles.TryGetValue(bundleName, out AssetBundle bundle))
            {
                Debug.LogError($"Bundle {bundleName} not loaded!");
                yield break;
            }

            AssetBundleRequest request = bundle.LoadAssetAsync<T>(assetName);
            yield return request;

            if(request.asset != null)
            {
                onComplete?.Invoke(request.asset as T);
            }
            else
            {
                Debug.LogError($"Failed to load asset {assetName} from {bundleName}");
            }
        }

        private IEnumerator LoadAssetsCoroutine<T>(string bundleName, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            if(!loadedBundles.TryGetValue(bundleName, out AssetBundle bundle))
            {
                Debug.LogError($"Bundle {bundleName} not loaded!");
                yield break;
            }

            AssetBundleRequest request = bundle.LoadAllAssetsAsync<T>();
            yield return request;

            if(request.allAssets.Length > 0)
            {
                onComplete?.Invoke(request.allAssets as T[]);
            }
            else
            {
                Debug.LogError($"Failed to load assets from {bundleName}");
            }
        }
        #endregion

        #region Load Asset
        public void LoadAssetDirect<T>(BundleConfig config, string assetName, Action<T> onComplete) where T : UnityEngine.Object
        {
            if(loadedBundles.ContainsKey(config.BundleName))
            {
                LoadAsset<T>(config.BundleName, assetName, onComplete);
                return;
            }

            LoadBundle(config, () => LoadAsset<T>(config.BundleName, assetName, onComplete));
        }
        #endregion

        #region Load Assets
        public void LoadAssetsDirect<T>(BundleConfig config, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            if(loadedBundles.ContainsKey(config.BundleName))
            {
                LoadAssets<T>(config.BundleName, onComplete);
                return;
            }

            LoadBundle(config, () => LoadAssets<T>(config.BundleName, onComplete));
        }
        #endregion

        #region Load Local
        private IEnumerator LoadLocalAssetAsync<T>(string path, string asset, Action<T> callback) where T : UnityEngine.Object
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            AssetBundle bundle = bundleRequest.assetBundle;
            if (bundle != null)
            {
                AssetBundleRequest assetRequest = bundle.LoadAssetAsync<T>(asset);
                yield return assetRequest;

                T a = assetRequest.asset as T;
                callback?.Invoke(a);
                bundle.Unload(false);
            }
            else
            {
                Debug.LogError($"Failed to load local bundle from: {path}");
            }
        }

        private IEnumerator LoadLocalAssetsAsync<T>(string path, Action<T[]> callback) where T : UnityEngine.Object
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;

            AssetBundle bundle = bundleRequest.assetBundle;
            if (bundle != null)
            {
                AssetBundleRequest assetRequest = bundle.LoadAllAssetsAsync<T>();
                yield return assetRequest;

                T[] assets = assetRequest.allAssets as T[];
                callback?.Invoke(assets);
                bundle.Unload(false);
            }
            else
            {
                Debug.LogError($"Failed to load local bundle from: {path}");
            }
        }
        #endregion

        #region Load Remote
        private IEnumerator LoadRemoteAssetAsync<T>(string url, string asset, Action<T> callback) where T : UnityEngine.Object
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                if (bundle != null)
                {
                    AssetBundleRequest assetRequest = bundle.LoadAssetAsync<T>(asset);
                    yield return assetRequest;

                    T a = assetRequest.asset as T;
                    callback?.Invoke(a);
                    bundle.Unload(false);
                }
                else
                {
                    Debug.LogError($"Failed to extract bundle from remote URL: {url}");
                }
            }
            else
            {
                Debug.LogError($"Failed to download remote bundle from {url}: {request.error}");
            }
        }

        private IEnumerator LoadRemoteAssetsAsync<T>(string url, Action<T[]> callback) where T : UnityEngine.Object
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                if (bundle != null)
                {
                    AssetBundleRequest assetRequest = bundle.LoadAllAssetsAsync<T>();
                    yield return assetRequest;

                    T[] assets = assetRequest.allAssets as T[];
                    callback?.Invoke(assets);
                    bundle.Unload(false);
                }
                else
                {
                    Debug.LogError($"Failed to extract bundle from remote URL: {url}");
                }
            }
            else
            {
                Debug.LogError($"Failed to download remote bundle from {url}: {request.error}");
            }
        }
        #endregion
    }
}
