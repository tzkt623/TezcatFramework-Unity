using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public abstract class TezSenceLoadChecker<T> where T : Enum
    {
        private readonly HashSet<T> mStates = new HashSet<T>();
        public abstract string name { get; }
        private static int mMaxEnum = 0;

        public event Action onComplete;

        static TezSenceLoadChecker()
        {
            mMaxEnum = Enum.GetValues(typeof(T)).Length;
        }

        public bool sumbit(T state)
        {
            if (mStates.Contains(state))
            {
                throw new Exception();
            }

            mStates.Add(state);

            if (mStates.Count == mMaxEnum)
            {
                onComplete();
                mStates.Clear();
                return true;
            }

            return false;
        }
    }
}