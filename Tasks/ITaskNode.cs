using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public interface ITaskNode
    {
        string GetName();
        void SetName(string name);
        bool IsComposite();
        bool CheckConditions(int[] ws);
        void ApplyEffects(int[] ws);
        void RestoreEffects(int[] ws);

        IOperator GetOperator();
        void SetOperator(IOperator op);

        ICondition GetCondition();
        void SetCondition( ICondition condition );

        ICondition GetMonitor();
        void SetMonitor(ICondition condition);

        IEffect GetEffect( int i );
        int GetNumEffects();
        void AddEffect(IEffect effect);

        ITaskNode GetParentTask();
        ITaskNode GetChildTask( int i );
        int NumChildrenTasks();
        void AddChildTask(ITaskNode child);

        ITaskNodeVisitor CreateVisitor();
        //void SetVisitor( ITaskNodeVisitor visitor );
    }

    public interface ITaskNode<Visitor> : ITaskNode where Visitor : ITaskNodeVisitor
    {
    }

    public interface ITaskNode<Visitor,T1,T2> : ITaskNode where Visitor : ITaskNodeVisitor where T1 : IRuntimeContext
    {
    }


    public interface ITaskNodeVisitor
    {
        void Visit( ITaskNode node );

        ITaskNode GetNode();

        bool HasNext();
        ITaskNode NextChild();
        ITaskNode CurrentChild();

        bool HasPrevious();
        ITaskNode PrevChild();

        void Reset();

        bool GetVisitResult();
        void SetVisitResult( bool success );

        ITaskNodeVisitor Copy();
    }
}
