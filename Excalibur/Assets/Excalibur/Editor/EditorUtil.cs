using UnityEngine;
using UnityEditor;
using Excalibur.Algorithms;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace Excalibur
{
    public static class EditorUtil
    {
        public static string GetPathNameInAssets (string path)
        {
            if (!string.IsNullOrEmpty(path) && KMP.Search(path, "Assets") >= 0)
            {
                path = path.Replace("Assets/", "");
            }
            return path;
        }

        public static void ChooseAssetsDirectory(ref string target, string title, Action successAction = default)
        {
            string path = OpenFolderPanel(title, Application.dataPath, "");
            if (!string.IsNullOrEmpty(path) && CheckPathInAssets(path))
            {
                target = GetPathNameInAssets(path);
                successAction?.Invoke();
            }
        }

        public static bool CheckPathInAssets (string path)
        {
            if (KMP.Search(path, "Assets") < 0)
            {
                EditorUtility.DisplayDialog("提示", "该路径不在Assets下", "确认");
                return false;
            }
            return true;
        }

        public static string OpenFilePanel (string title, string directory, string extension, bool assetsPath = true)
        {
            string path = EditorUtility.OpenFilePanel(title, directory, extension);
            if (!string.IsNullOrEmpty(path) && assetsPath)
            {
                path = IOAssistant.ConvertToUnityRelativePath(path);
            }
            return path;
        }

        public static string OpenFolderPanel (string title, string directory, string extension, bool assetsPath = true)
        {
            string path = EditorUtility.OpenFolderPanel(title, directory, extension);
            if (!string.IsNullOrEmpty(path) && assetsPath)
            {
                path = IOAssistant.ConvertToUnityRelativePath(path);
            }
            return path;
        }
    }
}
