using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public static class IOAssistant
    {
        public static string FileExt_Meta => ".meta";
        public static string FileExt_CS => ".cs";
        public static string FileExt_Txt => ".txt";
        public static string FileExt_Shader => ".shader";
        public static string FileExt_Scene => ".unity";
        public static string FileExt_SpriteAtlas => ".spriteatlas";
        public static string FileExt_Png => ".png";
        public static string FileExt_Jpg => ".jpg";
        public static string FileExt_Anim => ".anim";

        public static string ConvertPath (string path)
        {
            if (path.Contains ("\\")) { path = path.Replace ("\\", "/"); }
            return path;
        }

        public static string ConvertToUnityRelativePath (string path)
        {
            if (!AssetPathVerify (path)) { return string.Empty; }
            path = ConvertPath (path);
            path = path.Substring (path.IndexOf ("Assets/"));
            return path;
        }

        public static bool AssetPathVerify (string path)
        {
            if (!path.Contains ("Assets"))
            {
                Debug.Log (string.Format ("转换为unity相对路径需要为unity的Assets中的目录{0}", path));
                return false;
            }
            return true;
        }

        public static string[] GetDirectories (string path, string searchPattern, SearchOption searchOption)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                searchPattern = "*.*";
            }
            string[] directories = Directory.GetDirectories (path, searchPattern, searchOption);
            for (int i = 0; i < directories.Length; ++i)
            {
                directories[i] = ConvertPath(directories[i]);
            }
            return directories;
        }

        public static string[] GetFiles (string path, string searchPattern, SearchOption searchOption, Func<string, bool> searchCondition = null)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                searchPattern = "*.*";
            }
            string[] files = Directory.GetFiles (path, searchPattern, searchOption);
            List<string> result = new List<string> ();
            for (int i = 0; i < files.Length; ++i)
            {
                if (searchCondition != null)
                {
                    if (searchCondition (files[i]))
                    {
                        result.Add (ConvertPath(files[i]));
                    }
                }
                else
                {
                    result.Add (ConvertPath(files[i]));
                }
            }
            return result.ToArray ();
        }

        public static string CombinePath (string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public static string CombinePath(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3);
        }

        public static string CombinePath(string path1, string path2, string path3, string path4)
        {
            return Path.Combine(path1, path2, path3, path4);
        }

        public static string CombinePath(string path1, string path2, string path3, string path4, string path5)
        {
            return Path.Combine(CombinePath(path1, path2, path3, path4), path5);
        }
    }
}
