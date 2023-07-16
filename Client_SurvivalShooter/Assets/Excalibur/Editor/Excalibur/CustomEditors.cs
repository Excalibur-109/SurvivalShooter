using UnityEngine;
using UnityEditor;

namespace Excalibur
{
    [CustomEditor(typeof(CaptureBehaviour))]
    public class ComponentCacheEditor : Editor
    {
        CaptureBehaviour capture;

        private void OnEnable()
        {
            capture = target as CaptureBehaviour;
        }

        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                base.OnInspectorGUI();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Attach"))
                {
                    capture.AttachChilds();
                }
                if (capture.GetComponent<ComponentCaches>())
                {
                    if (GUILayout.Button("Cache"))
                    {
                        capture.CaptureCaches();
                    }
                }
            }
        }
    }
}
