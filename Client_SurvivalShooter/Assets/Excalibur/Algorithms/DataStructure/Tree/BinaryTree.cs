using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    internal enum BinaryTreeTraversal
    {
        BreadthFirst,
        PreOrder,
        InOrder,
        PostOrder,
    }

    /// summary
    /// 有根二叉树，允许重复节点
    /// summary
    public class BinaryTree<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        internal BinaryTreeNode<T> root;
        internal int count;
        internal int version;

        private EqualityComparer<T> _c;

        #region properties

        public int Count
        {
            get { return count; }
        }

        public int Height
        {
            get 
            {
                return _CalcHeight(root);
            }
        }

        public bool IsReadOnly 
        {
            get { return false; }
        }

        #endregion

        #region constructors

        public BinaryTree()
        {
            _c = EqualityComparer<T>.Default;
        }

        public BinaryTree(EqualityComparer<T> comparer)
        {
            _c = comparer;
        }

        public BinaryTree(IEnumerable<T> collection, EqualityComparer<T> comparer = default) : this(comparer)
        {
            foreach(T t in collection)
            {
                Add(t);
            }
        }

        #endregion

        #region publics

        public BinaryTreeNode<T> Insert(T value)
        {
            BinaryTreeNode<T> node = _CreateNode(value);
            _Inserqnode(root, node);
            return node;
        }

        public BinaryTreeNode<T> Insert(T parent, T value)
        {
            BinaryTreeNode<T> pNode = _SerachNode(root, parent);
            if (pNode == null && root != null)
            {
                return null;
            }
            BinaryTreeNode<T> node = _CreateNode(value);
            _Inserqnode(pNode, node);
            return node;
        }

        public BinaryTreeNode<T> Insert(BinaryTreeNode<T> parent, T value)
        {
            _ValidateNode(parent);
            BinaryTreeNode<T> node = _CreateNode(value);
            _Inserqnode(parent, node);
            return node;
        }

        public void Insert(BinaryTreeNode<T> parent, BinaryTreeNode<T> node)
        {
            _ValidateNode(parent);
            _ValidateNewNode(node);
            _Inserqnode(parent, node);
        }

        public void Add(T value)
        {
            Insert(value);
        }

        public void Clear()
        {
            if (root == null) { return; }
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>();
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                foreach (BinaryTreeNode<T> node in queue)
                {
                    stack.Push(node);
                    if (node.Left != null)
                    {
                        queue.Enqueue(node.Left);
                    }
                    
                    if (node.Right != null)
                    {
                        queue.Enqueue(node.Right);
                    }
                }
            }
            while (stack.Count > 0)
            {
                BinaryTreeNode<T> node = stack.Pop();
                node.Invalidate();
            }
            root = null;
            count = 0;
            ++version;
        }

        public bool Contains(T value)
        {
            return _SerachNode(root, value) != null;
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

            int index = 0;
            while (index < count)
            {
                _Traverse(BinaryTreeTraversal.BreadthFirst, root, value =>
                {
                    array[arrayIndex++] = value;
                    ++index;
                });
            }
        }

        public bool Remove(T value)
        {
            if (root == null) return false;
            BinaryTreeNode<T> parent = _SearchParentNode(root, value);
            if (parent == null && parent != root)
            {
                return false;
            }
            BinaryTreeNode<T> node = _c.Equals(parent.Left.Value, value) ? parent.Left : parent.Right;
            if (node != null)
            {
                _RemoveNode(parent, node);
                return true;
            }
            return false;
        }

        [Obsolete("Time != O(n)", true)]
        public bool Remove(T parent, T value)
        {
            if (root == null) return false;
            BinaryTreeNode<T> pNode = _SerachNode(root, parent);
            if (pNode == null)
            {
                return false;
            }
            BinaryTreeNode<T> nodeP = _SearchParentNode(pNode, value);
            BinaryTreeNode<T> node = _SerachNode(pNode, value);
            if (node != null)
            {
                _RemoveNode(nodeP, node);
                return true;
            }
            return false;
        }

        [Obsolete("Time != O(n)", true)]
        public bool Remove(BinaryTreeNode<T> parent, T value)
        {
            if (root == null) return false;
            _ValidateNode(parent);
            BinaryTreeNode<T> nodeP = _SearchParentNode(parent, value);
            BinaryTreeNode<T> node = _SerachNode(parent, value);
            if (node != null)
            {
                _RemoveNode(nodeP, node);
                return true;
            }
            return false;
        }

        public BinaryTreeNode<T> Find(T value)
        {
            return _SerachNode(root, value);
        }

        [Obsolete("Time != O(n)", true)]
        public BinaryTreeNode<T> Find(T parent, T value)
        {
            BinaryTreeNode<T> pNode = Find(parent);
            if (pNode == null)
            {
                return null;
            }
            return _SerachNode(pNode, value);
        }

        [Obsolete("Time != O(n)", true)]
        public BinaryTreeNode<T> Find(BinaryTreeNode<T> parent, T value)
        {
            _ValidateNode(parent);
            return _SerachNode(parent, value);
        }

        #region traversals

        public void BreadthFirstTraversal(Action<T> onTraverse)
        {
            _Traverse(BinaryTreeTraversal.BreadthFirst, root, onTraverse);
        }
        
        public void PreOrderTraversal(Action<T> onTraverse)
        {
            _Traverse(BinaryTreeTraversal.PreOrder, root, onTraverse);
        }
        
        public void InOrderTraversal(Action<T> onTraverse)
        {
            _Traverse(BinaryTreeTraversal.InOrder, root, onTraverse);
        }
        
        public void PostOrderTraversal(Action<T> onTraverse)
        {
            _Traverse(BinaryTreeTraversal.PostOrder, root, onTraverse);
        }

        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private BinaryTree<T> _t;
            private T _c;
            private int _version;
            private Queue<BinaryTreeNode<T>> _q;

            internal Enumerator(BinaryTree<T> tree)
            {
                _t = tree;
                _c = default(T);
                _version = tree.version;
                _q = new Queue<BinaryTreeNode<T>>();
                if (_t.root != null)
                {
                    _q.Enqueue(_t.root);
                }
            }

            public T Current => _c;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _t = null;
                _q = null;
                _c = default(T);
            }

            public bool MoveNext()
            {
                if (_version != _t.version)
                {
                    throw new InvalidOperationException();
                }
                
                if (_q.Count > 0)
                {
                    BinaryTreeNode<T> n = _q.Dequeue();
                    _c = n.Value;
                    _q.Enqueue(n.Left);
                    _q.Enqueue(n.Right);
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _q.Clear();
                _c = default(T);
                if (_t.root != null)
                {
                    _q.Enqueue(_t.root);
                }
            }
        }

        #endregion

        #region privates

        private void _Inserqnode(BinaryTreeNode<T> p, BinaryTreeNode<T> n)
        {
            if (p == null)
            {
                p = n;
            }
            if (root == null)
            {
                root = p;
            }
            else
            {
                Queue<BinaryTreeNode<T>> q = new Queue<BinaryTreeNode<T>>();
                q.Enqueue(p);
                while (q.Count > 0)
                {
                    BinaryTreeNode<T> qn = q.Dequeue();
                    if (qn.Left == null)
                    {
                        qn.Left = n;
                        break;
                    }
                    else if (qn.Right == null)
                    {
                        qn.Right = n;
                        break;
                    }
                    q.Enqueue(qn.Left);
                    q.Enqueue(qn.Right);
                }
            }
            ++count;
            ++version;
        }

        private void _RemoveNode(BinaryTreeNode<T> p, BinaryTreeNode<T> n)
        {
            if (p.Left == n)
            {
                switch (n.Left)
                {
                    case null when n.Right == null:
                    {
                        p.Left = null;
                        break;
                    }
                    case null when n.Right != null:
                    {
                        p.Left = n.Right;
                        break;
                    }
                    default:
                    {
                        if (n.Right == null)
                        {
                            p.Left = n.Left;
                        }
                        else
                        {
                            throw new InvalidOperationException("不能明确移除有两个孩子的节点");
                        }
                        break;
                    }
                }
            }
            else if (p.Right == n)
            {
                switch (n.Left)
                {
                    case null when n.Right == null:
                    {
                        p.Right = null;
                        break;
                    }
                    case null when n.Right != null:
                    {
                        p.Right = n.Right;
                        break;
                    }
                    default:
                    {
                        if (n.Right == null)
                        {
                            p.Right = n.Left;
                        }
                        else
                        {
                            throw new InvalidOperationException("不能明确移除有两个孩子的节点");
                        }
                        break;
                    }
                }
            }
            n.Invalidate();
            --count;
            ++version;
        }

        private int _CalcHeight(BinaryTreeNode<T> n)
        {
            if (n == null)
            {
                return -1;
            }
            return Math.Max(_CalcHeight(n.Left), _CalcHeight(n.Right)) + 1;
        }

        private BinaryTreeNode<T> _SerachNode(BinaryTreeNode<T> n, T t)
        {
            if (n == null) { return null; }
            Queue<BinaryTreeNode<T>> q = new Queue<BinaryTreeNode<T>>();
            q.Enqueue(n);
            while (q.Count > 0)
            {
                BinaryTreeNode<T> qn = q.Dequeue();
                if (_c.Equals(qn.Value, t))
                {
                    return qn;
                }

                if (qn.Left != null)
                {
                    q.Enqueue(qn.Left);
                }
                
                if (qn.Right != null)
                {
                    q.Enqueue(qn.Right);
                }
            }
            return null;
        }

        private BinaryTreeNode<T> _SearchParentNode(BinaryTreeNode<T> n, T t)
        {
            if (n == null) { return null; }
            Queue<BinaryTreeNode<T>> q = new Queue<BinaryTreeNode<T>>();
            q.Enqueue(n);
            while (q.Count > 0)
            {
                BinaryTreeNode<T> qn = q.Dequeue();
                if (qn.Left != null)
                {
                    if (_c.Equals(qn.Left.Value, t))
                    {
                        return qn;
                    }
                    else
                    {
                        q.Enqueue(qn.Left);
                    }
                }
                if (qn.Right != null)
                {
                    if (_c.Equals(qn.Right.Value, t))
                    {
                        return qn;
                    }
                    else
                    {
                        q.Enqueue(qn.Right);
                    }
                }
            }
            return null;
        }

        private BinaryTreeNode<T> _CreateNode(T t)
        {
            BinaryTreeNode<T> node = new BinaryTreeNode<T>();
            node.tree = this;
            node.Value = t;
            return node;
        }

        #endregion

        private void _Traverse(BinaryTreeTraversal t, BinaryTreeNode<T> n, Action<T> a)
        {
            if (n == null) { return; }
            switch (t)
            {
                case BinaryTreeTraversal.BreadthFirst:
                {
                    Queue<BinaryTreeNode<T>> q = new Queue<BinaryTreeNode<T>>();
                    q.Enqueue(n);
                    while (q.Count > 0)
                    {
                        BinaryTreeNode<T> qn = q.Dequeue();
                        a?.Invoke(qn.Value);
                        if (qn.Left != null)
                        {
                            q.Enqueue(qn.Left);
                        }
                        
                        if (qn.Right != null)
                        {
                            q.Enqueue(qn.Right);
                        }
                    }
                    break;
                }
                case BinaryTreeTraversal.PreOrder:
                {
                    a?.Invoke(n.Value);
                    _Traverse(t, n.Left, a);
                    _Traverse(t, n.Right, a);
                    break;
                }
                case BinaryTreeTraversal.InOrder:
                {
                    _Traverse(t, n.Left, a);
                    a?.Invoke(n.Value);
                    _Traverse(t, n.Right, a);
                    break;
                }
                case BinaryTreeTraversal.PostOrder:
                {
                    _Traverse(t, n.Left, a);
                    _Traverse(t, n.Right, a);
                    a?.Invoke(n.Value);
                    break;
                }
            }
        }

        #region validates

        private void _ValidateNewNode(BinaryTreeNode<T> n)
        {
            if (n.tree != null || n.Left != null || n.Right != null || n.Value == null)
            {
                throw new InvalidOperationException("节点不是一个新节点");
            }
        }

        private void _ValidateNode(BinaryTreeNode<T> n)
        {
            if (n == null)
            {
                throw new ArgumentNullException();
            }

            if (n.tree == null)
            {
                throw new ArgumentNullException("该节点不属于任何一棵树");
            }

            if (n.tree != this)
            {
                throw new InvalidOperationException("节点不是这棵树的节点");
            }
        }

        #endregion
    }

    public class BinaryTreeNode<T>
    {
        internal BinaryTree<T> tree;
        public T Value { get; set; }
        public BinaryTreeNode<T> Left { get; set; }
        public BinaryTreeNode<T> Right { get; set; }

        internal void Invalidate()
        {
            tree = null;
            Value = default(T);
            Left = null;
            Right = null;
        }
    }
}