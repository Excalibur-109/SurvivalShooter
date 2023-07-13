using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur.BehaviourTree
{
    public class SelectNode : BehaviourTreeNode
    {
        public override NodeStatus Evaluate()
        {
            NodeStatus status = currentChild.Evaluate();
            switch (status)
            {
                case NodeStatus.Success:
                    {
                        current = 0;
                        return NodeStatus.Success;
                    }
                case NodeStatus.Failure:
                    {
                        if (++current == childCount)
                        {
                            current = 0;
                            return NodeStatus.Failure;
                        }
                        break;
                    }
            }
            return NodeStatus.Evaluating;
        }
    }

    public class SequenceNode : BehaviourTreeNode
    {
        public override NodeStatus Evaluate()
        {
            NodeStatus status = currentChild.Evaluate();
            switch (status)
            {
                case NodeStatus.Success:
                    {
                        if (++current == childCount)
                        {
                            current = 0;
                            return NodeStatus.Success;
                        }
                        break;
                    }
                case NodeStatus.Failure:
                    {
                        Reset();
                        return NodeStatus.Failure;
                    }
            }
            return NodeStatus.Evaluating;
        }
    }
}
