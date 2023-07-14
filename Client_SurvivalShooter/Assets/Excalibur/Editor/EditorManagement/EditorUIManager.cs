using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Excalibur
{
    public class EditorUIManager : Singleton<EditorUIManager>
    {
        private Dictionary<string, UIEditorWindow> winCaches = new Dictionary<string, UIEditorWindow>();

        public void OpenWindow<T>(IWindowDrawer drawer) where T : UIEditorWindow
        {
            winCaches = winCaches.Where(e => e.Value != null).ToDictionary(pair => pair.Key, pair => pair.Value);
            if (winCaches.ContainsKey(drawer.title))
            {
                Debug.LogWarningFormat("{0}Ѿ", drawer.title);
                return;
            }
            drawer.Initialize();
            T t = ScriptableObject.CreateInstance<T>();
            t.titleContent = new GUIContent(drawer.title);
            t.SetDrawer(drawer);
            winCaches.Add(drawer.title, t);
        }

        internal void OnWindowClose(IWindowDrawer drawer)
        {
            if (drawer != null)
            {
                drawer.Terminate();
                if (winCaches.ContainsKey(drawer.title))
                {
                    winCaches.Remove(drawer.title);
                }
            }
        }
    }

    public class UIEditorWindow : EditorWindow
    {
        protected IWindowDrawer drawer;

        public void SetDrawer (IWindowDrawer drawer)
        {
            this.drawer = drawer;
            Show();
        }

        private void OnDisable()
        {
            EditorUIManager.Instance.OnWindowClose(drawer);
        }
    }

    public class PresetsWindow : UIEditorWindow
    {
        private void OnGUI()
        {
            if (drawer != null)
            {
                drawer.OnEditorGUI();
            }
        }
    }
}
