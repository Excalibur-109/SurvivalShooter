using System;
using System.Collections.Generic;
using Excalibur.Algorithms;
using UnityEngine;

namespace Excalibur
{
    public static class CustomPool
    {
        public static class ListPool<T>
        {
            private static readonly ObjectPool<List<T>> s_listPool = new ObjectPool<List<T>>(null, Clear);
            static void Clear(List<T> l) { l.Clear(); }

            public static List<T> Get()
            {
                return s_listPool.Get();
            }

            public static void Release(List<T> toRelease)
            {
                s_listPool.Release(toRelease);
            }

            public static void Clear()
            {
                s_listPool.Clear();
            }
        }
    }
}