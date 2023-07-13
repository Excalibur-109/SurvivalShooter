using System.Collections;
using System.Collections.Generic;

namespace Excalibur.Algorithms.DataStructure
{
    public abstract class Node<T>
    {
        internal T item;
        public abstract T Value { get; set; }
    }

    public abstract class TreeNode<T> : Node<T>, IDegree
    {
        public abstract bool IsLeaf { get; }
        public abstract int Degree { get; }
    }

    public interface IDegree
    {
        int Degree { get; }
    }

    public interface ITree<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection<T>, IDegree
    {
        /// <summary> 广度优先遍历/层序遍历 /// </summary>
        IEnumerable<T> BreadthFirstTraversal();
    }

    public interface IBinaryTree<T> : ITree<T>
    {
        /// <summary> 前序遍历 /// </summary>
        IEnumerable<T> PreOrderTraversal();
        /// <summary> 中序遍历 /// </summary>
        IEnumerable<T> InOrderTraversal();
        /// <summary> 后序遍历 /// </summary>
        IEnumerable<T> PostOrderTraversal();
    }
}
