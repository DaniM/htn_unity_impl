using System;
using System.Reflection;

namespace AI
{
    public static class DefaultOperators
    {
        public static DefaultSuccessOperator SuccessOperator = new DefaultSuccessOperator();
        public static DefaultFailedOperator FailedOperator = new DefaultFailedOperator();
    }

    public class DefaultSuccessOperator : IOperator
    {
        public void AddParameter(IOperatorParameter parameter)
        {
            throw new NotImplementedException("DefaultSuccessOperator doesn't support parameters");
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            return DefaultTasks.SuccessTask;
        }

        public string GetTaskName()
        {
            return "DefaultTasks.SuccessTask";
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Array.Empty<IOperatorParameter>();
        }

        public void SetTaskName(string name)
        {
            throw new NotImplementedException("DefaultSuccessOperator doesn't support set task");
        }
    }

    public class DefaultFailedOperator : IOperator
    {
        public void AddParameter(IOperatorParameter parameter)
        {
            throw new NotImplementedException("DefaultFailedOperator doesn't support parameters");
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            return DefaultTasks.FailTask;
        }

        public string GetTaskName()
        {
            return "DefaultTasks.FailTask";
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Array.Empty<IOperatorParameter>();
        }

        public void SetTaskName(string name)
        {
            throw new NotImplementedException("DefaultFailedOperator doesn't support set task");
        }
    }


    /**/
    public class Operator : IOperator//IOperator<T1, T2> where T1 : IRuntimeContext
    {
        public string TaskName;

        public IOperatorParameter[] Parameters = Array.Empty<IOperatorParameter>();

        public Operator() { }

        public Operator(string task)
        {
            TaskName = task;
            Parameters = Array.Empty<IOperatorParameter>();
        }

        public Operator(string task, IOperatorParameter[] parameters)
        {
            TaskName = task;
            Parameters = parameters;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            int currentLen = Parameters.Length;
            Array.Resize(ref Parameters, currentLen + 1);
            Parameters[currentLen] = parameter;
        }

        public ITask CreateTask<T1,T2>(T1 owner, T2 creator)
        {
            return DefaultTasks.CreateTask<T1, T2>(TaskName, Parameters, owner, creator);
        }

        public string GetTaskName()
        {
            return TaskName;
        }

        //public ITask CreateTask(object owner, object creator)
        //{
        //    return CreateTask((T1)owner, (T2)creator);
        //}

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {
            TaskName = name;
        }
    }

    public class CachedOperator : IOperator//IOperator<T1, T2> where T1 : IRuntimeContext
    {
        public string TaskName;

        public IOperatorParameter[] Parameters = Array.Empty<IOperatorParameter>();

        private FieldInfo[] bindedFields = null;

        private MethodInfo bindedMethod = null;

        public CachedOperator() { }

        public CachedOperator(string task)
        {
            TaskName = task;
            Parameters = Array.Empty<IOperatorParameter>();
        }

        public CachedOperator(string task, IOperatorParameter[] parameters)
        {
            TaskName = task;
            Parameters = parameters;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            int currentLen = Parameters.Length;
            Array.Resize(ref Parameters, currentLen + 1);
            Parameters[currentLen] = parameter;
        }

        public ITask CreateTask<T1, T2>(T1 owner, T2 creator)
        {
            if ( bindedFields == null )
            {
                bindedFields = DefaultTasks.FindBindedFields<T1>( Parameters );
            }

            if ( bindedMethod == null )
            {
                bindedMethod = DefaultTasks.FindMethod<T2>(TaskName);
            }

            return DefaultTasks.CreateTask<T1, T2>(bindedMethod, bindedFields, Parameters, owner, creator);
        }

        public string GetTaskName()
        {
            return TaskName;
        }

        //public ITask CreateTask(object owner, object creator)
        //{
        //    return CreateTask((T1)owner, (T2)creator);
        //}

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {
            TaskName = name;
        }
    }

    
    public class CachedOperator<OWNER,CREATOR> : IOperator//IOperator<T1, T2> where T1 : IRuntimeContext
    {
        public string TaskName;

        public IOperatorParameter[] Parameters = Array.Empty<IOperatorParameter>();

        private FieldInfo[] bindedFields = null;

        private MethodInfo bindedMethod = null;

        public CachedOperator() { }

        public CachedOperator(string task)
        {
            TaskName = task;
            Parameters = Array.Empty<IOperatorParameter>();
            bindedMethod = DefaultTasks.FindMethod<CREATOR>(TaskName);
        }

