using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Coroutines;

namespace AI
{
    public static class DefaultTasks
    {
        public static TaskStatus FailTaskFunction() { return TaskStatus.FAILURE; }
        public static TaskStatus SuccessTaskFunction() { return TaskStatus.SUCCESS; }

        public static TaskDelegate FailTask = new TaskDelegate( FailTaskFunction );
        public static TaskDelegate SuccessTask = new TaskDelegate( SuccessTaskFunction );

        public static ITask CreateTask<T1, T2>( string taskName, IOperatorParameter[] opParameters, T1 owner, T2 creator ) //where T1 : IRuntimeContext
        {
            Type t = typeof(T1);
            FieldInfo[] fields = t.GetFields();

            object[] parameters = new object[opParameters.Length];
            

            for (int i = 0; i < opParameters.Length; i++)
            {
                if (opParameters[i].GetBindingType() == BindingType.CONSTANT)
                {
                    parameters[i] = opParameters[i].GetValue();
                }
                else
                {
                    for (int j = 0; j < fields.Length; j++)
                    {
                        TaskParameter p = fields[j].GetCustomAttribute<TaskParameter>();
                        if ( p != null )
                        {
                            if ( p.Name == opParameters[i].GetName() )
                            {
                                parameters[i] = fields[j].GetValue(owner);
                                break;
                            }
                        }
                    }
                }
            }

            MethodInfo method = FindMethod(taskName, typeof(T2));

            if (method != null)
            {
                return (ITask)method.Invoke(creator, parameters);
            }

            return DefaultTasks.FailTask;
        }

        public static ITask CreateTask<T1,T2>(MethodInfo method, FieldInfo[] bindedParameters, IOperatorParameter[] opParameters, T1 owner, T2 creator) //where T1 : IRuntimeContext
        {
            object[] parameters = new object[opParameters.Length];

            int bindedIdx = 0;
            for (int i = 0; i < opParameters.Length; i++)
            {
                if (opParameters[i].GetBindingType() == BindingType.CONSTANT)
                {
                    parameters[i] = opParameters[i].GetValue();
                }
                else
                {
                    parameters[i] = bindedParameters[bindedIdx].GetValue( owner );
                    bindedIdx++;
                }
            }
            return (ITask)method.Invoke(creator, parameters);
        }

        public static FieldInfo[] FindBindedFields<T>(IOperatorParameter[] opParameters)
        {
            Type t = typeof(T);
            List<FieldInfo> bindedFields = new List<FieldInfo>();
            FieldInfo[] fields = t.GetFields();

            for (int i = 0; i < opParameters.Length; i++)
            {
                if (opParameters[i].GetBindingType() == BindingType.CONSTANT)
                {
                    continue;
                }
                else
                {
                    for (int j = 0; j < fields.Length; j++)
                    {
                        TaskParameter p = fields[j].GetCustomAttribute<TaskParameter>();
                        if (p != null)
                        {
                            if (p.Name == opParameters[i].GetName())
                            {
                                bindedFields.Add( fields[j] );
                            }
                        }
                    }
                }
            }

            return bindedFields.ToArray();
        }

        public static object[] BindParameters<T1>(IOperatorParameter[] opParameters, T1 owner)
        {
            Type t = typeof(T1);
            FieldInfo[] fields = t.GetFields();

            object[] parameters = new object[opParameters.Length];


            for (int i = 0; i < opParameters.Length; i++)
            {
                if (opParameters[i].GetBindingType() == BindingType.CONSTANT)
                {
                    parameters[i] = opParameters[i].GetValue();
                }
                else
                {
                    for (int j = 0; j < fields.Length; j++)
                    {
                        TaskParameter p = fields[j].GetCustomAttribute<TaskParameter>();
                        if (p != null)
                        {
                            if (p.Name == opParameters[i].GetName())
                            {
                                parameters[i] = fields[j].GetValue(owner);
                                break;
                            }
                        }
                    }
                }
            }

            return parameters;
        }

