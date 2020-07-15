using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TaskFunction : Attribute
    {
        public string Name;

        public TaskFunction(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class TaskParameter : Attribute
    {
        public string Name;

        public TaskParameter(string name)
        {
            Name = name;
        }
    }
}
