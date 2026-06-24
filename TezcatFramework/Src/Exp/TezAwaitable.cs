using System;
using System.Runtime.CompilerServices;

namespace tezcat.Framework.Exp
{
    public interface ITezAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface ITezAwaiter<out TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }

    public interface ITezCriticalAwaiter : ITezAwaiter, ICriticalNotifyCompletion
    {

    }

    public interface ITezCriticalAwaiter<out TResult> : ITezAwaiter<TResult>, ICriticalNotifyCompletion
    {

    }

    public interface ITezAwaitable
    {
        ITezAwaiter GetAwaiter();
    }

    public interface ITezAwaitable<out TAwaiter>
        where TAwaiter : ITezAwaiter
    {
        TAwaiter GetAwaiter();
    }

    public interface ITezAwaitable<out TAwaiter, out TResult>
        where TAwaiter : ITezAwaiter<TResult>
    {
        TAwaiter GetAwaiter();
    }



    public class TezAwaitable : ITezAwaitable
    {
        Awaiter mAwaiter = null;

        public TezAwaitable()
        {
            mAwaiter = new Awaiter();
        }

        public ITezAwaiter GetAwaiter()
        {
            return mAwaiter;
        }


        class Awaiter : ITezAwaiter
        {
            public bool IsCompleted { get; private set; }

            public void GetResult()
            {
                Console.WriteLine(nameof(this.GetResult));
            }

            public void OnCompleted(Action continuation)
            {
                Console.WriteLine(nameof(this.OnCompleted));
                if (this.IsCompleted)
                {
                    continuation?.Invoke();
                }
                else
                {

                }
            }
        }
    }
}