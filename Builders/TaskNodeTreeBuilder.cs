using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace AI
{
    public class TaskNodeTreeBuilder
    {
        private WorldStateDefinition world = null;
        private Stack<ITaskNode> nodesStack = new Stack<ITaskNode>();
        private Stack<List<ICondition>> nodesMonitors = new Stack<List<ICondition>>();
        private Stack<ICondition> conditionsStack = new Stack<ICondition>();
        private IOperator currentOperator = null;
        private ITaskNode rootNode = null;


        private string printNodeStack()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in nodesStack)
            {
                sb.AppendFormat("\n{0}", item.GetName());
            }
            sb.Append("\n]");

            return sb.ToString();
        }

        public TaskNodeTreeBuilder(WorldStateDefinition wsdef, ITaskNode root)
        {
            world = wsdef;
            rootNode = root;
            nodesStack.Push(root);
            nodesMonitors.Push( new List<ICondition>() );
        }

        public TaskNodeTreeBuilder BuildTree(WorldStateDefinition wsdef, ITaskNode root)
        {
            nodesStack.Clear();
            conditionsStack.Clear();
            currentOperator = null;

            world = wsdef;
            rootNode = root;
            nodesStack.Push(root);

            return this;
        }

        public TaskNodeTreeBuilder AddCondition(ICondition condition)
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException( string.Format( "The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack() ) );
            }

            conditionsStack.Push(condition);

            return this;
        }

        public TaskNodeTreeBuilder AddMonitor(ICondition condition)
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            nodesMonitors.Peek().Add( condition );

            return this;
        }

        public TaskNodeTreeBuilder AddCondition<CONDITION>( ) where CONDITION : ICondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack() ) );
            }
            ICondition cond = new CONDITION();
            
            conditionsStack.Push(cond);


            return this;
        }

        public TaskNodeTreeBuilder AddCondition<CONDITION>( string name ) where CONDITION : ICondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack() ) );
            }
            ICondition cond = new CONDITION();
            cond.SetName( name );

            conditionsStack.Push(cond);



            return this;
        }

        public TaskNodeTreeBuilder AddWorldCondition<CONDITION>( string name, int variable1Index, ArithmeticComparisonType cmp, int variable2Index) where CONDITION : IWorldStateCondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException( string.Format( "The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack() ) );
            }

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition( world );
            cond.SetVariableIndexOrigin( variable1Index );
            cond.SetVariableIndexToCmp( variable2Index );
            cond.SetComparison( cmp );

            conditionsStack.Push(cond);


            return this;
        }

        public TaskNodeTreeBuilder AddWorldCondition<CONDITION>(string name, string variable1, ArithmeticComparisonType cmp, string variable2) where CONDITION : IWorldStateCondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException( string.Format( "The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack() ) );
            }

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableOrigin(variable1);
            cond.SetVariableToCmp(variable2);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);


            return this;
        }

        public TaskNodeTreeBuilder AddWorldConstantCondition<CONDITION>(string name, string variable, ArithmeticComparisonType cmp, int value) where CONDITION : IWorldStateCondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableOrigin(variable);
            cond.SetValueToCmp(value);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);


            return this;
        }

        public TaskNodeTreeBuilder AddWorldConstantCondition<CONDITION>(string name, int variable, ArithmeticComparisonType cmp, int value) where CONDITION : IWorldStateCondition, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            IWorldStateCondition cond = new CONDITION();
            cond.SetName(name);
            cond.SetWorldDefinition(world);
            cond.SetVariableIndexOrigin(variable);
            cond.SetValueToCmp(value);
            cond.SetComparison(cmp);

            conditionsStack.Push(cond);


            return this;
        }

        public TaskNodeTreeBuilder SetAsMotinor()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            nodesMonitors.Peek().Add( conditionsStack.Peek() );
            return this;
        }


        public TaskNodeTreeBuilder EndCondition()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            ICondition c = conditionsStack.Pop();
            if (conditionsStack.Count > 0)
            {
                conditionsStack.Peek().AddCondition(c);
                return this;
            }
            else
            {
                nodesStack.Peek().SetCondition(c);
                return this;
            }
        }

        public TaskNodeTreeBuilder AddNode(ITaskNode node)
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS a condition. Node stack: {0}", printNodeStack()));
            }
            else
            {
                nodesStack.Push(node);
                nodesMonitors.Push( new List<ICondition>() );
                return this;
            }
        }

        public TaskNodeTreeBuilder AddRefNode<NODE>(string name, ITaskNode node) where NODE : ITaskNode, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS a condition. Node stack: {0}", printNodeStack()));
            }
            else
            {
                ITaskNode linkNode = new NODE();
                linkNode.SetName(name);
                linkNode.AddChildTask( node );

                nodesStack.Push(linkNode);
                nodesMonitors.Push(new List<ICondition>());
                return this;
            }
        }


        public TaskNodeTreeBuilder AddNode<NODE>(string name) where NODE : ITaskNode, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS a condition. Node stack: {0}", printNodeStack()));
            }
            else
            {
                ITaskNode node = new NODE();
                node.SetName( name );
                nodesStack.Push( node );
                nodesMonitors.Push(new List<ICondition>());
                return this;
            }
        }

        public TaskNodeTreeBuilder EndNode()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT a task node. Node stack: {0}", printNodeStack()));
            }
            else
            {
                ITaskNode node = nodesStack.Pop();
                List<ICondition> monitors = nodesMonitors.Pop();
                if ( monitors.Count > 0 )
                {
                    if (monitors.Count > 1)
                    {
                        AndCondition monitor = new AndCondition(monitors.ToArray());
                        node.SetMonitor( monitor );
                    }
                    else
                    {
                        node.SetMonitor( monitors[0] );
                    }
                }

                if (nodesStack.Count > 0)
                {
                    nodesStack.Peek().AddChildTask(node);
                }

                return this;
            }
        }

        public TaskNodeTreeBuilder AddEffect(IEffect effect)
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT a task node. Node stack: {0}", printNodeStack()));
            }
            else
            {
                nodesStack.Peek().AddEffect(effect);
                return this;
            }
        }

        public TaskNodeTreeBuilder AddEffect<EFFECT>( int variable, EffectOperation op, int value ) where EFFECT : IEffect, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT a task node. Node stack: {0}", printNodeStack()));
            }
            else
            {
                IEffect effect = new EFFECT();
                effect.SetVariable( variable );
                effect.SetEffectOperation( op );
                effect.SetValue( value );
                nodesStack.Peek().AddEffect(effect);
                return this;
            }
        }

        public TaskNodeTreeBuilder SetOperator(IOperator op)
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT a task node. Node stack: {0}", printNodeStack()));
            }
            else
            {
                currentOperator = op;
                return this;
            }
        }

        public TaskNodeTreeBuilder SetOperator<OPERATOR>( string name ) where OPERATOR : IOperator, new()
        {
            if (currentOperator != null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. First call EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if (conditionsStack.Count > 0)
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT a task node. Node stack: {0}", printNodeStack()));
            }
            else
            {
                currentOperator = new OPERATOR();
                currentOperator.SetTaskName( name );
                return this;
            }
        }

        public TaskNodeTreeBuilder AddParameter(IOperatorParameter parameter)
        {
            if (currentOperator == null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. Node stack: {0}", printNodeStack()));
            }

            currentOperator.AddParameter(parameter);
            return this;
        }

        public TaskNodeTreeBuilder AddParameter<PARAMETER>( string name ) where PARAMETER : IOperatorParameter, new()
        {
            if (currentOperator == null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. Node stack: {0}", printNodeStack()));
            }

            IOperatorParameter parameter = new PARAMETER();
            parameter.SetName( name );
            parameter.SetBindingType( BindingType.RUNTIME );
            currentOperator.AddParameter(parameter);
            return this;
        }

        public TaskNodeTreeBuilder AddParameter<PARAMETER, VALUE>(string name, VALUE v ) where PARAMETER : IOperatorParameter, new()
        {
            if (currentOperator == null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. Node stack: {0}", printNodeStack()));
            }

            IOperatorParameter parameter = new PARAMETER();
            parameter.SetName(name);
            parameter.SetBindingType(BindingType.CONSTANT);
            parameter.SetValue( v );
            currentOperator.AddParameter(parameter);
            return this;
        }

        public TaskNodeTreeBuilder AddConstantParameter<VALUE>(VALUE v)
        {
            if (currentOperator == null)
            {
                throw new InvalidOperationException(string.Format("The current built object IS an operator. Node stack: {0}", printNodeStack()));
            }

            OperatorParameter<VALUE> parameter = new OperatorParameter<VALUE>();
            parameter.SetBindingType(BindingType.CONSTANT);
            parameter.SetValue(v);
            currentOperator.AddParameter(parameter);
            return this;
        }

        public TaskNodeTreeBuilder EndOperator()
        {
            if (currentOperator != null)
            {
                nodesStack.Peek().SetOperator(currentOperator);
                currentOperator = null;
                return this;
            }
            else
            {
                throw new InvalidOperationException(string.Format("The current built object IS NOT an operator. Node stack: {0}", printNodeStack()));
            }
        }

        public ITaskNode Build()
        {
            if ( currentOperator != null )
            {
                throw new InvalidOperationException(string.Format("Incomplete Operator. Build called before calling EndOperator(). Node stack: {0}", printNodeStack()));
            }

            if ( conditionsStack.Count > 0 )
            {
                throw new InvalidOperationException(string.Format("Incomplete Condition. Build called before calling EndCondition(). Node stack: {0}", printNodeStack()));
            }

            if ( nodesMonitors.Count > 0 )
            {
                if ( nodesMonitors.Peek().Count > 0 )
                {
                    throw new InvalidOperationException(string.Format("Incomplete Monitor. Build called before calling EndNode(). Node stack: {0}", printNodeStack()));
                }
            }

            return rootNode;
        }
    }
}
