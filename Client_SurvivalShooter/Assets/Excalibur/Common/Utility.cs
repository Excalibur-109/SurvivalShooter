using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Excalibur
{
    public static partial class Utility
    {
        public static void PrintDictionary<TKey, TValue>(string keyWord, Dictionary<TKey, TValue> dic)
        {
            if (dic.Count == 0)
            {
                UnityEngine.Debug.Log("Empty Dictionary.");
            }
            foreach (var item in dic)
            {
                UnityEngine.Debug.LogFormat("{0}: Key-{1}, Value-{2}", keyWord, item.Key, item.Value);
            }
        }

        public static Vector3 Float3ArrayToVector3(this float[] array)
        {
            if (array == null) { return Vector3.zero; }
            Vector3 ret = Vector3.zero;
            int i = 0;
            while (i < 3)
            {
                ret[i++] = i < array.Length ? array[i] : ret[i];
            }
            return ret;
        }
    }
}
