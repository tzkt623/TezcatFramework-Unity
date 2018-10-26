using System;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.EntitySystem
{
    public class StructPool<T> : IEnumerable<int>, IEnumerable where T : struct
    {
        public T[] Pool;
        private short[] mIndices;
        public int Capacity;
        private int mCount;
        private int mFirstFree;
        private int mFirstUsed;
        private int mVersion;

        public StructPool(int maxCapacity)
        {
            if (maxCapacity <= 0 || maxCapacity > 32767)
            {
                throw new ArgumentOutOfRangeException("maxCapacity");
            }
            this.Capacity = maxCapacity;
            this.Pool = new T[this.Capacity];
            this.mIndices = new short[this.Capacity];
            this.mVersion = 0;
            this.ReleaseAll();
        }

        public int Count
        {
            get
            {
                return this.mCount;
            }
        }

        public StructPool<T>.StructPoolState GetState()
        {
            StructPool<T>.StructPoolState result;
            result.Capacity = this.Capacity;
            result.mCount = this.mCount;
            result.mFirstFree = this.mFirstFree;
            result.mFirstUsed = this.mFirstUsed;
            result.mVersion = this.mVersion;
            result.mIndices = new short[result.mCount];
            result.Pool = new T[result.mCount];
            int num = 0;
            for (int num2 = result.mFirstUsed; num2 != result.Capacity; num2 = (int)this.mIndices[num2])
            {
                result.mIndices[num] = this.mIndices[num2];
                result.Pool[num] = this.Pool[num2];
                num++;
            }
            return result;
        }

        public void SetFromState(StructPool<T>.StructPoolState data)
        {
            this.ReleaseAll();
            this.Capacity = data.Capacity;
            this.mCount = data.mCount;
            this.mFirstFree = data.mFirstFree;
            this.mFirstUsed = data.mFirstUsed;
            this.mVersion = data.mVersion;
            this.Pool = new T[this.Capacity];
            int num = 0;
            for (int num2 = this.mFirstUsed; num2 != this.Capacity; num2 = (int)this.mIndices[num2])
            {
                this.mIndices[num2] = data.mIndices[num];
                this.Pool[num2] = data.Pool[num];
                num++;
            }
            if (data.mIndices.Length > 1)
            {
                int num3 = (int)data.mIndices[data.mIndices.Length - 2];
                int i = this.mFirstFree;
                int num4 = this.mFirstFree + 1;
                while (i < num3)
                {
                    if (this.mIndices[num4] < 0)
                    {
                        this.mIndices[i] = (short)(-(short)num4);
                        i = num4;
                    }
                    num4++;
                }
            }
        }

        public int Reserve()
        {
            if (this.mFirstFree < this.Capacity)
            {
                int num = this.mFirstFree;
                this.mFirstFree = (int)(-(int)this.mIndices[this.mFirstFree]);
                if (num < this.mFirstUsed)
                {
                    this.mIndices[num] = (short)this.mFirstUsed;
                    this.mFirstUsed = num;
                }
                else
                {
                    this.mIndices[num] = this.mIndices[num - 1];
                    this.mIndices[num - 1] = (short)num;
                }
                this.mCount++;
                this.mVersion++;
                return num;
            }
            return -1;
        }

        public void Release(int indexToFree)
        {
            int num = (int)this.mIndices[indexToFree];
            if (num < 0)
            {
                throw new IndexOutOfRangeException("Item is not currently reserved.");
            }
            int num2 = -1;
            if (indexToFree == this.mFirstUsed)
            {
                this.mFirstUsed = num;
            }
            else
            {
                num2 = indexToFree;
                do
                {
                    num2--;
                }
                while ((int)this.mIndices[num2] != indexToFree);
            }
            if (indexToFree < this.mFirstFree)
            {
                this.mIndices[indexToFree] = (short)(-(short)this.mFirstFree);
                this.mFirstFree = indexToFree;
            }
            else
            {
                int num3 = indexToFree;
                do
                {
                    num3--;
                }
                while ((int)(-(int)this.mIndices[num3]) <= indexToFree);
                this.mIndices[indexToFree] = this.mIndices[num3];
                this.mIndices[num3] = (short)(-(short)indexToFree);
            }
            if (num2 >= 0)
            {
                this.mIndices[num2] = (short)num;
            }
            this.mCount--;
            this.mVersion++;
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < this.Capacity; i++)
            {
                this.mIndices[i] = (short)(-(short)(i + 1));
            }
            this.mCount = 0;
            this.mFirstFree = 0;
            this.mFirstUsed = this.Capacity;
            this.mVersion++;
        }


        public IEnumerator<int> GetEnumerator()
        {
            return new StructPool<T>.Enumerator(this);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }



        public struct StructPoolState
        {

            public T[] Pool;


            public short[] mIndices;


            public int Capacity;


            public int mCount;


            public int mFirstFree;


            public int mFirstUsed;


            public int mVersion;
        }


        private struct Enumerator : IEnumerator<int>, IDisposable, IEnumerator
        {
            private StructPool<T> mPool;
            private int mCurrent;
            private int mNext;
            private int mIteratingVersion;

            public Enumerator(StructPool<T> pool)
            {
                this.mPool = pool;
                this.mCurrent = -1;
                this.mNext = pool.mFirstUsed;
                this.mIteratingVersion = pool.mVersion;
            }


            public int Current
            {
                get
                {
                    return this.mCurrent;
                }
            }


            public bool MoveNext()
            {
                if (this.mIteratingVersion == this.mPool.mVersion && this.mNext < this.mPool.Capacity)
                {
                    this.mCurrent = this.mNext;
                    this.mNext = (int)this.mPool.mIndices[this.mNext];
                    return true;
                }
                return this.MoveNextRare();
            }


            private bool MoveNextRare()
            {
                if (this.mIteratingVersion != this.mPool.mVersion)
                {
                    while (this.mNext < this.mPool.Capacity && this.mPool.mIndices[this.mNext] < 0)
                    {
                        this.mNext++;
                    }
                    if (this.mNext < this.mPool.Capacity)
                    {
                        this.mCurrent = this.mNext;
                        this.mNext = (int)this.mPool.mIndices[this.mNext];
                        return true;
                    }
                }
                return false;
            }


            public void Reset()
            {
                this.mCurrent = -1;
                this.mNext = this.mPool.mFirstUsed;
            }

            void IDisposable.Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.mCurrent;
                }
            }
        }
    }
}
