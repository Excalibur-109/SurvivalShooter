using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    public sealed class MultiTree<T> : ITree<T>
    {
        internal MultiTreeNode<T> root;
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

        public MultiTree ()
        {
            root = new MultiTreeNode<T>(this, default(T));
        }

        public MultiTree (EqualityComparer<T> comparer) : this()
        {
            if (comparer != null)
            {
                _c = comparer;
            }
        }

        public MultiTree (IEnumerable<T> collection, EqualityComparer<T> comparer) : this(comparer)
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
        public MultiTreeNode<T> Append (T value)
        {
            MultiTreeNode<T> newNode = new MultiTreeNode<T>(this, value);
            _InsertNode(root, newNode);
            return newNode;
        }

        /// <summary> AppendRoot /// </summary>
        public void Append (MultiTreeNode<T> newNode)
        {
            ValidateNewNode(newNode);
            _InsertNode(root, newNode);
        }

        public MultiTreeNode<T> Append (T parent, T value)
        {
            MultiTreeNode<T> parentNode = Find(parent);
            ValidateNode(parentNode);
            MultiTreeNode<T> newNode = new MultiTreeNode<T>(this, value);
            _InsertNode(parentNode, newNode);
            return newNode;
        }

        public MultiTreeNode<T> Append (MultiTreeNode<T> parent, T value)
        {
            ValidateNode(parent);
            MultiTreeNode<T> newNode = new MultiTreeNode<T>(this, value);
            _InsertNode(parent, newNode);
            return newNode;
        }

        public void Append (MultiTreeNode<T> parent, MultiTreeNode<T> node)
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
            Queue<MultiTreeNode<T>> iteratorQueue = new Queue<MultiTreeNode<T>>();
            Stack<MultiTreeNode<T>> iteratorStack = new Stack<MultiTreeNode<T>>();
            iteratorQueue.Enqueue(root);
            while (iteratorQueue.Count > 0)
            {
                foreach (MultiTreeNode<T> child in iteratorQueue.Dequeue().children)
                {
                    iteratorStack.Push(child);
                }
            }
            while (iteratorStack.Count > 0)
            {
                MultiTreeNode<T> node = iteratorStack.Pop();
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
            MultiTreeNode<T> node = Find(item);
            if (node != null)
            {
                _RemoveNode(root, node);
                return true;
            }
            return false;
        }

        public bool Remove(T parent, T item)
        {
            MultiTreeNode<T> parentNode = Find(parent);
            MultiTreeNode<T> node = Find(item);
            if (parentNode != null && node != null)
            {
                _RemoveNode(parentNode, node);
                return true;
            }
            return false;
        }

        public bool Remove(MultiTreeNode<T> parent, T item)
        {
            ValidateNode(parent);
            MultiTreeNode<T> node = Find(parent, item);
            if (node != null)
            {
                _RemoveNode(parent, node);
                return true;
            }
            return false;
        }

        public bool Remove(MultiTreeNode<T> parent, MultiTreeNode<T> node)
        {
            ValidateNode(parent);
            ValidateNode(node);
            MultiTreeNode<T> current = node.parent;
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

        public MultiTreeNode<T> Find (T value)
        {
            return Find(root, value);
        }

        public MultiTreeNode<T> Find (T parent, T value)
        {
            MultiTreeNode<T> parentNode = Find(parent);
            return parent == null ? null : Find(parentNode, value);
        }

        public MultiTreeNode<T> Find (MultiTreeNode<T> node, T value)
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

                foreach (MultiTreeNode<T> child in node.children)
                {
                    MultiTreeNode<T> result = Find(child, value);
                    if (result != null) { return result; }
                }
            }

            return null;
        }

        #endregion

        private void _InsertNode (MultiTreeNode<T> parent, MultiTreeNode<T> node)
        {
            parent.children.Add(node);
            node.parent = parent;
            ++count;
            ++version;
        }

        private void _RemoveNode (MultiTreeNode<T> parent, MultiTreeNode<T> node)
        {
            parent.children.Remove(node);
            node.Invalidate();
            --count;
            ++version;
        }

        #region Validate

        internal void ValidateNode (MultiTreeNode<T> node)
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

        internal void ValidateNewNode(MultiTreeNode<T> newNode)
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
            Queue<MultiTreeNode<T>> iterator = new Queue<MultiTreeNode<T>>();
            iterator.Enqueue(root);
            List<T> enumeralbe = new List<T>();
            while (iterator.Count > 0)
            {
                foreach (MultiTreeNode<T> child in iterator.Dequeue().children)
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
            internal MultiTree<T> tree;
            internal IEnumerator<T> enumerator;
            internal uint version;

            internal Enumerator (MultiTree<T> tree)
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

    /// <summary> implemented by System.Collections.Generic.List<T> /// </summary>
    public class MultiTreeNode<T> : TreeNode<T>
    {
        internal MultiTree<T> tree;
        internal MultiTreeNode<T> parent;
        internal List<MultiTreeNode<T>> children;

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

        public MultiTreeNode(T value) 
        {
            item = value;
            children = new List<MultiTreeNode<T>>();
        }

        internal MultiTreeNode(MultiTree<T> tree, T value) : this(value)
        {
            this.tree = tree;
            children = new List<MultiTreeNode<T>>();
        }

        public MultiTree<T> Tree
        {
            get { return tree; }
        }

        public MultiTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public List<MultiTreeNode<T>> Children
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