        public static MethodInfo FindMethod(string methodName, Type t)
        {
            MethodInfo[] Methods = t.GetMethods();

            // Loop through all methods in this class that are in the
            // MyMemberInfo array.
            for (int i = 0; i < Methods.Length; i++)
            {
                TaskFunction att = Methods[i].GetCustomAttribute<TaskFunction>();
                if (att != null)
                {
                    if (att.Name == methodName)
                    {
                        return Methods[i];
                    }
                }
            }

            return null;
        }

        public static MethodInfo FindMethod<T>(string methodName)
        {
            Type t = typeof(T);
            MethodInfo[] Methods = t.GetMethods();

            // Loop through all methods in this class that are in the
            // MyMemberInfo array.
            for (int i = 0; i < Methods.Length; i++)
            {
                TaskFunction att = Methods[i].GetCustomAttribute<TaskFunction>();
                if (att != null)
                {
                    if (att.Name == methodName)
                    {
                        return Methods[i];
                    }
                }
            }

            return null;
        }

        public static List<MethodInfo> ListAvailableOperators<T>()
        {
            Type t = typeof(T);
            MethodInfo[] Methods = t.GetMethods();
            List<MethodInfo> methods = new List<MethodInfo>();

            // Loop through all methods in this class that are in the
            // MyMemberInfo array.
            for (int i = 0; i < Methods.Length; i++)
            {
                TaskFunction att = Methods[i].GetCustomAttribute<TaskFunction>();
                if (att != null)
                {
                    methods.Add( Methods[i] );
                }
            }

            return methods;
        }

        public static List<string> ListAvailableOperatorsNames<T>()
        {
            Type t = typeof(T);
            MethodInfo[] Methods = t.GetMethods();
            List<string> methods = new List<string>();

            // Loop through all methods in this class that are in the
            // MyMemberInfo array.
            for (int i = 0; i < Methods.Length; i++)
            {
                TaskFunction att = Methods[i].GetCustomAttribute<TaskFunction>();
                if (att != null)
                {
                    methods.Add(Methods[i].Name);
                }
            }

            return methods;
        }

        public static List<string> ListAvailableParametersNames<T>()
        {
            Type t = typeof(T);
            FieldInfo[] fields = t.GetFields();
            List<string> names = new List<string>();

            // Loop through all methods in this class that are in the
            // MyMemberInfo array.
            for (int i = 0; i < fields.Length; i++)
            {
                TaskParameter p = fields[i].GetCustomAttribute<TaskParameter>();
                if (p != null)
                {
                    names.Add( fields[i].Name );
                }
            }

            return names;
        }
    }

    public class TaskDelegate : ITask
    {
        private Func<TaskStatus> function;

        private TaskStatus status = TaskStatus.CONTINUE;

        public TaskDelegate( Func<TaskStatus> func )
        {
            function = func;
        }

        public TaskStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(TaskStatus status)
        {
            
        }

        public void Stop()
        {
            
        }

        public TaskStatus Tick()
        {
            status = function();
            return status;
        }
    }

    public class TaskCoroutine : ITask
    {
        private CoroutineWrapper<TaskStatus> coroutine;

        private TaskStatus status = TaskStatus.CONTINUE;

        public TaskCoroutine(MonoBehaviour owner, IEnumerator<TaskStatus> func)
        {
            coroutine = new CoroutineWrapper<TaskStatus>(func,owner);
        }

        public TaskStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(TaskStatus status)
        {
            this.status = status;
        }

        public void Stop()
        {
            coroutine.Stop();
            status = TaskStatus.FAILURE;
        }

        public TaskStatus Tick()
        {
            if ( status == TaskStatus.CONTINUE )
            {
                status = coroutine.Result;
            }
            return status;
        }
    }

    public class WaitTask : ITask
    {
        private float time;
        private float timeLeft;
        private TaskStatus status = TaskStatus.CONTINUE;

        public WaitTask(float t)
        {
            time = t;
            timeLeft = t;
        }

        public TaskStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(TaskStatus status)
        {
            
        }

        public void Stop()
        {
            timeLeft = time;
        }

        public TaskStatus Tick()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0 )
            {
                timeLeft = 0;
                status = TaskStatus.SUCCESS;
            }
            else
            {
                status = TaskStatus.CONTINUE;
            }
            return status;
        }
    }
}
