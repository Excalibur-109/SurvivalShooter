using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excalibur
{
    public class MainEditorWindow : EditorWindow
    {
        static string[] presetArr;

        Vector2 scrollPos;

        [MenuItem("Excalibur/Main Window &A", false, 400)]
        static void OpenMainWin()
        {
            GetWindow<MainEditorWindow>("Main Window").Show();
        }

        private void OnEnable()
        {
            EditorProjectPreset.Instance.Load();
            presetArr = Enum.GetNames(typeof(EditorPreset));
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < presetArr.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(string.Format("Open {0}", presetArr[i])))
                {
                    EditorPreset preset = (EditorPreset)Enum.Parse(typeof(EditorPreset), presetArr[i]);
                    EditorProjectPreset.Instance.GetPreset(preset).OpenWindow();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            _CustomCommands();
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void OnDisable()
        {
            EditorProjectPreset.Instance.Save();
            EditorProjectPreset.Instance.ClearCaches();
        }

        private void _CustomCommands()
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Generate ABConfig"))
            {
                AssetBundleBuilder.GenerateAssetBundleConfig();
            }

            if (GUILayout.Button("Generate Editor Assets Config"))
            {
                List<string> exclude = new List<string>()
                {
                    IOAssistant.FileExt_Anim, IOAssistant.FileExt_Meta
                };

                AssetBundleBuildPreset preset = (AssetBundleBuildPreset)EditorProjectPreset.Instance.GetPreset(EditorPreset.AssetBundleBuild);
                string path = Path.Combine(Application.dataPath, preset.assetsPath);
                string[] files = IOAssistant.GetFiles(path, "*.*", SearchOption.AllDirectories, file => !exclude.Contains(file));
                JObject jObect = new JObject();
                for (int i = 0; i < files.Length; ++i)
                {
                    string fileName = Path.GetFileNameWithoutExtension(files[i]);
                    jObect[fileName] = IOAssistant.ConvertToUnityRelativePath(files[i]);
                }
                File.WriteAllText(CP.GetEditorAssetConfigPath(), jObect.ToString());
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
