using System;
using UnityEngine;

namespace AI
{
    interface ITaskValue
    {
        object GetValue();
    }

    public abstract class OperatorParameterValueReference : ScriptableObject, ITaskValue
    {
        public abstract object GetValue();
    }

    /**/
    public class OperatorParameterReference : IOperatorParameter<OperatorParameterValueReference>
    {
        public string Name;

        public BindingType Binding;

        public OperatorParameterValueReference Value;

        public OperatorParameterReference() { }

        public BindingType GetBindingType()
        {
            return Binding;
        }

        public string GetName()
        {
            return Name;
        }

        public object GetValue()
        {
            return Value.GetValue();
        }

        public void SetBindingType(BindingType binding)
        {
            Binding = binding;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetValue(object value)
        {
            throw new Exception( "Operation not supported" );
        }
    }
}
