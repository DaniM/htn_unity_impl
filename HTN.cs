using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /*
     * Implements ITaskNode interface just in case we want to embed this htm into another
     */
    public class HTN<T1,T2> : ITaskNode<SelectorVisitor,T1, T2> where T1 : IRuntimeContext
    {
        public TaskNode<SelectorVisitor,T1,T2> Root;

        public void AddChildTask(ITaskNode child)
        {
            throw new System.NotImplementedException();
        }

        public void AddEffect(IEffect effect)
        {
            throw new System.NotImplementedException();
        }

        public void ApplyEffects(int[] ws)
        {
            Root.ApplyEffects( ws );
        }

        public bool CheckConditions(int[] ws)
        {
            return Root.CheckConditions( ws );
        }

        public ITaskNode GetChildTask(int i)
        {
            return Root.GetChildTask( i );
        }

        public ICondition GetCondition()
        {
            return Root.GetCondition();
        }

        public IEffect GetEffect(int i)
        {
            return Root.GetEffect( i );
        }

        public string GetName()
        {
            return Root.GetName();
        }

        public int GetNumEffects()
        {
            return Root.GetNumEffects();
        }

        public IOperator GetOperator()
        {
            return Root.GetOperator();
        }

        public ITaskNode GetParentTask()
        {
            return null;
        }

        public ITaskNodeVisitor CreateVisitor()
        {
            return Root.CreateVisitor();
        }

        public bool IsComposite()
        {
            return Root.IsComposite();
        }

        public int NumChildrenTasks()
        {
            throw new System.NotImplementedException();
        }

        public void RestoreEffects(int[] ws)
        {
            throw new System.NotImplementedException();
        }

        public void SetCondition(ICondition condition)
        {
            throw new System.NotImplementedException();
        }

        public void SetName(string name)
        {
            Root.SetName( name );
        }

        public void SetOperator(IOperator op)
        {
            Root.SetOperator( op );
        }

        public ICondition GetMonitor()
        {
            return Root.GetMonitor();
        }

        public void SetMonitor(ICondition condition)
        {
            Root.SetMonitor( condition );
        }
    }
}
