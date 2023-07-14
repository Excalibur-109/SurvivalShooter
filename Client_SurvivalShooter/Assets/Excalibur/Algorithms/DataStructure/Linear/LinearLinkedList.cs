using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    public sealed class LinearLinkedList<T> : IEnumerable<T>, IEnumerable, ICollection<T>, IReadOnlyCollection<T>
    {
        internal LinearLinkedListNode<T> head;
        internal int count;
        internal uint version;

        private readonly EqualityComparer<T> _c = EqualityComparer<T>.Default;

        public int Count
        {
            get { return count; }
        }

        public bool IsReadOnly 
        {
            get { return false; } 
        }

        public LinearLinkedListNode<T> First
        {
            get { return head; }
        }

        public LinearLinkedListNode<T> Last
        {
            get { return head == null ? null : head.prev; }
        }

        public LinearLinkedList () { }

        public LinearLinkedList (EqualityComparer<T> comparer)
        {
            _c = comparer;
        }

        public LinearLinkedList (EqualityComparer<T> comparer, IEnumerable<T> collection) : this(comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

        public LinearLinkedListNode<T> AddAfter(LinearLinkedListNode<T> node, T value)
        {
            _ValidateNode(node);
            LinearLinkedListNode<T> newNode = new LinearLinkedListNode<T>(this, value);
            _InsertNodeBefore(node.next, newNode);
            return newNode;
        }

        public void AddAfter(LinearLinkedListNode<T> node, LinearLinkedListNode<T> newNode)
        {
            _ValidateNode(node);
            _ValidateNewNode(newNode);
            _InsertNodeBefore(node.next, newNode);
            newNode.list = this;
        }

        public LinearLinkedListNode<T> AddBefore (LinearLinkedListNode<T> node, T value)
        {
            _ValidateNode(node);
            LinearLinkedListNode<T> newNode = new LinearLinkedListNode<T>(this, value);
            _InsertNodeBefore(node, newNode);
            if (node == head)
            {
                head = newNode;
            }
            return newNode;
        }

        public void AddBefore (LinearLinkedListNode<T> node, LinearLinkedListNode<T> newNode)
        {
            _ValidateNode(node);
            _ValidateNewNode(newNode);
            _InsertNodeBefore(node, newNode);
            newNode.list = this;
            if (node == head)
            {
                head = newNode;
            }
        }

        public LinearLinkedListNode<T> AddFirst (T value)
        {
            LinearLinkedListNode<T> node = new LinearLinkedListNode<T>(this, value);
            if (head == null)
            {
                _InsertNodeToEmptyList(node);
            }
            else
            {
                _InsertNodeBefore(head, node);
                head = node;
            }

            return node;
        }

        public void AddFirst (LinearLinkedListNode<T> node)
        {
            _ValidateNewNode(node);
            if (head == null)
            {
                _InsertNodeToEmptyList(node);
            }
            else
            {
                _InsertNodeBefore(head, node);
                head = node;
            }
            node.list = this;
        }

        public LinearLinkedListNode<T> AddLast (T value)
        {
            LinearLinkedListNode<T> node = new LinearLinkedListNode<T>(this, value);
            if (head == null)
            {
                _InsertNodeToEmptyList(node);
            }
            else
            {
                _InsertNodeBefore(head, node);
            }

            return node;
        }

        public void AddLast (LinearLinkedListNode<T> node)
        {
            _ValidateNewNode(node);
            if (head == null)
            {
                _InsertNodeToEmptyList(node);
            }
            else
            {
                _InsertNodeBefore(head, node);
            }
            node.list = this;
        }

        public void Add (T item)
        {
            AddLast(item);
        }

        public void Clear() 
        {
            LinearLinkedListNode<T> current = head;
            while (current != null)
            {
                LinearLinkedListNode<T> temp = current;
                current = current.next;
                temp.Invalidate();
            }

            head = null;
            count = 0;
            ++version;
        }

        public bool Contains (T item)
        {
            return Find(item) != null;
        }

        public void CopyTo (T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", string.Format("{0} is out of range.", arrayIndex));
            }

            if (array.Length - arrayIndex < count)
            {
                throw new ArgumentException();
            }

            LinearLinkedListNode<T> node = head;
            if (node != null)
            {
                do
                {
                    array[arrayIndex++] = node.item;
                    node = node.next;
                }
                while (node != head);
            }
        }

        /// <summary> 从头到尾查找 /// </summary>
        public LinearLinkedListNode<T> Find (T value)
        {
            LinearLinkedListNode<T> node = head;
            if (node != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (_c.Equals(node.item, value))
                        {
                            return node;
                        }
                        node = node.next;
                    }
                    while (node != head);
                }
                else
                {
                    do
                    {
                        if (node.item == null)
                        {
                            return node;
                        }
                        node = node.next;
                    }
                    while (node != head);
                }
            }
            return null;
        }

        /// <summary> 从尾到头查找 /// </summary>
        public LinearLinkedListNode<T> FindLast (T value)
        {
            if (head == null) { return null; }

            LinearLinkedListNode<T> last = head.prev;
            LinearLinkedListNode<T> node = last;
            if (node != null)
            {
                if (value != null)
                {
                    do
                    {
                        if (_c.Equals(node.item, value))
                        {
                            return node;
                        }
                        node = node.prev;
                    }
                    while (node != last);
                }
                else
                {
                    do
                    {
                        if (node.item == null)
                        {
                            return node;
                        }
                        node = node.prev;
                    }
                    while (node != last);
                }
            }
            return null;
        }

        public bool Remove (T item)
        {
            LinearLinkedListNode<T> node = Find(item);
            if (node != null)
            {
                _RemoveNode(node);
                return true;
            }
            return false;
        }

        public void Remove (LinearLinkedListNode<T> node)
        {
            _ValidateNode(node);
            _RemoveNode(node);
        }

        public void RemoveFirst ()
        {
            if (head == null) { throw new InvalidOperationException("LinearLinkedList is empty."); }
            _RemoveNode(head);
        }

        public void RemoveLast ()
        {
            if (head == null) { throw new InvalidOperationException("LinearLinkedList is empty."); }
            _RemoveNode(head.prev);
        }

        private void _InsertNodeBefore (LinearLinkedListNode<T> node, LinearLinkedListNode<T> newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;
            ++count;
            ++version;
        }

        private void _InsertNodeToEmptyList (LinearLinkedListNode<T> newNode)
        {
            newNode.next = newNode;
            newNode.prev = newNode;
            head = newNode;
            ++count;
            ++version;
        }

        private void _RemoveNode(LinearLinkedListNode<T> node)
        {
            if (node.list != this || head == null) {  return; }
            if (node.next == node) 
            {
                head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                if (head == node)
                {
                    head = node.next;
                }
            }
            node.Invalidate();
            --count;
            ++version;
        }

        private void _ValidateNode (LinearLinkedListNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.list != this)
            {
                throw new InvalidOperationException("node is not belong to this list.");
            }
        }

        private void _ValidateNewNode (LinearLinkedListNode<T> newNode)
        {
            if (newNode == null)
            {
                throw new ArgumentNullException("newNode");
            }

            if (newNode.list != null)
            {
                throw new InvalidOperationException("newNode already has list.");
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private LinearLinkedList<T> list;
            private LinearLinkedListNode<T> node;
            private T current;
            private int index;
            private uint version;

            internal Enumerator (LinearLinkedList<T> list)
            {
                this.list = list;
                node = list.head;
                current = default(T);
                index = 0;
                version = list.version;
            }

            public T Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == list.count + 1))
                    {
                        throw new InvalidOperationException();
                    }
                    return current;
                }
            }

            public bool MoveNext()
            {
                if (version != list.version)
                {
                    throw new InvalidOperationException();
                }

                if (node == null)
                {
                    index = list.count + 1;
                    return false;
                }

                ++index;
                current = node.item;
                node = node.next;
                if (node == list.head)
                {
                    node = null;
                }
                return true;
            }

            public void Reset()
            {
                current = default(T);
                node = list.head;
                index = 0;
            }

            public void Dispose()
            {
            }
        }
    }

    public class LinearLinkedListNode<T> : Node<T>
    {
        internal LinearLinkedList<T> list;
        internal LinearLinkedListNode<T> next;
        internal LinearLinkedListNode<T> prev;

        public LinearLinkedListNode(T value)
        {
            item = value;
        }

        internal LinearLinkedListNode(LinearLinkedList<T> list, T value) : this(value)
        {
            this.list = list;
        }

        public override T Value
        {
            get { return item; }
            set { item = value; }
        }

        public virtual LinearLinkedList<T> List
        {
            get { return list; }
        }

        public virtual LinearLinkedListNode<T> Next
        {
            get { return next; }
            set { next = value; }
        }

        public virtual LinearLinkedListNode<T> Previous
        {
            get { return prev; }
            set { prev = value; }
        }

        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }
    }
}