        public CachedOperator(string task, IOperatorParameter[] parameters)
        {
            TaskName = task;
            Parameters = parameters;
            bindedFields = DefaultTasks.FindBindedFields<OWNER>(Parameters);
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            int currentLen = Parameters.Length;
            Array.Resize(ref Parameters, currentLen + 1);
            Parameters[currentLen] = parameter;
            if ( parameter.GetBindingType() == BindingType.RUNTIME )
            {
                bindedFields = DefaultTasks.FindBindedFields<OWNER>(Parameters);
            }
        }

        
        public ITask CreateTask<T1, T2>(T1 owner, T2 creator)
        {
            if (typeof(T1) == typeof(OWNER) && typeof(T2) == typeof(CREATOR))
            {
                bool bindingNeeded = false;
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if ( Parameters[i].GetBindingType() == BindingType.RUNTIME )
                    {
                        bindingNeeded = true;
                        break;
                    }
                }

                if (bindingNeeded)
                {
                    if (bindedFields == null)
                    {
                        bindedFields = DefaultTasks.FindBindedFields<OWNER>(Parameters);
                    }
                }
                else
                {
                    bindedFields = Array.Empty<FieldInfo>();
                }

                if (bindedMethod == null)
                {
                    bindedMethod = DefaultTasks.FindMethod<CREATOR>(TaskName);
                }

                return DefaultTasks.CreateTask(bindedMethod, bindedFields, Parameters, owner, creator);
            }
            else
            {
                return DefaultTasks.CreateTask<T1, T2>(TaskName, Parameters, owner, creator);
            }
        }

        public string GetTaskName()
        {
            return TaskName;
        }

        //public ITask CreateTask(object owner, object creator)
        //{
        //    return CreateTask((T1)owner, (T2)creator);
        //}

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {
            TaskName = name;
            bindedMethod = DefaultTasks.FindMethod<CREATOR>(TaskName);
        }
    }
    

    public class DefaultEmptyOperator : IOperator
    {
        public DefaultEmptyOperator() { }

        public DefaultEmptyOperator(string task)
        {
        }

        public void AddParameter(IOperatorParameter parameter)
        {

        }

        public ITask CreateTask<T1, T2>(T1 owner, T2 creator)
        {
            return DefaultTasks.SuccessTask;
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Array.Empty<IOperatorParameter>();
        }

        public void SetTaskName(string name)
        {
            
        }

        public string GetTaskName()
        {
            return "DefaultTasks.SuccessTask";
        }
    }

    public class DelegateOperator : IOperator
    {
        public Func<ITask> Creator;

        public IOperatorParameter[] Parameters = Array.Empty<IOperatorParameter>();

        public DelegateOperator( Func<ITask> f )
        {
            Creator = f;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            throw new NotImplementedException("DelegateOperator don't use parameters");
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            return Creator();
        }

        public string GetTaskName()
        {
            return "DelegateOperator." + Creator.ToString();
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return null;
        }

        public void SetTaskName(string name)
        {
            
        }


    }

    public class DelegateOperator<P1> : IOperator
    {
        public Func<P1, ITask> Creator;

        public IOperatorParameter[] Parameters = new IOperatorParameter[1];

        public DelegateOperator(Func<P1,ITask> f)
        {
            Creator = f;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            Parameters[0] = parameter;
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            object[] parameters = DefaultTasks.BindParameters<T1>(Parameters, data);
            return Creator( (P1)parameters[0] );
        }

        public string GetTaskName()
        {
            return "DelegateOperator<P1>" + Creator.ToString();
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {

        }
    }

    public class DelegateOperator<P1,P2> : IOperator
    {
        public Func<P1,P2, ITask> Creator;

        public IOperatorParameter[] Parameters = new IOperatorParameter[2];

        private int currentLen = 0;

        public DelegateOperator(Func<P1, P2, ITask> f)
        {
            Creator = f;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            Parameters[currentLen] = parameter;
            currentLen++;
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            object[] parameters = DefaultTasks.BindParameters<T1>(Parameters, data);
            return Creator((P1)parameters[0], (P2)parameters[1]);
        }

        public string GetTaskName()
        {
            return "DelegateOperator<P1,P2>" + Creator.ToString();
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {

        }
    }

    public class DelegateOperator<P1,P2,P3> : IOperator
    {
        public Func<P1, P2, P3, ITask> Creator;

        public IOperatorParameter[] Parameters = new IOperatorParameter[3];

        private int currentLen = 0;

        public DelegateOperator(Func<P1, P2, P3, ITask> f)
        {
            Creator = f;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            Parameters[currentLen] = parameter;
            currentLen++;
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            object[] parameters = DefaultTasks.BindParameters<T1>(Parameters, data);
            return Creator( (P1)parameters[0], (P2)parameters[1], (P3)parameters[2] );
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {

        }

        public string GetTaskName()
        {
            return "DelegateOperator<P1,P2,P3>" + Creator.ToString();
        }
    }

    public class DelegateOperator<P1, P2, P3, P4> : IOperator
    {
        public Func<P1, P2, P3, P4, ITask> Creator;

        public IOperatorParameter[] Parameters = new IOperatorParameter[4];

        private int currentLen = 0;

        public DelegateOperator(Func<P1, P2, P3, P4, ITask> f)
        {
            Creator = f;
        }

        public void AddParameter(IOperatorParameter parameter)
        {
            Parameters[currentLen] = parameter;
            currentLen++;
        }

        public ITask CreateTask<T1, T2>(T1 data, T2 creator)
        {
            object[] parameters = DefaultTasks.BindParameters<T1>(Parameters, data);
            return Creator((P1)parameters[0], (P2)parameters[1], (P3)parameters[2], (P4)parameters[3]);
        }

        public IOperatorParameter[] ObtainParameters()
        {
            return Parameters;
        }

        public void SetTaskName(string name)
        {

        }

        public string GetTaskName()
        {
            return "DelegateOperator<P1, P2, P3, P4>" + Creator.ToString();
        }
    }

    // Add more delegates operatos if you need more parameters
}
