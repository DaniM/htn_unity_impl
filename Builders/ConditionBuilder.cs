using System;
using System.Collections.Generic;

using System.Text;

namespace AI
{
    /**/
    public class ConditionBuilder
    {
        private WorldStateDefinition world = null;
        private Stack<ICondition> conditionsStack = new Stack<ICondition>();
        private ICondition rootCondition = null;

        private string printConditionStack()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in conditionsStack)
            {
                sb.AppendFormat("\n{0}", item.GetName());
            }
            sb.Append("\n]");

            return sb.ToString();
        }

        public ConditionBuilder(WorldStateDefinition wsdef, ICondition root)
        {
            world = wsdef;
            rootCondition = root;
            conditionsStack.Push(root);
        }

        public ConditionBuilder BuildTree(WorldStateDefinition wsdef, ICondition root)
        {
            conditionsStack.Clear();

            world = wsdef;
            rootCondition = root;

            return this;
        }

        public ConditionBuilder AddCondition(ICondition condition)
        {
            conditionsStack.Push(condition);

            return this;
        }

        public ConditionBuilder AddCondition<CONDITION>() where CONDITION : ICondition, new()
        {
            ICondition cond = new CONDITION();

            conditionsStack.Push(cond);
            return this;
        }

        public ConditionBuilder AddCondition<CONDITION>(string name) where CONDITION : ICondition, new()
        {

            ICondition cond = new CONDITION();
            cond.SetName(name);


            conditionsStack.Push(cond);

            return this;
        }

        public ConditionBuilder AddWorldCondition<CONDITION>(string name, int variable1Index, ArithmeticComparisonType cmp, int variable2Index) where CONDITION : IWorldStateCondition, new()
        {

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableIndexOrigin(variable1Index);
            cond.SetVariableIndexToCmp(variable2Index);
            cond.SetComparison(cmp);


            conditionsStack.Push(cond);


            return this;
        }

        public ConditionBuilder AddWorldCondition<CONDITION>(string name, string variable1, ArithmeticComparisonType cmp, string variable2) where CONDITION : IWorldStateCondition, new()
        {

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableOrigin(variable1);
            cond.SetVariableToCmp(variable2);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);


            return this;
        }

        public ConditionBuilder AddWorldConstantCondition<CONDITION>(string name, string variable, ArithmeticComparisonType cmp, int value) where CONDITION : IWorldStateCondition, new()
        {

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableOrigin(variable);
            cond.SetValueToCmp(value);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);

            return this;
        }

        public ConditionBuilder AddWorldConstantCondition<CONDITION>(string name, int variable, ArithmeticComparisonType cmp, int value) where CONDITION : IWorldStateCondition, new()
        {

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableIndexOrigin(variable);
            cond.SetValueToCmp(value);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);


            return this;
        }


        public ConditionBuilder EndCondition()
        {

            ICondition c = conditionsStack.Pop();
            if (conditionsStack.Count > 0)
            {
                conditionsStack.Peek().AddCondition(c);
                return this;
            }
            else
            {
                throw new InvalidOperationException("Root condition");
            }
        }

        

        public ICondition Build()
        {
            return rootCondition;
        }
    }
}
