using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.Utils.Web
{
    [CreateAssetMenu]
    public class CdnLoaderData : ScriptableObject
    {
        [Tooltip("Enable or disable loading of CDN scripts.")]
        public bool allowLoadScript = true;

        [SerializeField]
        [Tooltip("List of script URLs to load from CDN (e.g., 'https://cdn.example.com/script.js').")]
        private List<string> scriptList = new();

        [Tooltip("Enable or disable loading of CDN modules.")]
        public bool allowLoadModule = true;

        [SerializeField]
        [Tooltip("List of module URLs to load from CDN (e.g., 'https://cdn.example.com/module.js').")]
        private List<string> moduleList = new();

        public List<string> ScriptList => scriptList;
        public List<string> ModuleList => moduleList;
    }
}