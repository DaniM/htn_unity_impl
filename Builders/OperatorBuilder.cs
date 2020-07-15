using System;
using System.Collections.Generic;

namespace AI
{
    /**/
    public class OperatorBuilder
    {
        private IOperator currentOperator;

        public OperatorBuilder BuildOperator(IOperator op)
        {
            currentOperator = op;
            return this;
        }

        public OperatorBuilder AddParameter(IOperatorParameter taskParameter)
        {
            currentOperator.AddParameter( taskParameter );
            return this;
        }

        public IOperator EndOperator()
        {
            return currentOperator;
        }
    }
}
