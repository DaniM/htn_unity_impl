using System.Collections;
using System.Collections.Generic;
using System;

namespace AI
{
    public interface IBlackboard
    {
        T GetValue<T>( string name );
        void SetValue<T>(string name, T value);
        void AddValue<T>(string name, T value);
        T GetRef<T>(string name) where T : class;
    }
}
