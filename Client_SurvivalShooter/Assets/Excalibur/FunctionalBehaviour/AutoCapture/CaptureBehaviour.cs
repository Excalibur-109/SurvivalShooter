using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using System;

namespace Excalibur
{
    [DisallowMultipleComponent, AddComponentMenu ("Excalibur/Fundamental/Component Cache")]
    public sealed class CaptureBehaviour : MonoBehaviour
    {
        [SerializeField] private List<Transform> _caches = new List<Transform> ();
        private Dictionary<string, Transform> _cacheDic = new Dictionary<string, Transform> ();

        public Transform this[string name] => GetTransform(name);

        private void Awake ()
        {
            if (_caches.Count == 0) { AttachChilds (); }
            for (int i = 0; i < _caches.Count; ++i)
            {
                _cacheDic[_caches[i].name] = _caches[i];
            }
        }

        public void AttachChilds ()
        {
            if (Application.isPlaying) { return; }
            if (_caches.Count > 0) { _caches.Clear (); }
            _caches.Add (transform);
            _caches.AddRange (transform.AttachChilds ());
        }

        public void CaptureCaches()
        {
            if (Application.isPlaying) { return; }
            ComponentCaches caches = GetComponent<ComponentCaches>();
            CaptureComponentsReflection(caches);
        }

        public void CaptureComponentsReflection<T>(T instance)
        {
            FieldInfo[] fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo field = fields[i];
                AutoCaptureAttribute attribute = field.GetCustomAttribute<AutoCaptureAttribute>();
                if (attribute != null)
                {
                    switch (attribute.capture)
                    {
                        case Capture.Component:
                            field.SetValue(instance, GetTransform(attribute.name).GetComponent(attribute.component));
                            break;
                        case Capture.Entity:
                            IEntity entity = Activator.CreateInstance(attribute.component) as IEntity;
                            entity.Attach(GetTransform(name).gameObject);
                            field.SetValue(instance, entity);
                            break;
                    }
                }
            }
        }

        public Transform GetTransform(string name)
        {
            return _cacheDic[name];
        }

        public T CaptureComponent<T>(string name)
        {
            return GetTransform(name).GetComponent<T>();
        }

        public GameObject CaptureGameObject(string name)
        {
            return GetTransform(name).gameObject;
        }

        public T[] CaptureChildrenComponents<T>(string name)
        {
            return GetTransform(name).GetComponentsInChildren<T>();
        }
    }
}
