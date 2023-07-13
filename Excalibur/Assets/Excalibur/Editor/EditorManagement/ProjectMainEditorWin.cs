using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Excalibur
{
    public class MainEditorWindow : EditorWindow
    {
        string[] presetArr;

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
        }

        private void _CustomCommands()
        {
            if (GUILayout.Button("Generate ABConfig"))
            {
                AssetBundleBuilder.GenerateAssetBundleConfig();
            }
        }
    }
}
