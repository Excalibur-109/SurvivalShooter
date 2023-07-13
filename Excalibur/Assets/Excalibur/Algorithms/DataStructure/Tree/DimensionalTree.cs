using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    [Obsolete("unfinished", true)]
    public sealed class DimensionalTree<T> : ITree<T>
    {
        internal DimensionalTreeNode<T> root;
        internal readonly int dimension = 2;
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

        public DimensionalTree(int dimension)
        {
            if (dimension < 1)
            {
                throw new InvalidOperationException("dimension required at least 1 dimension.");
            }
            this.dimension = dimension;
            root = new DimensionalTreeNode<T>(this, default(T));
        }

        public DimensionalTree (EqualityComparer<T> comparer, int dimension) : this(dimension)
        {
            if (comparer != null)
            {
                _c = comparer;
            }
        }

        public DimensionalTree (IEnumerable<T> collection, EqualityComparer<T> comparer, int dimension) : this(comparer, dimension)
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

        public DimensionalTreeNode<T> Append (T value)
        {
            DimensionalTreeNode<T> newNode = new DimensionalTreeNode<T>(this, value);
            _InsertNode(root, newNode);
            return newNode;
        }

        public DimensionalTreeNode<T> Append (DimensionalTreeNode<T> newNode)
        {
            ValidateNewNode(newNode);
            _InsertNode(root, newNode);
            return newNode;
        }

        public DimensionalTreeNode<T> Append (T parent, T value)
        {
            DimensionalTreeNode<T> parentNode = Find(parent);
            ValidateNode(parentNode);
            DimensionalTreeNode<T> newNode = new DimensionalTreeNode<T>(this, value);
            _InsertNode(parentNode, newNode);
            return newNode;
        }

        public DimensionalTreeNode<T> Append (DimensionalTreeNode<T> parent, T value)
        {
            DimensionalTreeNode<T> newNode = new DimensionalTreeNode<T>(this, value);
            _InsertNode(parent, newNode);
            return newNode;
        }

        public void Append (DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
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
            Queue<DimensionalTreeNode<T>> iteratorQueue = new Queue<DimensionalTreeNode<T>>();
            Stack<DimensionalTreeNode<T>> iteratorStack = new Stack<DimensionalTreeNode<T>>();
            iteratorQueue.Enqueue(root);
            while (iteratorQueue.Count > 0)
            {
                foreach (DimensionalTreeNode<T> child in iteratorQueue.Dequeue().children)
                {
                    iteratorStack.Push(child);
                }
            }
            while (iteratorStack.Count > 0)
            {
                DimensionalTreeNode<T> node = iteratorStack.Pop();
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
            DimensionalTreeNode<T> node = Find(item);
            if (node != null)
            {
                _RemoveNode(root, node);
                return true;
            }
            return false;
        }

        public bool Remove(T parent, T item)
        {
            DimensionalTreeNode<T> parentNode = Find(parent);
            DimensionalTreeNode<T> node = Find(item);
            if (parentNode != null && node != null)
            {
                _RemoveNode(parentNode, node);
                return true;
            }
            return false;
        }

        public bool Remove(DimensionalTreeNode<T> parent, T item)
        {
            ValidateNode(parent);
            DimensionalTreeNode<T> node = Find(parent, item);
            if (node != null)
            {
                _RemoveNode(parent, node);
                return true;
            }
            return false;
        }

        public bool Remove(DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
        {
            ValidateNode(parent);
            ValidateNode(node);
            DimensionalTreeNode<T> current = node.parent;
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

        public DimensionalTreeNode<T> Find (T value)
        {
            return Find(root, value);
        }

        public DimensionalTreeNode<T> Find (T parent, T value)
        {
            DimensionalTreeNode<T> parentNode = Find(parent);
            return parent == null ? null : Find(parentNode, value);
        }

        public DimensionalTreeNode<T> Find (DimensionalTreeNode<T> node, T value)
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

                foreach (DimensionalTreeNode<T> child in node.children)
                {
                    DimensionalTreeNode<T> result = Find(child, value);
                    if (result != null) { return result; }
                }
            }

            return null;
        }

        #endregion

        private void _InsertNode (DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
        {
            parent.children.Add(node);
            node.parent = parent;
            ++count;
            ++version;
        }

        private void _RemoveNode (DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
        {
            parent.children.Remove(node);
            node.Invalidate();
            --count;
            ++version;
        }

        #region Validate

        internal void ValidateNode (DimensionalTreeNode<T> node)
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

        internal void ValidateNewNode(DimensionalTreeNode<T> newNode)
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
            Queue<DimensionalTreeNode<T>> iterator = new Queue<DimensionalTreeNode<T>>();
            iterator.Enqueue(root);
            List<T> enumeralbe = new List<T>();
            while (iterator.Count > 0)
            {
                foreach (DimensionalTreeNode<T> child in iterator.Dequeue().children)
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
            internal DimensionalTree<T> tree;
            internal IEnumerator<T> enumerator;
            internal uint version;

            internal Enumerator (DimensionalTree<T> tree)
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
    [Obsolete("unfinished", true)]
    public class DimensionalTreeNode<T> : TreeNode<T>
    {
        internal DimensionalTree<T> tree;
        internal DimensionalTreeNode<T> parent;
        internal List<DimensionalTreeNode<T>> children;

        public DimensionalTreeNode(T value)
        {
            item = value;
            children = new List<DimensionalTreeNode<T>>();
        }

        internal DimensionalTreeNode(DimensionalTree<T> tree, T value) : this(value)
        {
            this.tree = tree;
            children = new List<DimensionalTreeNode<T>>();
        }

        public override T Value
        {
            get { return item; }
            set { item = value; }
        }

        internal int Dimension
        {
            get { return tree.dimension; }
        }

        public DimensionalTreeNode<T> this[int childIndex]
        {
            get { return children[childIndex]; }
            set { children[childIndex] = value; }
        }

        public override int Degree
        {
            get { return children.Count; }
        }

        public override bool IsLeaf
        {
            get { return children.Count == 0; }
        }

        public DimensionalTreeNode<T> LeftChild
        {
            get { return children[0]; }
        }

        public DimensionalTreeNode<T> RightChild
        {
            get { return children[children.Count - 1]; }
        }

        public DimensionalTree<T> Tree
        {
            get { return tree; }
        }

        public DimensionalTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public List<DimensionalTreeNode<T>> Children
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