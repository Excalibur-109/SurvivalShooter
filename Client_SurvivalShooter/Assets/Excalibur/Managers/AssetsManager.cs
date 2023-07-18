using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using OfficeOpenXml.ConditionalFormatting;
using UnityEditor;
#if UNITY_EDITOR && ASSET_BUNDLE_LOAD_DISENABLE
using UnityEditor;
#endif

namespace Excalibur
{
    /*
     * 外部增加一个引用，引用为空时从AssetBundle加载，引用不为空时，需要该类型的资源就从该引用获取
     */

    public sealed partial class AssetsManager : Singleton<AssetsManager>, IExecutableBehaviour
    {
        /// <summary> 正在加载的AssetBundle /// </summary>
        private readonly HashSet<string> r_LoadingBundles = new HashSet<string>();
        /// <summary> AssetBundle中正在加载的Asset /// </summary>
        private readonly Dictionary<string, AssetBundleRequest> r_LoadingAssets = new Dictionary<string, AssetBundleRequest>();
        /// <summary> 加载完成的AssetBundle /// </summary>
        private readonly Dictionary<string, AssetBundle> r_LoadedBundles = new Dictionary<string, AssetBundle>();
        /// <summary> 卸载队列 /// </summary>
        private readonly Queue<string> r_UnloadQueue = new Queue<string>();
        /// <summary> 该包被依赖的数量 /// </summary>
        private readonly Dictionary<string, int> r_BundleRefCount = new Dictionary<string, int>();
        private static readonly object _locker = new object();

        /// <summary> 配置文件记录的 Asset 和 AssetBundle 的信息 /// </summary>
        private Dictionary<string, IdentifyManifest> _identifierDic = new Dictionary<string, IdentifyManifest>();
#if UNITY_EDITOR && ASSET_BUNDLE_LOAD_DISENABLE
#endif
        private Dictionary<string, string> _editorAssetsConfig;

        public bool Executable { get; set; }

        protected override void OnConstructed()
        {
#if UNITY_EDITOR && ASSET_BUNDLE_LOAD_DISENABLE
#endif
            string jsonStr = File.ReadAllText(CP.GetEditorAssetConfigPath());
            _editorAssetsConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

            //string jsonStr = File.ReadAllText(CP.GetAssetBundleConfigPath());
            //JObject jObject = JObject.Parse(jsonStr);
            //_identifierDic = JsonConvert.DeserializeObject<Dictionary<string, IdentifyManifest>>((string)jObject[CP.BundleIdentifiersKey]);
            //AssetBundle.UnloadAllAssetBundles(true);
            //GameManager.Instance.AttachExecutableUnit(this);
        }

        public void Execute ()
        {
            if (Executable)
            {
                if (r_UnloadQueue.Count == 0)
                {
                    Executable = false;
                    return;
                }
                _UnloadAssetBundle(r_UnloadQueue.Dequeue());
            }
        }

        public void LoadAsset<T> (string assetName, System.Action<T> onComplete = null) where T : Object
        {
            if (_editorAssetsConfig.TryGetValue(assetName, out string assetPath))
            {
                T result = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                onComplete?.Invoke(result);
            }
            else
            {
                Debug.LogWarningFormat("Editor下不存在资源：{0}", assetName);
            }
            return;
#if UNITY_EDITOR && ASSET_BUNDLE_LOAD_DISENABLE
#else
            _LoadAsset<T>(assetName, onComplete);
#endif
        }

        public void UnloadAllAssetBundles (bool unloadAllLoadedObjects = false)
        {
            foreach (AssetBundle bundle in r_Bundles.Values)
            {
                bundle.Unload(unloadAllLoadedObjects);
            }
            r_Bundles.Clear();
            r_BundleRefCount.Clear();
        }

        private async void _LoadAsset<T>(string assetName, System.Action<T> onComplete = null) where T : Object
        {
            assetName = assetName.ToLower();
            await _LoadDependencies(_GetDependencies(assetName));
            AssetBundle assetBundle = await _LoadAssetBundleAsync(assetName);
            try
            {
                if (r_LoadingAssets.ContainsKey(assetName))
                {
                    while (!r_LoadingAssets[assetName].isDone) 
                    {
                        await Task.Yield();
                    }
                    onComplete?.Invoke((T)r_LoadingAssets[assetName].asset);
                    return;
                }

                AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetName);
                r_LoadingAssets.Add(assetName, request);
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                r_LoadingAssets.Remove(assetName);
                onComplete?.Invoke((T)request.asset);
                _TryUnloadBundle(assetName);
            }
            catch (System.ArgumentNullException e)
            {
                throw e;
            }
        }

