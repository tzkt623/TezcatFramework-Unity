using System;
using System.Collections.Generic;
using System.Linq;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
#if false
    public class TezInteractionLocker
    {
        public struct Token : ITezCloseable
        {
            private ulong mMutex;
            private bool mAcquired;
            public bool isAcquired => mAcquired;

            public Token(ulong mutex)
            {
                mMutex = mutex;
                mAcquired = TezInteractionLocker.instance.tryAcquire(mMutex);
            }

            public Token(ulong mutex, ulong mustBeLock, ulong mustBeUnlock)
            {
                mMutex = mutex;
                mAcquired = TezInteractionLocker.instance.tryAcquire(mMutex, mustBeLock, mustBeUnlock);
            }

            public void close()
            {
                if (mAcquired)
                {
                    TezInteractionLocker.instance.release(mMutex);
                }
            }

            public void autoRun(Action<bool> run)
            {
                run(mAcquired);
                this.close();
            }
        }

        public static Token newLock(ulong mutex)
        {
            return new Token(mutex);
        }

        public static Token newLock(ulong mutex, ulong mustBeLock, ulong mustBeUnlock)
        {
            return new Token(mutex, mustBeLock, mustBeUnlock);
        }

        public static readonly TezInteractionLocker instance = new TezInteractionLocker();

        private ulong mMutex = 0;

        private TezInteractionLocker() { }

        private bool tryAcquire(ulong mutex)
        {
            if ((mMutex & mutex) == 0)
            {
                mMutex |= mutex;
                return true;
            }

            return false;
        }

        private bool tryAcquire(ulong mutex, ulong mustBeLock, ulong mustBeUnlock)
        {
            if ((mMutex & mutex) == 0
                && (mMutex & mustBeLock) == mustBeLock
                && (mMutex & mustBeUnlock) == 0)
            {
                mMutex |= mutex;
                return true;
            }

            return false;
        }

        private void release(ulong mutex)
        {
            mMutex &= ~mutex;
        }

        public void clear()
        {
            mMutex = 0;
        }

        public bool isLocked(ulong mutex)
        {
            return (mMutex & mutex) == mutex;
        }
    }
#endif

    public class TezInteractionFSM
    {
        public class BitMask
        {
            public string name { get; }
            public ulong bit { get; }
            public int index { get; }

            internal BitMask(string name, int index)
            {
                this.name = name;
                this.bit = 1ul << index;
                this.index = index;
            }
        }

        public class State
        {
            public ulong lockBitMask { get; }
            public int index { get; }
            public StateData stateData { get; }

            public State(int index, string name, int[] myLockMaskArray, int[] beLockMaskArray, int[] beUnLockMaskArray)
            {
                for (int i = 0; i < myLockMaskArray.Length; ++i)
                {
                    this.lockBitMask |= (1ul << myLockMaskArray[i]);
                }
                this.index = index;
                this.stateData = new StateData(name, myLockMaskArray, beLockMaskArray, beUnLockMaskArray);
            }

            public void enter()
            {
                TezInteractionFSM.instance.addState(this.index);
            }

            public void exit()
            {
                TezInteractionFSM.instance.removeState(this.index);
            }
        }

        public class StateData
        {
            public string name { get; }
            public ulong lockMask { get; }
            public ulong beLockMask { get; }
            public ulong beUnLockMask { get; }

            public int[] myLockMaskArray { get; }
            public int[] beLockMaskArray { get; }
            public int[] beUnLockMaskArray { get; }

            private int mRefCount = 0;

            public StateData(string name, int[] myLockMaskArray, int[] beLockMaskArray, int[] beUnLockMaskArray)
            {
                this.name = name;
                this.myLockMaskArray = myLockMaskArray;
                this.beLockMaskArray = beLockMaskArray;
                this.beUnLockMaskArray = beUnLockMaskArray;

                this.lockMask = 0;
                this.beLockMask = 0;
                this.beUnLockMask = 0;

                for (int i = 0; i < myLockMaskArray.Length; ++i)
                {
                    this.lockMask |= (1ul << myLockMaskArray[i]);
                }

                if (beLockMaskArray != null)
                {
                    for (int i = 0; i < beLockMaskArray.Length; ++i)
                    {
                        this.beLockMask |= (1ul << beLockMaskArray[i]);
                    }
                }

                if (beUnLockMaskArray != null)
                {
                    for (int i = 0; i < beUnLockMaskArray.Length; ++i)
                    {
                        this.beUnLockMask |= (1ul << beUnLockMaskArray[i]);
                    }
                }
            }

            internal void addRef(ref int[] mBitCount)
            {
                for (int i = 0; i < myLockMaskArray.Length; ++i)
                {
                    mBitCount[myLockMaskArray[i]]++;
                }
            }

            internal bool addRef()
            {
                mRefCount++;
                return (mRefCount - 1) == 0;
            }

            internal bool subRef()
            {
                mRefCount--;
                return mRefCount == 0;
            }
        }

        public class StateToken : ITezCloseable
        {
            private int mIndex;
            private bool mAcquired;
            public bool isAcquired => mAcquired;

            public StateToken(int index)
            {
                mIndex = index;
                mAcquired = TezInteractionFSM.instance.addState(mIndex);
            }

            public void close()
            {
                if (mAcquired)
                {
                    TezInteractionFSM.instance.removeState(mIndex);
                }
            }

            public void autoRun(Action<bool> run)
            {
                run(mAcquired);
                this.close();
            }
        }

        public static readonly TezInteractionFSM instance = new TezInteractionFSM();

        private int[] mBitCount = new int[64];
        private ulong mBitLock = 0;
        List<State> mStates = new List<State>();
        List<BitMask> mBitMask = new List<BitMask>(64);

        public TezInteractionFSM()
        {
            for (int i = 0; i < mBitCount.Length; i++)
            {
                mBitCount[i] = 0;
            }
        }

        public BitMask createBitMask(string name)
        {
            var r = new BitMask(name, mBitMask.Count);
            mBitMask.Add(r);
            return r;
        }

        public State registerState(string name, int[] myLockMaskArray, int[] beLockMaskArray, int[] beUnLockMaskArray)
        {
            var state = new State(mStates.Count, name, myLockMaskArray, beLockMaskArray, beUnLockMaskArray);
            mStates.Add(state);
            return state;
        }

        //         public Token enterState(int index)
        //         {
        //             return new Token(index, @lock(index));
        //         }

        public bool isLock(ulong beLockBitMask)
        {
            return (mBitLock & beLockBitMask) == beLockBitMask;
        }

        public bool isUnLock(ulong beUnLockBitMask)
        {
            return (mBitLock & beUnLockBitMask) == 0;
        }

        private bool addState(int index)
        {
            var state = mStates[index].stateData;
            if ((state.beLockMask & mBitLock) != 0)
            {
                //锁必须锁上
                return false;
            }

            if ((state.beUnLockMask & mBitLock) != state.beUnLockMask)
            {
                //锁必须没有锁上
                return false;
            }

            if (state.addRef())
            {
                var ary = state.myLockMaskArray;
                for (int i = 0; i < ary.Length; ++i)
                {
                    mBitCount[ary[i]]++;
                }

                mBitLock |= state.lockMask;
            }

            return true;
        }

        private void removeState(int index)
        {
            var state = mStates[index].stateData;
            if (state.subRef())
            {
                var ary = state.myLockMaskArray;
                for (int i = 0; i < ary.Length; ++i)
                {
                    mBitCount[ary[i]]--;
                    if (mBitCount[ary[i]] == 0)
                    {
                        mBitLock &= ~(1ul << ary[i]);
                    }
                }
            }
        }
    }
}