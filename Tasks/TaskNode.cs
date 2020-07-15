using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

namespace AI
{
    public static class DefaultTaskNodes
    {
        public static SuccessTaskNode SuccessNode = new SuccessTaskNode();
        public static FailureTaskNode FailureNode = new FailureTaskNode();
    }

    public class SuccessTaskNode : ITaskNode
    {
        public TreeNode<ITaskNode> Node; // with this tree structure we can reuse other trees independently (no parent reference problems)

        public SuccessTaskNode()
        {
            Node = new TreeNode<ITaskNode>(this);
        }

        public void AddChildTask(ITaskNode child)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support children");
        }

        public void AddEffect(IEffect effect)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support effects");
        }

        public void ApplyEffects(int[] ws)
        {
            
        }

        public bool CheckConditions(int[] ws)
        {
            return true;
        }

        public ITaskNodeVisitor CreateVisitor()
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support visitor");
        }

        public ITaskNode GetChildTask(int i)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support children");
        }

        public ICondition GetCondition()
        {
            return null;
        }

        public IEffect GetEffect(int i)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support effects");
        }

        public ICondition GetMonitor()
        {
            return null;
        }

        public string GetName()
        {
            return "SuccessTaskNode";
        }

        public int GetNumEffects()
        {
            return 0;
        }

        public IOperator GetOperator()
        {
            return DefaultOperators.SuccessOperator;
        }

        public ITaskNode GetParentTask()
        {
            if (Node.m_Parent != null)
            {
                return Node.m_Parent.m_Data;
            }
            else
            {
                return null;
            }
        }

        public bool IsComposite()
        {
            return false;
        }

        public int NumChildrenTasks()
        {
            return 0;
        }

        public void RestoreEffects(int[] ws)
        {
            
        }

        public void SetCondition(ICondition condition)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support effects");
        }

        public void SetMonitor(ICondition condition)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support monitors");
        }

        public void SetName(string name)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support set name");
        }

        public void SetOperator(IOperator op)
        {
            throw new NotImplementedException("SuccessTaskNode doesn't support operator");
        }
    }

    public class FailureTaskNode : ITaskNode
    {
        public TreeNode<ITaskNode> Node; // with this tree structure we can reuse other trees independently (no parent reference problems)

        public FailureTaskNode()
        {
            Node = new TreeNode<ITaskNode>(this);
        }

        public void AddChildTask(ITaskNode child)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support children");
        }

        public void AddEffect(IEffect effect)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support effects");
        }

        public void ApplyEffects(int[] ws)
        {

        }

        public bool CheckConditions(int[] ws)
        {
            return true;
        }

        public ITaskNodeVisitor CreateVisitor()
        {
            throw new NotImplementedException("FailureTaskNode doesn't support visitor");
        }

        public ITaskNode GetChildTask(int i)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support children");
        }

        public ICondition GetCondition()
        {
            return null;
        }

        public IEffect GetEffect(int i)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support effects");
        }

        public ICondition GetMonitor()
        {
            return null;
        }

        public string GetName()
        {
            return "FailureTaskNode";
        }

        public int GetNumEffects()
        {
            return 0;
        }

        public IOperator GetOperator()
        {
            return DefaultOperators.FailedOperator;
        }

        public ITaskNode GetParentTask()
        {
            if (Node.m_Parent != null)
            {
                return Node.m_Parent.m_Data;
            }
            else
            {
                return null;
            }
        }

        public bool IsComposite()
        {
            return false;
        }

        public int NumChildrenTasks()
        {
            return 0;
        }

        public void RestoreEffects(int[] ws)
        {

        }

        public void SetCondition(ICondition condition)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support effects");
        }

        public void SetMonitor(ICondition condition)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support monitors");
        }

        public void SetName(string name)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support set name");
        }

        public void SetOperator(IOperator op)
        {
            throw new NotImplementedException("FailureTaskNode doesn't support operator");
        }
    }

    public class TaskNode : ITaskNode
    {
        public string Name;

        public TreeNode<ITaskNode> Node; // with this tree structure we can reuse other trees independently (no parent reference problems)

        public ICondition Precondition;

        public ICondition Monitor;

        public IOperator Operator;

        public List<IEffect> Effects = new List<IEffect>();

        private ITaskNodeVisitor visitor;

        public TaskNode()
        {
            Node = new TreeNode<ITaskNode>(this);
        }

        public TaskNode(string name)
        {
            Name = name;
            Node = new TreeNode<ITaskNode>(this);
        }

        public TaskNode(string name, ITaskNodeVisitor visitor)
        {
            Name = name;
            Node = new TreeNode<ITaskNode>(this);
            this.visitor = visitor;
        }

        public bool CheckConditions(int[] ws)
        {
            if (Precondition != null)
            {
                return Precondition.Eval(ws);
            }
            else
            {
                return true;
            }
        }

        public void ApplyEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Apply(ws);
            }
        }

        public bool IsComposite()
        {
            return Node.m_Children.Count > 0;
        }

        public string GetName()
        {
            return Name;
        }

        IOperator ITaskNode.GetOperator()
        {
            return Operator;
        }

        public void SetOperator(IOperator op)
        {
            Operator = op;
        }

        public ICondition GetCondition()
        {
            return Precondition;
        }

        public void SetCondition(ICondition condition)
        {
            Precondition = condition;
        }

        public ITaskNode GetChildTask(int i)
        {
            return Node.m_Children[i].m_Data;
        }

        public int NumChildrenTasks()
        {
            return Node.m_Children.Count;
        }

        public ITaskNode GetParentTask()
        {
            if (Node.m_Parent != null)
            {
                return Node.m_Parent.m_Data;
            }
            else
            {
                return null;
            }
        }

        public virtual void RestoreEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].UnApply(ws);
            }
        }

        public IEffect GetEffect(int i)
        {
            return Effects[i];
        }

        public int GetNumEffects()
        {
            return Effects.Count;
        }

        public void AddEffect(IEffect effect)
        {
            Effects.Add(effect);
        }

        public void AddChildTask(ITaskNode child)
        {
            TreeNode<ITaskNode> childNode = new TreeNode<ITaskNode>(child);
            childNode.m_Parent = Node;
            Node.m_Children.Add(childNode);
        }

        public virtual ITaskNodeVisitor CreateVisitor()
        {
            if (visitor != null)
            {
                ITaskNodeVisitor v = visitor.Copy();
                v.Visit(this);
                return v;
            }
            else
            {
                return null;
            }
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public ICondition GetMonitor()
        {
            return Monitor;
        }

        public void SetMonitor(ICondition condition)
        {
            Monitor = condition;
        }
    }

    public class TaskNode<Visitor> : ITaskNode<Visitor> where Visitor : ITaskNodeVisitor, new()
    {
        public string Name;

        public TreeNode<ITaskNode> Node;

        public ICondition Precondition;

        public IOperator Operator;

        public ICondition Monitor;

        public List<IEffect> Effects = new List<IEffect>();

        public TaskNode()
        {
            Node = new TreeNode<ITaskNode>(this);
        }

        public TaskNode(string name)
        {
            Name = name;
            Node = new TreeNode<ITaskNode>(this);
        }


        public bool CheckConditions(int[] ws)
        {
            if (Precondition != null)
            {
                return Precondition.Eval(ws);
            }
            else
            {
                return true;
            }
        }

        public void ApplyEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Apply(ws);
            }
        }

        public bool IsComposite()
        {
            return Node.m_Children.Count > 0;
        }

        public string GetName()
        {
            return Name;
        }

        IOperator ITaskNode.GetOperator()
        {
            return Operator;
        }

        public void SetOperator(IOperator op)
        {
            Operator = op;
        }

        public ICondition GetCondition()
        {
            return Precondition;
        }

        public void SetCondition(ICondition condition)
        {
            Precondition = condition;
        }

        public ITaskNode GetChildTask(int i)
        {
            return Node.m_Children[i].m_Data;
        }

        public int NumChildrenTasks()
        {
            return Node.m_Children.Count;
        }

        public ITaskNode GetParentTask()
        {
            if (Node.m_Parent != null)
            {
                return Node.m_Parent.m_Data;
            }
            else
            {
                return null;
            }
        }

        public virtual void RestoreEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].UnApply(ws);
            }
        }

        public IEffect GetEffect(int i)
        {
            return Effects[i];
        }

        public int GetNumEffects()
        {
            return Effects.Count;
        }

        public void AddEffect(IEffect effect)
        {
            Effects.Add(effect);
        }

        public void AddChildTask(ITaskNode child)
        {
            TreeNode<ITaskNode> childNode = new TreeNode<ITaskNode>(child);
            childNode.m_Parent = Node;
            Node.m_Children.Add(childNode);
        }

        public virtual ITaskNodeVisitor CreateVisitor()
        {
            Visitor v = new Visitor();
            v.Visit(this);
            return v;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public ICondition GetMonitor()
        {
            return Monitor;
        }

        public void SetMonitor(ICondition condition)
        {
            Monitor = condition;
        }
    }

    public class TaskNode<Visitor, T1, T2> : ITaskNode<Visitor, T1, T2> where Visitor : ITaskNodeVisitor where T1 : IRuntimeContext
    {
        public string Name;

        public TreeNode<ITaskNode> Node;

        public ICondition Precondition;

        public ICondition Monitor;

        public IOperator<T1, T2> Operator;

        public List<IEffect> Effects = new List<IEffect>();

        public TaskNode()
        {
            Node = new TreeNode<ITaskNode>(this);
        }

        public TaskNode(string name)
        {
            Name = name;
            Node = new TreeNode<ITaskNode>(this);
        }

        public bool CheckConditions(int[] ws)
        {
            return Precondition.Eval(ws);
        }

        public void ApplyEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Apply(ws);
            }
        }

        public bool IsComposite()
        {
            return Node.m_Children.Count > 0;
        }

        public IOperator<T1, T2> GetOperator()
        {
            return Operator;
        }

        public string GetName()
        {
            return Name;
        }

        IOperator ITaskNode.GetOperator()
        {
            return Operator;
        }

        public void SetOperator(IOperator op)
        {
            Operator = op as IOperator<T1, T2>;
        }

        public ICondition GetCondition()
        {
            return Precondition;
        }

        public void SetCondition(ICondition condition)
        {
            Precondition = condition;
        }

        public ITaskNode GetChildTask(int i)
        {
            return Node.m_Children[i].m_Data;
        }

        public int NumChildrenTasks()
        {
            return Node.m_Children.Count;
        }

        public ITaskNode GetParentTask()
        {
            if (Node.m_Parent != null)
            {
                return Node.m_Parent.m_Data;
            }
            else
            {
                return null;
            }
        }

        public virtual void RestoreEffects(int[] ws)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].UnApply(ws);
            }
        }

        public IEffect GetEffect(int i)
        {
            return Effects[i];
        }

        public int GetNumEffects()
        {
            return Effects.Count;
        }

        public void AddEffect(IEffect effect)
        {
            Effects.Add(effect);
        }

        public void AddChildTask(ITaskNode child)
        {
            TreeNode<ITaskNode> childNode = new TreeNode<ITaskNode>(child);
            childNode.m_Parent = Node;
            Node.m_Children.Add(childNode);
        }

        public virtual ITaskNodeVisitor CreateVisitor()
        {
            throw new NotImplementedException("Leaf Node");
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public ICondition GetMonitor()
        {
            return Monitor;
        }

        public void SetMonitor(ICondition condition)
        {
            Monitor = condition;
        }
    }

    public class SequenceVisitor : ITaskNodeVisitor
    {
        public ITaskNode root = null;
        private int nodeIdx = -1;
        private bool visitResult = true;

        public SequenceVisitor(ITaskNode node)
        {
            root = node;
            nodeIdx = -1;
            visitResult = true;
        }

        public SequenceVisitor() { }

        public ITaskNode GetNode()
        {
            return root;
        }

        public ITaskNode CurrentChild()
        {
            return root.GetChildTask(nodeIdx);
        }

        public bool GetVisitResult()
        {
            return visitResult;
        }

        public bool HasNext()
        {
            if (visitResult)
            {
                return nodeIdx < root.NumChildrenTasks() - 1;
            }
            else
            {
                return false;
            }
        }

        public ITaskNode NextChild()
        {
            if (visitResult)
            {
                ++nodeIdx;
                return root.GetChildTask(nodeIdx);
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            nodeIdx = -1;
            visitResult = true;
        }

        public void SetVisitResult(bool success)
        {
            visitResult = success;
        }

        public bool HasPrevious()
        {
            return nodeIdx > 0;
        }

        public ITaskNode PrevChild()
        {
            ITaskNode child = root.GetChildTask(nodeIdx);
            nodeIdx--;
            return child;
        }

        public ITaskNodeVisitor Copy()
        {
            SequenceVisitor visitor = new SequenceVisitor( root );
            visitor.nodeIdx = nodeIdx;
            visitor.visitResult = visitResult;
            return visitor;
        }

        public void Visit(ITaskNode node)
        {
            root = node;
            nodeIdx = -1;
            visitResult = true;
        }
    }

    public class SelectorVisitor : ITaskNodeVisitor
    {
        public ITaskNode root = null;
        private int nodeIdx = -1;
        private bool visitResult = false;

        public SelectorVisitor(ITaskNode node)
        {
            root = node;
            nodeIdx = -1;
            visitResult = false;
        }

        public SelectorVisitor()
        {
        }

        public ITaskNode GetNode()
        {
            return root;
        }

        public ITaskNode CurrentChild()
        {
            return root.GetChildTask(nodeIdx);
        }

        public bool GetVisitResult()
        {
            return visitResult;
        }

        public bool HasNext()
        {
            if (!visitResult)
            {
                return nodeIdx < root.NumChildrenTasks() - 1;
            }
            else
            {
                return false;
            }
        }

        public ITaskNode NextChild()
        {
            if (!visitResult)
            {
                ++nodeIdx;
                return root.GetChildTask(nodeIdx);
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            nodeIdx = -1;
            visitResult = false;
        }

        public void SetVisitResult(bool success)
        {
            visitResult = success;
        }

        public bool HasPrevious()
        {
            return false;
        }

        public ITaskNode PrevChild()
        {
            return null;
        }

        public ITaskNodeVisitor Copy()
        {
            SelectorVisitor visitor = new SelectorVisitor(root);
            visitor.nodeIdx = nodeIdx;
            visitor.visitResult = visitResult;
            return visitor;
        }

        public void Visit(ITaskNode node)
        {
            root = node;
            nodeIdx = -1;
            visitResult = false;
        }
    }

    public class SequenceTaskNode : TaskNode
    {
        public SequenceTaskNode(string name) : base(name) { }

        public override ITaskNodeVisitor CreateVisitor()
        {
            return new SequenceVisitor(this);
        }
    }

    public class SelectorTaskNode : TaskNode
    {
        public SelectorTaskNode(string name) : base(name) { }

        public override ITaskNodeVisitor CreateVisitor()
        {
            return new SelectorVisitor(this);
        }
    }

    public class CompositeTaskNode<Visitor, T1, T2> : TaskNode<Visitor, T1, T2> where Visitor : ITaskNodeVisitor, new() where T1 : IRuntimeContext
    {
        public CompositeTaskNode(string name) : base(name) { }

        public override ITaskNodeVisitor CreateVisitor()
        {
            Visitor v = new Visitor();
            v.Visit( this );
            return v;
        }
    }

    public class SequenceTaskNode<T1, T2> : TaskNode<SequenceVisitor,T1, T2> where T1 : IRuntimeContext
    {
        public SequenceTaskNode(string name) : base(name) { }

        public override ITaskNodeVisitor CreateVisitor()
        {
            return new SequenceVisitor(this);
        }
    }

    public class SelectorTaskNode<T1, T2> : TaskNode<SelectorVisitor,T1, T2> where T1 : IRuntimeContext
    {
        public SelectorTaskNode(string name) : base(name) { }

        public override ITaskNodeVisitor CreateVisitor()
        {
            return new SelectorVisitor(this);
        }
    }
}