        private async Task<AssetBundle> _LoadAssetBundleAsync (string assetName)
        {
            string bundleName = _GetAssetBundleName(assetName);

            while (r_LoadingBundles.Contains(bundleName)) 
            {
                await Task.Yield();
            }

            if (r_LoadedBundles.ContainsKey(bundleName))
            {
                return r_LoadedBundles[bundleName];
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(_GetAssetBundlePath(assetName));
            r_LoadingBundles.Add(bundleName);
            while (!request.isDone)
            {
                await Task.Yield();
            }
            r_LoadingBundles.Remove(bundleName);
            AssetBundle assetBundle = request.assetBundle;
            if (assetBundle != null)
            {
                r_LoadedBundles.Add(bundleName, assetBundle);
                /// 对自己进行依赖，避免加载该包依赖中的资源，卸载计数key不存在
                //_ChangeRefCountOnLoad(bundleName);
            }
            return assetBundle;
        }

        private async Task _LoadDependencies (string[] dependencies)
        {
            if (dependencies == null) { return; }
            Task<AssetBundle>[] tasks = new Task<AssetBundle>[dependencies.Length];
            for (int i = 0; i < dependencies.Length; ++i)
            {
                tasks[i] = _LoadAssetBundleAsync(dependencies[i]);
                _ChangeRefCountOnLoad(dependencies[i]);
            }
            await Task.WhenAll(tasks);
        }

        private void _ChangeRefCountOnLoad (string bundleName)
        {
            if (!r_BundleRefCount.ContainsKey(bundleName))
            {
                r_BundleRefCount.Add(bundleName, 0);
            }
            ++r_BundleRefCount[bundleName];
        }

        private void _ChangeRefCountOnUnload (string bundleName)
        {
            if (r_BundleRefCount.ContainsKey(bundleName))
            {
                --r_BundleRefCount[bundleName];
            }
        }

        private void _TryUnloadBundle (string assetName)
        {
            lock (_locker)
            {
                string bundleName = _GetAssetBundleName(assetName);
                _ChangeRefCountOnUnload(bundleName);
                if (!r_BundleRefCount.ContainsKey(bundleName) || r_BundleRefCount[bundleName] == 0)
                {
                    r_UnloadQueue.Enqueue(bundleName);
                    string[] dependencies = _GetDependencies(assetName);
                    if (dependencies != null)
                    {
                        for (int i = 0; i < dependencies.Length; ++i)
                        {
                            _ChangeRefCountOnUnload(dependencies[i]);
                            if (!r_BundleRefCount.ContainsKey(dependencies[i]) || r_BundleRefCount[dependencies[i]] == 0)
                            {
                                r_UnloadQueue.Enqueue(dependencies[i]);
                            }
                        }
                    }
                    Executable = true;
                }
            }
        }

        private void _UnloadAssetBundle (string bundleName, bool unloadAllLoadedObjects = false)
        {
            if (r_BundleRefCount.ContainsKey(bundleName))
            {
                r_BundleRefCount.Remove(bundleName);
            }

            if (r_LoadedBundles.ContainsKey(bundleName))
            {
                r_LoadedBundles[bundleName].Unload(unloadAllLoadedObjects);
                r_LoadedBundles.Remove(bundleName);
            }
        }

        private string _GetAssetBundleName (string assetName)
        {
            return _identifierDic[assetName].assetBundleName;
        }

        private string _GetAssetBundlePath (string assetName)
        {
            return _identifierDic[assetName].path;
        }

        private string[] _GetDependencies (string assetName)
        {
            return _identifierDic[assetName].dependencies;
        }

        public void Print()
        {
            Utility.PrintDictionary("Loaded", r_LoadedBundles);
            Utility.PrintDictionary("RefCount", r_BundleRefCount);
        }
    }

    public partial class AssetsManager
    {
        private readonly Dictionary<string, AssetBundle> r_Bundles = new Dictionary<string, AssetBundle>();
        private readonly Dictionary<string, uint> r_CitationCounts = new Dictionary<string, uint>();

        private class ABLoader
        {
            public string file;
            public AssetBundle bundle;
            public uint referenceCount;
            public Dictionary<string, ABLoader> depend;
            public Dictionary<string, ABLoader> depended;
        }

        public class IdentifyManifest
        {
            public string assetBundleName;
            public string path;
            public string[] dependencies;
        }

        public enum BundleAssets { Assets, Scenes, Shaders, Count }

#region static

        public static string GetBuildFolder(BundleAssets bundleAssets)
        {
            switch (bundleAssets)
            {
                case BundleAssets.Assets:
                    return CP.AssetsDir;
                case BundleAssets.Scenes:
                    return CP.ScenesDir;
                case BundleAssets.Shaders:
                    return CP.ShadersDir;
            }
            return string.Empty;
        }

        public static string GetBundleVariant (BundleAssets bundleAssets)
        {
            string variant = string.Empty;
            switch (bundleAssets)
            {
                case BundleAssets.Assets:
                    variant = "asset";
                    break;
                case BundleAssets.Scenes:
                    variant = "scene";
                    break;
                case BundleAssets.Shaders:
                    variant = "shaderlab";
                    break;
            }
            return variant;
        }

#endregion
    }
}