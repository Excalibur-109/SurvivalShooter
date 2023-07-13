using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
