using System;


namespace AI
{
    public class OperatorParameter : IOperatorParameter
    {
        public string Name;

        public BindingType Binding;

        public object Value;

        public OperatorParameter() { }

        public OperatorParameter(string name)
        {
            Name = name;
            Binding = BindingType.RUNTIME;
        }

        public OperatorParameter(string name, object value)
        {
            Name = name;
            Binding = BindingType.CONSTANT;
            Value = value;
        }

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
            return Value;
        }

        public void SetBindingType(BindingType binding)
        {
            Binding = binding;
        }

        public void SetName( string name )
        {
            Name = name;
        }

        public void SetValue(object value)
        {
            Value = value;
        }
    }

    public class OperatorParameter<T> : IOperatorParameter
    {
        public string Name;

        public BindingType Binding;

        public T Value;

        public OperatorParameter() { }

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
            return Value;
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
            Value = (T)value;
        }
    }
}
