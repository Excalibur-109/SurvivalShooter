using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur.BehaviourTree
{
    public enum NodeStatus { Success, Failure, Evaluating }

    public sealed class BehaviourTree : BehaviourTreeNode
    {
        public override NodeStatus Evaluate()
        {
            if (childCount == 0) { return NodeStatus.Success; }
            return currentChild.Evaluate();
        }
    }
}
