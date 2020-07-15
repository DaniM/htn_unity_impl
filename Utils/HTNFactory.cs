using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /**/
    public interface HTNFactory
    {
        ITaskNode Build();
    }
}
