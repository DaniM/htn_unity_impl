using System;
using System.Collections.Generic;
using static AI.ConstantComparison;

namespace AI
{
    public static class DefaultConditions
    {
        public static bool TrueConditionFunction(int[] ws) { return true; }
        public static bool FalseConditionFunction(int[] ws) { return false; }

        public static ConditionDelegate TrueCondition = new ConditionDelegate(TrueConditionFunction,null);
        public static ConditionDelegate FalseCondition = new ConditionDelegate(FalseConditionFunction, null);

        public static SimpleCondition CreateConstantCondition(string name, int index, ArithmeticComparisonType cmp, int value)
        {
            SimpleCondition cond = new SimpleCondition();
            cond.SetName( name );
            cond.SetVariableIndexOrigin( index );
            cond.SetComparison( cmp );
            cond.SetValueToCmp( value );

            return cond;
        }

        public static SimpleCondition CreateSimpleCondition(string name, int index, ArithmeticComparisonType cmp, int indexToCmp)
        {
            SimpleCondition cond = new SimpleCondition();
            cond.SetName(name);
            cond.SetVariableIndexOrigin(index);
            cond.SetComparison(cmp);
            cond.SetVariableIndexToCmp(indexToCmp);

            return cond;
        }

        public static bool Eval(int[] ws, int index, int value, CompareTo type, ArithmeticComparisonType cmp)
        {
            switch (type)
            {
                case CompareTo.CMP_WITH_FIELD:
                    value = ws[value];
                    break;
                default:
                    break;
            }

            return Eval( ws[index], value, cmp );
        }

        public static bool Eval(int v1, int v2, ArithmeticComparisonType cmp)
        {
            switch (cmp)
            {
                case ArithmeticComparisonType.EQ:
                    return v1 == v2;
                case ArithmeticComparisonType.NEQ:
                    return v1 != v2;
                case ArithmeticComparisonType.LT:
                    return v1 < v2;
                case ArithmeticComparisonType.LE:
                    return v1 <= v2;
                case ArithmeticComparisonType.GT:
                    return v1 > v2;
                case ArithmeticComparisonType.GE:
                    return v1 >= v2;
                default:
                    return false;
            }
        }
    }

    public class ConditionDelegate : ICondition
    {
        public Func<int[], bool> WorldConditionDelegate;

        public Func<IRuntimeContext, bool> RuntimeConditionDelegate;

        public ConditionDelegate(Func<int[], bool> worldCondition, Func<IRuntimeContext, bool> runtimeCondition)
        {
            WorldConditionDelegate = worldCondition;
            RuntimeConditionDelegate = runtimeCondition;
        }

        public void AddCondition(ICondition condition)
        {
            throw new NotImplementedException( "Atomic condition" );
        }

        public bool Eval(int[] worldState)
        {
            if (WorldConditionDelegate != null)
            {
                return WorldConditionDelegate.Invoke( worldState );
            }
            else
            {
                return false;
            }
        }

        public ICondition GetChildCondition(int i)
        {
            throw new NotImplementedException("Atomic condition");
        }

        public string GetName()
        {
            return null;
        }

        public int NumChildren()
        {
            return 0;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            if (RuntimeConditionDelegate != null)
            {
                return RuntimeConditionDelegate.Invoke(context);
            }
            else
            {
                return true;
            }
        }

        public void SetName(string name)
        {
            
        }
    }

    public class SimpleCondition : IWorldStateCondition
    {
        public int IndexOrigin;

        public int Value;

        public CompareTo CompareType;

        public ArithmeticComparisonType CompareLogic;

#if DEBUG
        public string Name;
#endif

        public SimpleCondition()
        {
        }

        public void AddCondition(ICondition condition)
        {
            throw new NotSupportedException( "Atomic condition" );
        }

        public bool Eval(int[] worldState)
        {
            return DefaultConditions.Eval( worldState, IndexOrigin, Value, CompareType, CompareLogic );
        }

        public ICondition GetChildCondition(int i)
        {
            throw new NotSupportedException("Atomic condition");
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public int NumChildren()
        {
            return 0;
        }

        public virtual bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }

        public void SetComparison(ArithmeticComparisonType cmp)
        {
            CompareLogic = cmp;
        }

        public void SetName(string name)
        {
#if DEBUG
            Name = name;
#endif
        }

        public void SetValueToCmp(int value)
        {
            Value = value;
            CompareType = CompareTo.CMP_WITH_CONSTANT;
        }

        public void SetVariableIndexOrigin(int variable)
        {
            IndexOrigin = variable;
        }

        public void SetVariableIndexToCmp(int variable)
        {
            Value = variable;
            CompareType = CompareTo.CMP_WITH_FIELD;
        }

        public void SetVariableOrigin(string variable)
        {
            throw new NotSupportedException( "Use other type of condition" );
        }

        public void SetVariableToCmp(string variable)
        {
            throw new NotSupportedException("Use other type of condition");
        }

        public void SetWorldDefinition(WorldStateDefinition wdef)
        {
            
        }
    }

    public class ConstantCondition : IWorldStateCondition
    {
        public string Name;

        public string Variable;

        public int Value;

        public ArithmeticComparisonType CompareLogic;

        public WorldStateDefinition World;

        public ConstantCondition()
        { }

        public ConstantCondition(string name)
        {
            Name = name;
        }


        public ConstantCondition(string name, string variable, ArithmeticComparisonType cmp, int value )
        {
            Name = name;
            Variable = variable;
            CompareLogic = cmp;
            Value = value;
        }

        public ConstantCondition( WorldStateDefinition wdef, string name, string variable, ArithmeticComparisonType cmp, int value)
        {
            Name = name;
            Variable = variable;
            CompareLogic = cmp;
            Value = value;
            World = wdef;
        }

