using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Excalibur.AssetsManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using UnityEditor.Presets;

namespace Excalibur
{
    public static class AssetBundleBuilder
    {
        public enum BuildOption
        {
            OneAssetOneBundle,
            [Obsolete("unfinished")]
            MininumDependency,
        }

        private class ABBuildeInfo
        {
            public string file;
            public Dictionary<string, ABBuildeInfo> dependDic = new Dictionary<string, ABBuildeInfo>();
            public Dictionary<string, ABBuildeInfo> dependedDic = new Dictionary<string, ABBuildeInfo>();
        }

        public static BundleAssets buildAssets;
        private static AssetBundleBuildPreset preset;

        public static void BuildAssetBundles (BundleAssets build)
        {
            buildAssets = build;
            preset = (AssetBundleBuildPreset)EditorProjectPreset.Instance.GetPreset (EditorPreset.AssetBundleBuild);
            string srcPath = string.Empty;
            switch (build)
            {
                case BundleAssets.Assets:
                    srcPath = preset.assetsPath;
                    break;
                case BundleAssets.Scenes:
                    srcPath = preset.scenesPath;
                    break;
                case BundleAssets.Shaders:
                    srcPath = preset.shadersPath;
                    break;
            }
            if (string.IsNullOrEmpty(srcPath))
            {
                EditorUtility.DisplayDialog("Warning", $"打包路径不存在", "OK");
                return;
            }
            srcPath = IOAssistant.ConvertPath(Path.Combine(Application.dataPath, srcPath));

            string dstPath = Path.Combine(_GetOutputPath(), GetBuildFolder(buildAssets));
            if (!Directory.Exists(dstPath))
            {
                Directory.CreateDirectory(dstPath);
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetBundleBuild[] builds = _RequestBuildAssetBundleFunc(BuildOption.OneAssetOneBundle)(srcPath);

            string variant = GetBundleVariant(build);
            for (int i = 0; i < builds.Length; ++i)
            {
                builds[i].assetBundleVariant = variant;
            }

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(dstPath, builds, preset.buildAssetBundleOption, preset.buildTarget);
            string[] all = manifest.GetAllAssetBundles();
            AssetDatabase.Refresh();
        }

        private static Func<string, AssetBundleBuild[]> _RequestBuildAssetBundleFunc (BuildOption option)
        {
            Func<string, AssetBundleBuild[]> func = default;
            switch (option)
            {
                case BuildOption.OneAssetOneBundle:
                    func = _OneAssetOneBundleBuild;
                    break;
                case BuildOption.MininumDependency:
                    break;
            }
            return func;
        }

        private static AssetBundleBuild[] _OneAssetOneBundleBuild (string sourcePath)
        {
            List<string> assetPaths = IOAssistant.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories,
                assetPath => _ValidateAssetPath(assetPath)).ToList();
            HashSet<string> bundles = new HashSet<string>();
            assetPaths.ForEach(e =>
            {
                e = IOAssistant.ConvertToUnityRelativePath(e);
                if (!bundles.Contains(e)) { bundles.Add(e); }
                AssetDatabase.GetDependencies(e, true).Where(e => _ValidateAssetPath(e)).ToList().ForEach(v =>
                {
                    if (!bundles.Contains(v)) { bundles.Add(v); }
                });
            });
            AssetBundleBuild[] builds = new AssetBundleBuild[bundles.Count];
            int i = 0;
            foreach (string assetBundle in bundles)
            {
                builds[i].assetBundleName = Path.GetFileNameWithoutExtension(assetBundle);
                builds[i++].assetNames = new string[] { assetBundle };
            }
            return builds;
        }

        private static bool _ValidateAssetPath(string assetsPath)
        {
            if (assetsPath.Contains(IOAssistant.FileExtension_Meta) || assetsPath.Contains(IOAssistant.FileExtension_CS))
            {
                return false;
            }
            bool valid = true;
            switch (buildAssets)
            {
                case BundleAssets.Assets:
                    valid = !(assetsPath.Contains(IOAssistant.FileExtension_Shader) || assetsPath.Contains(IOAssistant.FileExtension_Scene));
                    break;
                case BundleAssets.Scenes:
                    valid = assetsPath.Contains(IOAssistant.FileExtension_Scene);
                    break;
                case BundleAssets.Shaders:
                    valid = assetsPath.Contains(IOAssistant.FileExtension_Shader);
                    break;
            }
            return valid;
        }

        private static string _GetOutputPath()
        {
            string dstPath;
            if (string.IsNullOrEmpty(preset.outPath))
            {
                dstPath = Application.streamingAssetsPath;
            }
            else
            {
                dstPath = Path.Combine(Application.dataPath, preset.outPath);
            }
            return IOAssistant.ConvertToUnityRelativePath(dstPath);
        }

        public static void GenerateAssetBundleConfig()
        {
            preset = (AssetBundleBuildPreset)EditorProjectPreset.Instance.GetPreset (EditorPreset.AssetBundleBuild);
            JObject jObject = new JObject();
            string outputPath = _GetOutputPath();
            Dictionary<string, IdentifyManifest> manifestDic = new Dictionary<string, IdentifyManifest>();
            for (BundleAssets i = 0; i < BundleAssets.Count; ++i)
            {
                string currentPath = Path.Combine(outputPath, GetBuildFolder(i));
                string mainPath = Path.Combine(currentPath, GetBuildFolder(i));
                if (!File.Exists(mainPath)) { continue; }
                byte[] bytes = File.ReadAllBytes(mainPath);
                AssetBundle main = AssetBundle.LoadFromMemory(bytes);
                AssetBundleManifest mainFest = main.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                string variant = GetBundleVariant(i);
                string[] files = IOAssistant.GetFiles(currentPath, string.Format("*.{0}", variant), SearchOption.AllDirectories);
                for (int j = 0; j < files.Length; ++j)
                {
                    string file = Path.GetFileNameWithoutExtension(files[j]);
                    IdentifyManifest identify = new IdentifyManifest()
                    {
                        assetBundleName = file,
                        path = Path.Combine(currentPath, string.Format("{0}.{1}", file, variant)).ToLower(),
                };
                    List<string> depends = mainFest.GetAllDependencies(string.Format("{0}.{1}", file, variant)).ToList();
                    if (depends.Count > 0)
                    {
                        identify.dependencies = new string[depends.Count];
                        int t = 0;
                        depends.ForEach(e =>
                        {
                            identify.dependencies[t++] = e.Split('.')[0];
                        });
                    }
                    manifestDic[file] = identify;
                }
                main.Unload(true);
            }
            jObject[CP.BundleIdentifiersKey] = JsonConvert.SerializeObject(manifestDic, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(CP.GetAssetBundleConfigPath(), false))
            {
                writer.Write(jObject.ToString());
                writer.Dispose();
            }
        }
    }
}