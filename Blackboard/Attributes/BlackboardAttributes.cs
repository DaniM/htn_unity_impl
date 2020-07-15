using System;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class BlackboardField : Attribute
    {
        public string Name;

        public BlackboardField(string name)
        {
            Name = name;
        }
    }
}
