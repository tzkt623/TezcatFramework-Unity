using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezCountArrayEnumerator<Array> : IEnumerator<Array>
    {
        public Array Current { get; private set; }
        object IEnumerator.Current => this.Current;

        private int mIndex;
        private int mCount;
        private Array[] mArray;

        public TezCountArrayEnumerator(Array[] array, int count)
        {
            mArray = array;
            mCount = count;
        }

        public void Dispose()
        {
            this.Current = default;
            mArray = null;
        }

        public bool MoveNext()
        {
            if (mIndex < mCount)
            {
                this.Current = mArray[mIndex];
                mIndex++;
                return true;
            }
            else
            {
                this.Current = default;
                return false;
            }
        }

        public void Reset()
        {
            mIndex = 0;
            mCount = 0;
            this.Current = default;
        }
    }
}