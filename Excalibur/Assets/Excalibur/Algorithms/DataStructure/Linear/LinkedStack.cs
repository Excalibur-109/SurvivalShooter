using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    public sealed class LinkedStack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        private readonly LinkedList<T> _linkedList;

        public LinkedStack ()
        {
            _linkedList = new LinkedList<T> ();
        }
        public LinkedStack (IEnumerable<T> collection)
        {
            _linkedList = new LinkedList<T> (collection);
        }

        public int Count { get { return _linkedList.Count; } }

        public void Clear ()
        {
            _linkedList.Clear ();
        }

        public bool Contains (T obj)
        {
            return _linkedList.Contains (obj);
        }

        public void CopyTo (T[] array, int index)
        {
            _linkedList.CopyTo (array, index);
        }

        public IEnumerator GetEnumerator ()
        {
            return _linkedList.GetEnumerator ();
        }

        public T Peek ()
        {
            return _linkedList.First.Value;
        }

        public T Pop ()
        {
            T obj = _linkedList.First.Value;
            _linkedList.RemoveFirst ();
            return obj;
        }

        public void Push (T obj)
        {
            _linkedList.AddFirst (obj);
        }

        public T[] ToArray ()
        {
            T[] array = new T[Count];
            int i = 0;
            foreach  (var item in _linkedList)
            {
                array[i++] = item;
            }
            return array;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return _linkedList.GetEnumerator ();
        }
    }
}
