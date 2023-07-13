using UnityEngine;
using System.IO;

namespace Excalibur
{
    /// <summary> Const Params /// </summary>
    public static class CP
    {
        public const string
            AssetsDir = "Assets",
            ScenesDir = "Scenes",
            ShadersDir = "Shaders",
            BundleOutputPathKey = "BundleOutputPath",
            BundleIdentifiersKey = "BundleIdentifiers",
            AssetBundleConfig = "abconfig.json",
            ProjectPresets = "ProjectPresets",
            ProjectSettings = "Settings";

        public static string GetAssetBundleConfigPath ()
        {
            return Path.Combine(Application.dataPath, AssetBundleConfig);
        }
    }
}