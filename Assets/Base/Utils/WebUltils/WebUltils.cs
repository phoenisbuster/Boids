using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace WebUltils
{
    public static class WebGLCdnHelper
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void LoadCDNScript(string url, bool cached, Action callback, Action<string> errorCallback);

        [DllImport("__Internal")]
        private static extern void LoadCDNModule(string url, bool cached, Action callback, Action<string> errorCallback);
        #endif

        public static void LoadScript(string url, bool cached, Action callback, Action<string> errorCallback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                LoadCDNScript(url, cached, callback, errorCallback);
            #else
                Debug.LogWarning($"LoadScript is a WebGL-only feature. Simulating for '{url}'.");
                callback?.Invoke();
            #endif
        }

        public static void LoadModule(string url, bool cached, Action callback, Action<string> errorCallback)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                LoadCDNModule(url, cached, callback, errorCallback);
            #else
                Debug.LogWarning($"LoadModule is a WebGL-only feature. Simulating for '{url}'.");
                callback?.Invoke();
            #endif
        }
    }

    public static class ByteConverter
    {
        public static T ParseBase64ToJsonObject<T>(byte[] data)
        {
            // Convert byte[] to Base64 string
            string base64String = Convert.ToBase64String(data);

            // Decode Base64 string back to JSON string
            string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            // Parse JSON string into an object
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static T ParseByteArrayToJsonObject<T>(byte[] data)
        {
            if (data == null || data.Length == 0) return default;

            // Convert byte[] (Uint8Array) to JSON string
            string jsonString = Encoding.UTF8.GetString(data);

            // Parse JSON string into an object
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static byte[] EndcodeObjectToByteArray<T>(T obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}  
