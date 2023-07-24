using UnityEngine;
using System.Collections.Generic;
using System;

namespace Excalibur
{
    public static class MonoExtension
    {
        public static Transform FindRecusive (this Transform transform, string name)
        {
            Transform ret;
            ret = transform.Find (name);
            if (ret != null) { return ret; }
            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    ret = transform.GetChild (i).FindRecusive (name);
                    if (ret != null) { return ret; }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取该Transform下的所有transform
        /// </summary>
        public static List<Transform> AttachChilds (this Transform transform, Func<string, bool> cacheCondition = default)
        {
            List<Transform> list = new List<Transform> ();
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild (i);
                if (cacheCondition != null)
                {
                    if (cacheCondition(child.name))
                    {
                        list.Add(child);
                    }
                }
                else
                {
                    list.Add(child);
                }
                list.AddRange (child.AttachChilds (cacheCondition));
            }

            return list;
        }

        public static GameObject InitializeObject(GameObject src, Transform parent)
        {
            GameObject go = InitializeObject(src);
            go.transform.SetParent(parent);
            return go;
        }

        public static GameObject InitializeObject(GameObject src, string name = default)
        {
            GameObject go = UnityEngine.Object.Instantiate(src);
            go.name = string.IsNullOrEmpty(name) ? src.name : name;
            return go;
        }

        public static Vector2 WorldPos2ScreenPos(Vector3 worldPos)
        {
            return (Vector2)Camera.main.WorldToScreenPoint(worldPos);
        }
    }
}