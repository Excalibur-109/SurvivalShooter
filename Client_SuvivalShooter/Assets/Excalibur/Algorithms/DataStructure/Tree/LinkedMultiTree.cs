using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    public sealed class LinkedMultiTree<T> : ITree<T>
    {
        /// <summary> 根节点保持类型T的默认值 /// </summary>
        internal LinkedMultiTreeNode<T> root;
        /// <summary> 节点数量，不包含root /// </summary>
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

        public int Degree => throw new NotImplementedException();

        #region Constructors

        public LinkedMultiTree ()
        {
            root = new LinkedMultiTreeNode<T>(this, default(T));
        }

        public LinkedMultiTree (EqualityComparer<T> comparer) : this()
        {
            if (comparer != null)
            {
                _c = comparer;
            }
        }

        public LinkedMultiTree (IEnumerable<T> collection, EqualityComparer<T> comparer) : this(comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (T item in collection)
            {
                Add(item);
            }
        }

        #endregion

        /// <summary> AppendRoot /// </summary>
        public LinkedMultiTreeNode<T> Append (T value)
        {
            LinkedMultiTreeNode<T> newNode = new LinkedMultiTreeNode<T>(this, value);
            _InsertNode(root, newNode);
            return newNode;
        }

        /// <summary> AppendRoot /// </summary>
        public void Append (LinkedMultiTreeNode<T> newNode)
        {
            ValidateNewNode(newNode);
            _InsertNode(root, newNode);
        }

        public LinkedMultiTreeNode<T> Append (T parent, T value)
        {
            LinkedMultiTreeNode<T> parentNode = Find(parent);
            ValidateNode(parentNode);
            LinkedMultiTreeNode<T> newNode = new LinkedMultiTreeNode<T>(this, value);
            _InsertNode(parentNode, newNode);
            return newNode;
        }

        public LinkedMultiTreeNode<T> Append (LinkedMultiTreeNode<T> parent, T value)
        {
            LinkedMultiTreeNode<T> newNode = new LinkedMultiTreeNode<T>(this, value);
            _InsertNode(parent, newNode);
            return newNode;
        }

        public void Append (LinkedMultiTreeNode<T> parent, LinkedMultiTreeNode<T> node)
        {
            ValidateNode(parent);
            ValidateNewNode(node);
            _InsertNode(parent, node);
            node.tree = this;
        }

        public void Add (T item)
        {
            Append(root, item);
        }

        public void Clear()
        {
            Queue<LinkedMultiTreeNode<T>> iteratorQueue = new Queue<LinkedMultiTreeNode<T>>();
            Stack<LinkedMultiTreeNode<T>> iteratorStack = new Stack<LinkedMultiTreeNode<T>>();
            iteratorQueue.Enqueue(root);
            while (iteratorQueue.Count > 0)
            {
                foreach (LinkedMultiTreeNode<T> child in iteratorQueue.Dequeue().children)
                {
                    iteratorStack.Push(child);
                }
            }
            while (iteratorStack.Count > 0)
            {
                LinkedMultiTreeNode<T> node = iteratorStack.Pop();
                node.Invalidate();
            }
            root.children = null;
            count = 0;
            ++version;
        }

        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
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

            List<T> enumerable = BreadthFirstTraversal() as List<T>;
            foreach (T value in enumerable)
            {
                array[arrayIndex++] = value;
            }
        }

        #region Remove

        public bool Remove(T item)
        {
            LinkedMultiTreeNode<T> node = Find(item);
            if (node != null)
            {
                _RemoveNode(root, node);
                return true;
            }
            return false;
        }

        public bool Remove(T parent, T item)
        {
            LinkedMultiTreeNode<T> parentNode = Find(parent);
            LinkedMultiTreeNode<T> node = Find(item);
            if (parentNode != null && node != null)
            {
                _RemoveNode(parentNode, node);
                return true;
            }
            return false;
        }

        public bool Remove(LinkedMultiTreeNode<T> parent, T item)
        {
            ValidateNode(parent);
            LinkedMultiTreeNode<T> node = Find(parent, item);
            if (node != null)
            {
                _RemoveNode(parent, node);
                return true;
            }
            return false;
        }

        public bool Remove(LinkedMultiTreeNode<T> parent, LinkedMultiTreeNode<T> node)
        {
            ValidateNode(parent);
            ValidateNode(node);
            LinkedMultiTreeNode<T> current = node.parent;
            do
            {
                if (current == parent) 
                {
                    _RemoveNode(parent, node);
                    return true;
                }
                current = current.parent;
            }
            while (current != root);
            return false;
        }

        #endregion

        #region Find

        public LinkedMultiTreeNode<T> Find (T value)
        {
            return Find(root, value);
        }

        public LinkedMultiTreeNode<T> Find (T parent, T value)
        {
            LinkedMultiTreeNode<T> parentNode = Find(parent);
            return parent == null ? null : Find(parentNode, value);
        }

        public LinkedMultiTreeNode<T> Find (LinkedMultiTreeNode<T> node, T value)
        {
            if (node != null)
            {
                if (node != root)
                {
                    if (value != null)
                    {
                        if (_c.Equals(node.item, value))
                        {
                            return node;
                        }
                    }
                    else
                    {
                        if (node.item == null)
                        {
                            return node;
                        }
                    }
                }

                foreach (LinkedMultiTreeNode<T> child in node.children)
                {
                    LinkedMultiTreeNode<T> result = Find(child, value);
                    if (result != null) { return result; }
                }
            }

            return null;
        }

        #endregion

        private void _InsertNode (LinkedMultiTreeNode<T> parent, LinkedMultiTreeNode<T> node)
        {
            parent.children.AddLast(node);
            node.parent = parent;
            ++count;
            ++version;
        }

        private void _RemoveNode (LinkedMultiTreeNode<T> parent, LinkedMultiTreeNode<T> node)
        {
            parent.children.Remove(node);
            node.Invalidate();
            --count;
            ++version;
        }

        #region Validate

        internal void ValidateNode (LinkedMultiTreeNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.tree != this)
            {
                throw new InvalidOperationException("node is not belong to this tree.");
            }
        }

        internal void ValidateNewNode(LinkedMultiTreeNode<T> newNode)
        {
            if (newNode == null)
            {
                throw new ArgumentNullException("node");
            }

            if (newNode.tree != null)
            {
                throw new InvalidOperationException("node already has tree.");
            }
        }

        #endregion

        #region Traversal

        public IEnumerable<T> BreadthFirstTraversal()
        {
            Queue<LinkedMultiTreeNode<T>> iterator = new Queue<LinkedMultiTreeNode<T>>();
            iterator.Enqueue(root);
            List<T> enumeralbe = new List<T>();
            while (iterator.Count > 0)
            {
                foreach (LinkedMultiTreeNode<T> child in iterator.Dequeue().children)
                {
                    enumeralbe.Add(child.item);
                    iterator.Enqueue(child);
                }
            }
            return enumeralbe;
        }

        #endregion

        #region Enumerator

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
            internal LinkedMultiTree<T> tree;
            internal IEnumerator<T> enumerator;
            internal uint version;

            internal Enumerator (LinkedMultiTree<T> tree)
            {
                this.tree = tree;
                enumerator = tree.BreadthFirstTraversal().GetEnumerator();
                version = tree.version;
            }

            public T Current
            {
                get { return enumerator.Current; }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                tree = null;
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (version != tree.version)
                {
                    throw new InvalidOperationException();
                }
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        #endregion
    }

    /// <summary> implemented by System.Collections.Generic.LinkedList<T> /// </summary>
    public class LinkedMultiTreeNode<T> : TreeNode<T>
    {
        internal LinkedMultiTree<T> tree;
        internal LinkedMultiTreeNode<T> parent;
        internal LinkedList<LinkedMultiTreeNode<T>> children;

        public override T Value
        {
            get { return item; }
            set { item = value; }
        }

        public override int Degree
        {
            get { return children.Count; }
        }

        public override bool IsLeaf
        {
            get { return children.Count == 0; }
        }

        public LinkedMultiTreeNode(T value) 
        {
            item = value;
            children = new LinkedList<LinkedMultiTreeNode<T>>();
        }

        internal LinkedMultiTreeNode(LinkedMultiTree<T> tree, T value) : this(value)
        {
            this.tree = tree;
            children = new LinkedList<LinkedMultiTreeNode<T>>();
        }

        public LinkedMultiTree<T> Tree
        {
            get { return tree; }
        }

        public LinkedMultiTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public LinkedList<LinkedMultiTreeNode<T>> Children
        {
            get { return children; }
            set { children = value; }
        }

        internal void Invalidate()
        {
            tree = null;
            parent = null;
            children = null;
        }
    }

}