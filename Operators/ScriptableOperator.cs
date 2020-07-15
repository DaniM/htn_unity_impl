using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
    //[CreateAssetMenu(menuName = "Assets/AI/Operators/Scriptable Operator")]
    public abstract class ScriptableOperator<T1,T2> : ScriptableObject, IOperator//IOperator<T1,T2> where T1 : IRuntimeContext
    {
        public string TaskName;

        [HideInInspector]
        public OperatorParameterReference[] Parameters;

        public virtual void AddParameter(IOperatorParameter parameter)
        {
            int currentLen = Parameters.Length;
            Array.Resize<OperatorParameterReference>(ref Parameters, currentLen + 1);
            Parameters[currentLen] = parameter as OperatorParameterReference;
        }

        public ITask CreateTask(object owner, object creator)
        {
            return CreateTask( (T1)owner, (T2)creator );
        }

        public virtual ITask CreateTask<T11, T22>(T11 data, T22 creator)
        {
            return DefaultTasks.CreateTask<T11, T22>(TaskName, Parameters, data, creator);
        }

        public string GetTaskName()
        {
            return TaskName;
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {
            TaskName = name;
        }
    }
}