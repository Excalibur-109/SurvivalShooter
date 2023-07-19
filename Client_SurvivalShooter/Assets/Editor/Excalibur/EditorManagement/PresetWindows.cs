using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excalibur
{
    public class GenComponentData : EditorWindow
    {
        ComponentsGen componentsGen;

        private void OnEnable()
        {
            componentsGen = (ComponentsGen)EditorProjectPreset.Instance.GetPreset(EditorPreset.ComponentGen);
            componentsGen.Initialize();
        }

        private void OnGUI()
        {
            componentsGen.OnEditorGUI();
        }
    }
}
