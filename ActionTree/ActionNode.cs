using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI
{
    public class ActionNode : MonoBehaviour
    {
        public string Name;

        public TreeNode<ActionNode> Node;

        public ICondition Precondition;

        public Action Action;

        public List<IEffect> Effects = new List<IEffect>();

        public ActionNode[] Children = Array.Empty<ActionNode>();
    }
}