        public void AddCondition(ICondition condition)
        {
            throw new System.NotImplementedException("Atomic condition");
        }

        public bool Eval(int[] worldState)
        {
            int value = World.FindValueInWorldState(Variable, worldState);
            return DefaultConditions.Eval( value, Value, CompareLogic );
        }

        public ICondition GetChildCondition(int i)
        {
            throw new NotImplementedException( "Atomic condition" );
        }

        public string GetName()
        {
            return Name;
        }

        public int NumChildren()
        {
            return 0;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }

        public void SetComparison(ArithmeticComparisonType cmp)
        {
            CompareLogic = cmp;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetValueToCmp(int value)
        {
            Value = value;
        }

        public void SetVariableIndexOrigin(int variable)
        {
            Variable = World.order[variable];
        }

        public void SetVariableIndexToCmp(int variable)
        {
            throw new NotSupportedException( "Only constants value supported" );
        }

        public void SetVariableOrigin(string variable)
        {
            Variable = variable;
        }

        public void SetVariableToCmp(string variable)
        {
            throw new NotSupportedException("Only constants value supported");
        }

        public void SetWorldDefinition(WorldStateDefinition wdef)
        {
            World = wdef;
        }
    }

    public class WorldStateCondition : IWorldStateCondition
    {
        public int IndexOrigin;

        public int Value;

        public CompareTo CompareType;

        public ArithmeticComparisonType CompareLogic;

        public WorldStateDefinition World;

#if DEBUG
        public string Name;
#endif

        public void AddCondition(ICondition condition)
        {
            throw new NotSupportedException("Atomic condition");
        }

        public bool Eval(int[] worldState)
        {
            return DefaultConditions.Eval(worldState, IndexOrigin, Value, CompareType, CompareLogic);
        }

        public ICondition GetChildCondition(int i)
        {
            throw new NotSupportedException("Atomic condition");
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public int NumChildren()
        {
            return 0;
        }

        public virtual bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }

        public void SetComparison(ArithmeticComparisonType cmp)
        {
            CompareLogic = cmp;
        }

        public void SetName(string name)
        {
#if DEBUG
            Name = name;
#endif
        }

        public void SetValueToCmp(int value)
        {
            throw new NotImplementedException();
        }

        public void SetVariableIndexOrigin(int variable)
        {
            throw new NotImplementedException();
        }

        public void SetVariableIndexToCmp(int variable)
        {
            throw new NotImplementedException();
        }

        public void SetVariableOrigin(string variable)
        {
            throw new NotImplementedException();
        }

        public void SetVariableToCmp(string variable)
        {
            throw new NotImplementedException();
        }

        public void SetWorldDefinition(WorldStateDefinition wdef)
        {
            World = wdef;
        }
    }

    public abstract class CompositeCondition : ICondition
    {
        public string Name;

        public ICondition[] conditions = Array.Empty<ICondition>();

        public void AddCondition(ICondition condition)
        {
            int len = conditions.Length;
            Array.Resize<ICondition>(ref conditions, len + 1);
            conditions[len] = condition;
        }

        public abstract bool Eval(int[] worldState);

        public ICondition GetChildCondition(int i)
        {
            return conditions[i];
        }

        public string GetName()
        {
            return Name;
        }

        public int NumChildren()
        {
            return conditions.Length;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            return false;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

    public class AndCondition : ICondition
    {
        public ICondition[] conditions = Array.Empty<ICondition>();

        public AndCondition()
        {
        }

        public AndCondition(string name)
        {
        }

        public AndCondition(int size)
        {
            conditions = new ICondition[size];
        }

        public AndCondition(ICondition[] conds)
        {
            conditions = conds;
        }

        public bool Eval(int[] worldState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Eval(worldState))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddCondition(ICondition condition)
        {
            int len = conditions.Length;
            System.Array.Resize<ICondition>(ref conditions, len + 1);
            conditions[len] = condition;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            return false;
        }

        public int NumChildren()
        {
            return conditions.Length;
        }

        public ICondition GetChildCondition(int i)
        {
            return conditions[i];
        }

        public void SetName(string name)
        {
            
        }

        public string GetName()
        {
            return null;
        }
    }

    public class OrCondition : ICondition
    {
        public ICondition[] conditions = Array.Empty<ICondition>();

        public OrCondition()
        {
        }

        public OrCondition(int size)
        {
            conditions = new ICondition[size];
        }

        public OrCondition(ICondition[] conds)
        {
            conditions = conds;
        }

        public bool Eval(int[] worldState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Eval(worldState))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddCondition(ICondition condition)
        {
            int len = conditions.Length;
            System.Array.Resize<ICondition>(ref conditions, len + 1);
            conditions[len] = condition;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            return false;
        }

        public int NumChildren()
        {
            return conditions.Length;
        }

        public ICondition GetChildCondition(int i)
        {
            return conditions[i];
        }

        public void SetName(string name)
        {
            
        }

        public string GetName()
        {
            return null;
        }
    }

    public class NotCondition : ICondition
    {
        public ICondition Negate;

        public NotCondition() { }

        public NotCondition(ICondition toNegate)
        {
            Negate = toNegate;
        }

        public void AddCondition(ICondition condition)
        {
            Negate = condition;
        }

        public bool Eval(int[] worldState)
        {
            return !Negate.Eval(worldState);
        }

        public ICondition GetChildCondition(int i)
        {
            return Negate;
        }

        public string GetName()
        {
            return null;
        }

        public int NumChildren()
        {
            return 1;
        }

        public bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }

        public void SetName(string name)
        {
            
        }
    }
}

