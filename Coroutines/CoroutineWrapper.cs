using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Coroutines
{

    public enum TaskState
    {
        INVALID,
        RUNNING,
        SUCCESS,
        FAILED
    };

    public interface ITask
    {
        TaskState Result
        {
            get;
        }

        bool Finished
        {
            get;
        }

        IEnumerator Run();
    }

    public class CoroutineWrapper<T> : CustomYieldInstruction
    {
        #region implemented abstract members of CustomYieldInstruction

        public override bool keepWaiting
        {
            get
            {
                Step();
                return !Finished;
            }
        }

        #endregion

        public T Result
        {
            get;
            private set;
        }

        public Coroutine coroutine
        {
            get;
            private set;
        }

        public bool Finished
        {
            get;
            private set;
        }

        private IEnumerator iterator;

        public IEnumerator Iterator
        {
            get
            {
                return iterator;
            }
        }

        private MonoBehaviour owner;

        public CoroutineWrapper(IEnumerator coroutine)
        {
            iterator = coroutine;
        }

        public Coroutine StartCoroutine(MonoBehaviour owner)
        {
            this.owner = owner;
            if (coroutine == null)
            {
                this.coroutine = owner.StartCoroutine(Run(iterator));
            }

            return coroutine;
        }

        public CoroutineWrapper(IEnumerator coroutine, MonoBehaviour owner)
        {
            this.owner = owner;
            iterator = coroutine;
            this.coroutine = owner.StartCoroutine(Run(coroutine));
        }

        private IEnumerator Run(IEnumerator coroutine)
        {
            Finished = false;
            while (coroutine.MoveNext())
            {
                Result = (T)coroutine.Current;
                yield return null;
            }
            Finished = true;
        }

        public IEnumerator Step()
        {
            if (iterator.MoveNext())
            {
                Finished = false;
                Result = (T)iterator.Current;
                //yield return false;
                yield return null;
            }
            else
            {
                Finished = true;
                //yield return true;
            }
        }

        public bool Next()
        {
            if (iterator.MoveNext())
            {
                Finished = false;
                Result = (T)iterator.Current;
                return true;
            }
            else
            {
                Finished = true;
                return false;
            }
        }

        public void Restart(IEnumerator<T> coroutine)
        {
            iterator = coroutine;
            if ( owner )
            {
                if (coroutine != null)
                {
                    owner.StopCoroutine(coroutine);
                }
                this.coroutine = owner.StartCoroutine(Run(iterator));
            }
        }

        public void Stop()
        {
            if (owner)
            {
                if (coroutine != null)
                {
                    owner.StopCoroutine(coroutine);
                }
            }
        }
    }


    public class TimedCoroutineWrapper<T> : CustomYieldInstruction
    {
        private float time = 0f;

        #region implemented abstract members of CustomYieldInstruction

        public override bool keepWaiting
        {
            get
            {
                Step();
                return !Finished;
            }
        }

        #endregion

        public bool Timeout
        {
            get { return time < 0; }
        }

        public T Result
        {
            get;
            private set;
        }

        public Coroutine coroutine
        {
            get;
            private set;
        }

        public bool Finished
        {
            get;
            private set;
        }

        private IEnumerator<T> iterator;

        public IEnumerator<T> Iterator
        {
            get
            {
                return iterator;
            }
        }

        public TimedCoroutineWrapper(IEnumerator<T> coroutine, float time)
        {
            iterator = coroutine;
            this.time = time;
        }

        public TimedCoroutineWrapper(IEnumerator<T> coroutine, MonoBehaviour owner, float time)
        {
            iterator = coroutine;
            this.time = time;
            this.coroutine = owner.StartCoroutine(Run(coroutine));
        }

        private IEnumerator Run(IEnumerator<T> coroutine)
        {
            Finished = false;
            while (time >= 0 && coroutine.MoveNext())
            {
                Result = coroutine.Current;
                time -= Time.deltaTime;
                yield return 0;
            }
            Finished = true;
        }

        public IEnumerator Step()
        {
            if (time >= 0 && iterator.MoveNext())
            {
                Finished = false;
                Result = iterator.Current;
                time -= Time.deltaTime;
                yield return 0;
            }
            else
            {
                Finished = true;
            }
        }
    }

    public class Coroutinizer<SUCCESS>
    {
        public int State = 1;

        public SUCCESS Response;


        public void Callback(SUCCESS response)
        {
            State = 0;
            Response = response;
        }
    }

    public class Coroutinizer<SUCCESS, ERROR>
    {
        public int State = 1;

        public SUCCESS Response;

        public ERROR Error;

        public void OnSuccessCallback(SUCCESS response)
        {
            State = 0;
            Response = response;
        }

        public void OnErrorCallback(ERROR error)
        {
            State = -1;
            Error = error;
        }
    }
}