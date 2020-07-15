using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum BindingType
    {
        RUNTIME,
        CONSTANT
    }

    public interface IOperatorParameter
    {
        string GetName();
        void SetName( string name );
        BindingType GetBindingType();
        void SetBindingType( BindingType binding );
        object GetValue();
        void SetValue(object value);
    }

    public interface IOperatorParameter<T> : IOperatorParameter
    {
    }

    public interface IOperator
    {
        IOperatorParameter[] ObtainParameters();
        void AddParameter(IOperatorParameter parameter);
        ITask CreateTask<T1,T2>(T1 data, T2 creator);
        void SetTaskName(string name);
        string GetTaskName();
    }

    public interface IOperator<T1, T2> : IOperator where T1 : IRuntimeContext
    {
        ITask CreateTask(T1 owner, T2 creator);
    }

}