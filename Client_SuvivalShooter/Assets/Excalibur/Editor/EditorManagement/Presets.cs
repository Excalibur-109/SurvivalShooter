using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.U2D;
using UnityEditor.U2D;

namespace Excalibur
{
    public class AssetBundleBuildPreset : IWindowDrawer
    {
        public string assetsPath;
        public string scenesPath;
        public string shadersPath;
        public string outPath;
        public BuildAssetBundleOptions buildAssetBundleOption = BuildAssetBundleOptions.None;
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;

        public string title => "AssetBundleBuild";

        public void Save(IDataWriter writer)
        {
            writer.Write(assetsPath);
            writer.Write(scenesPath);
            writer.Write(shadersPath);
            writer.Write(outPath);
            writer.Write((int)buildAssetBundleOption);
            writer.Write((int)buildTarget);
        }

        public void Load(IDataReader reader)
        {
            assetsPath = reader.ReadString();
            scenesPath = reader.ReadString();
            shadersPath = reader.ReadString();
            outPath = reader.ReadString();
            buildAssetBundleOption = (BuildAssetBundleOptions)reader.ReadInt();
            buildTarget = (BuildTarget)reader.ReadInt();
        }

        public void OpenWindow()
        {
            EditorUIManager.Instance.OpenWindow<PresetsWindow>(this);
        }

        public void OnEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("资源路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(assetsPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref assetsPath, "选择资源路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("场景路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(scenesPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref scenesPath, "选择场景路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("着色器路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(shadersPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref shadersPath, "选择着色器路径");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("输出路径", GUILayout.Width(120f));
            EditorGUILayout.TextField(outPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref outPath, "选择");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            buildAssetBundleOption = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(buildAssetBundleOption);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTarget);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Assets"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Assets);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Scenes"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Scenes);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Shaders"))
            {
                AssetBundleBuilder.BuildAssetBundles(AssetsManager.BundleAssets.Shaders);
            }
            EditorGUILayout.EndHorizontal();
        }

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }

    public class ConfigurationPreset : IWindowDrawer
    {
        private const string FOLDER = "Tables";

        public string destinationPath;
        public string classPath;
        public string selectedDirectory;

        public string title => "Configuration";

        static Vector2 scrollPos;
        static string[] directories;
        static int selectedIndex, tempIndex;

        FormatExcelClsHandler clsHandler;
        FormatXMLHandler xmlHandler;

        public void OnEditorGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            tempIndex = EditorGUILayout.Popup(selectedIndex, directories);
            if (tempIndex != selectedIndex)
            {
                selectedDirectory = directories[tempIndex];
                selectedIndex = tempIndex;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("配置导出路径", GUILayout.Width(80f));
            EditorGUILayout.TextField(destinationPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref destinationPath, "选择导出路径", () =>
                    {
                        string path = Path.Combine(Application.dataPath, destinationPath);
                    });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("类导出路径", GUILayout.Width(80f));
            EditorGUILayout.TextField(classPath);
            if (GUILayout.Button("选择", GUILayout.Width(100f)))
            {
                EditorUtil.ChooseAssetsDirectory(ref classPath, "选择导出路径", () =>
                {
                    string path = Path.Combine(Application.dataPath, classPath);
                });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成相关类"))
            {
                clsHandler = new FormatExcelClsHandler();
                clsHandler.SetSrc(Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER, selectedDirectory));
                clsHandler.SetDst(Path.Combine(Application.dataPath, classPath));
                EditorUtility.ClearProgressBar();
                clsHandler.Serialize();
            }
            if (clsHandler != null)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Generating classes", "generate" + clsHandler.processInfo, clsHandler.process);
                    if (clsHandler.process >= 1f)
                    {
                        clsHandler = null;
                        EditorUtility.ClearProgressBar();
                        AssetDatabase.Refresh();
                    }
                }
                catch (ArgumentNullException e)
                {
                    clsHandler = null;
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                    throw e;
                }
            }
            if (GUILayout.Button("生成配置"))
            {
                xmlHandler = new FormatXMLHandler();
                xmlHandler.SetSrc(Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER, selectedDirectory));
                xmlHandler.SetDst(Path.Combine(Application.dataPath, destinationPath));
                EditorUtility.ClearProgressBar();
                xmlHandler.Serialize();
            }
            if (xmlHandler != null)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Generating configs", "generate" + xmlHandler.processInfo, xmlHandler.process);
                    if (xmlHandler.process >= 1f)
                    {
                        xmlHandler = null;
                        EditorUtility.ClearProgressBar();
                        AssetDatabase.Refresh();
                    }
                }
                catch (ArgumentNullException e)
                {
                    xmlHandler = null;
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                    throw e;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void OpenWindow()
        {
            EditorUIManager.Instance.OpenWindow<PresetsWindow>(this);
        }

        public void Load(IDataReader reader)
        {
            destinationPath = reader.ReadString();
            classPath = reader.ReadString();
            selectedDirectory = reader.ReadString();
        }

        public void Save(IDataWriter writer)
        {
            writer.Write(destinationPath);
            writer.Write(classPath);
            writer.Write(selectedDirectory);
        }

        public void Initialize()
        {
            string path = Path.Combine(EditorProjectPreset.GameAssetsPath, FOLDER);
            directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; ++i)
            {
                string directory = directories[i];
                for (int j = directories[i].Length - 1; j >= 0; --j)
                {
                    char value = directory[j];
                    if (value == '\\')
                    {
                        directories[i] = directory.Substring(j + 1);
                        break;
                    }
                }
                if (selectedIndex == -1 && selectedDirectory == directories[i])
                {
                    selectedIndex = i;
                }
            }
        }

        public void Terminate()
        {
            directories = null;
            selectedIndex = -1;
            if (!string.IsNullOrEmpty(destinationPath))
            {
                ProjectSettings settings = Resources.Load<ProjectSettings>(CP.ProjectSettings);
                if (settings != null && settings.configurationPath != destinationPath)
                {
                    settings.configurationPath = destinationPath;
                }
            }
        }
    }
}
