using System;
using System.Collections.Generic;
using Excalibur.Algorithms.DataStructure;
using UnityEngine;

namespace Excalibur
{
    public interface IObjectPool<T>
    {
        T Get();
        void Release (T instance);
        void Clear();
    }

    /// <summary>
    /// 对象池
    /// </summary>
    public sealed class ObjectPool<T> : IObjectPool<T> where T : new()
    {
        private readonly LinkedStack<T> r_linkedStackPool;
        private readonly Action<T> r_actionOnGet;
        private readonly Action<T> r_actionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return r_linkedStackPool.Count; } }

        public ObjectPool (Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            r_linkedStackPool = new LinkedStack<T>();
            r_actionOnGet = actionOnGet;
            r_actionOnRelease = actionOnRelease;
        }

        public T Get ()
        {
            T instance;
            if (r_linkedStackPool.Count == 0)
            {
                instance = new T();
                ++countAll;
            }
            else
            {
                instance = r_linkedStackPool.Pop();
            }
            r_actionOnGet?.Invoke(instance);
            return instance;
        }

        public void Release (T instance)
        {
            if (r_linkedStackPool.Count > 0 && ReferenceEquals(r_linkedStackPool.Peek(), instance))
                Debug.LogError("对象池中已存在该对象，勿重复释放");
            r_actionOnRelease?.Invoke(instance);
            r_linkedStackPool.Push(instance);
        }

        public void Clear ()
        {
            r_linkedStackPool.Clear();
        }
    }
}