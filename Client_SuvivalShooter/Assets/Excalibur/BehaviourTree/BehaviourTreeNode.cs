using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur.BehaviourTree
{
    public abstract class BehaviourTreeNodeBase
    {
        public abstract NodeStatus Evaluate();
        public abstract void Reset();
    }

    public abstract class BehaviourTreeNode : BehaviourTreeNodeBase
    {
        private List<BehaviourTreeNodeBase> children = new List<BehaviourTreeNodeBase>();
        protected int current;

        protected BehaviourTreeNodeBase currentChild
        {
            get { return children[current]; }
        }

        protected int childCount
        {
            get { return children.Count; }
        }

        public void Append (BehaviourTreeNodeBase child)
        {
            children.Add(child);
        }

        public override void Reset()
        {
            current = 0;
            for (int i = 0; i < childCount; ++i)
            {
                children[i].Reset();
            }
        }
    }
}
