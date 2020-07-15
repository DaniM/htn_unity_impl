using System;

namespace AI
{
    public interface ICondition
    {
        void SetName(string name);
        string GetName();

        bool Eval(int[] worldState);
        bool RuntimeEval(IRuntimeContext context);

        void AddCondition(ICondition condition);
        int NumChildren();
        ICondition GetChildCondition( int i );
    }

    public interface IWorldStateCondition : ICondition
    {
        void SetVariableOrigin( string variable );
        void SetVariableIndexOrigin( int variable );
        void SetVariableIndexToCmp( int variable );
        void SetVariableToCmp( string variable );
        void SetValueToCmp( int value );
        void SetComparison( ArithmeticComparisonType cmp );
        void SetWorldDefinition(WorldStateDefinition wdef);
    }

    public interface IConditionVisitor
    {
        ICondition GetCondition();

        bool HasNext();
        ICondition NextChild();
        ICondition CurrentChild();

        bool HasPrevious();
        ICondition PrevChild();

        void Reset();

        bool GetVisitResult();
        void SetVisitResult(bool success);

        IConditionVisitor Copy();
    }
}
