using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum TaskStatus
    {
        CONTINUE,
        SUCCESS,
        FAILURE,
        BREAK
    }


    /**/
    public interface ITask
    {
        void SetStatus( TaskStatus status );
        TaskStatus GetStatus();
        TaskStatus Tick();
        void Stop();
    }
}
