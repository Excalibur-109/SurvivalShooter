using System;
using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    /// <summary> implemented by DimensionalTree<T> /// </summary>
    [Obsolete("unfinished", true)]
    public sealed class BinaryTreeObsolete<T> : IBinaryTree<T>
    {
        const int DIMENSION = 2;

        internal DimensionalTree<T> tree;

        public int Count
        {
            get { return tree.count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Degree => tree.Degree;

        #region Constructors

        public BinaryTreeObsolete()
        {
            tree = new DimensionalTree<T>(DIMENSION);
        }

        public BinaryTreeObsolete(EqualityComparer<T> comparer)
        {
            tree = new DimensionalTree<T>(comparer, DIMENSION);
        }

        public BinaryTreeObsolete(IEnumerable<T> collection, EqualityComparer<T> comparer) : this(comparer)
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

        public DimensionalTreeNode<T> Append(T parent, T value)
        {
            return tree.Append(parent, value);
        }

        public DimensionalTreeNode<T> Append(DimensionalTreeNode<T> parent, T value)
        {
            return tree.Append(parent, value);
        }

        public void Append(DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
        {
            tree.Append(parent, node);
        }

        public void Add(T item)
        {
            tree.Add(item);
        }

        public void Clear()
        {
            tree.Clear();
        }

        public bool Contains(T item)
        {
            return tree.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            tree.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return tree.Remove(item);
        }

        public bool Remove(T parent, T item)
        {
            return tree.Remove(parent, item);
        }

        public bool Remove(DimensionalTreeNode<T> parent, T item)
        {
            return tree.Remove(parent, item);
        }

        public bool Remove(DimensionalTreeNode<T> parent, DimensionalTreeNode<T> node)
        {
            return tree.Remove(parent, node);
        }

        public DimensionalTreeNode<T> Find(T value)
        {
            return Find(value);
        }

        public DimensionalTreeNode<T> Find(T parent, T value)
        {
            return Find(parent, value);
        }

        public DimensionalTreeNode<T> Find(DimensionalTreeNode<T> node, T value)
        {
            return Find(node, value);
        }

        public IEnumerable<T> BreadthFirstTraversal()
        {
            return tree.BreadthFirstTraversal();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return tree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> PreOrderTraversal()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> InOrderTraversal()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> PostOrderTraversal()
        {
            throw new NotImplementedException();
        }
    }
}