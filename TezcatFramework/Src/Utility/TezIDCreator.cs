using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// ID生成器
    /// 生成静态整数ID
    /// 用类型T来区别类型
    /// </summary>
    public static class TezIDCreator<T>
    {
        static int mID = 0;

        public static int count
        {
            get { return mID; }
        }

        public static int next()
        {
            return mID++;
        }

        public static void reset()
        {
            mID = 0;
        }
    }

    public static class TezIDPool<T>
    {
        static Queue<int> sPool = new Queue<int>();
        static int sID = 0;

        public static int count
        {
            get { return sID; }
        }

        public static int next()
        {
            if (sPool.Count > 0)
            {
                return sPool.Dequeue();
            }
            return sID++;
        }

        public static void recycle(int id)
        {
            sPool.Enqueue(id);
        }

        public static void reset()
        {
            sID = 0;
        }
    }

    /// <summary>
    /// 池ID
    /// </summary>
    public class TezPUID<T>
    {
        static Queue<int> sPool = new Queue<int>();
        static int sID = 0;

        int mUID = -1;
        public int UID => mUID;

        public TezPUID()
        {
            if (sPool.Count > 0)
            {
                mUID = sPool.Dequeue();
            }
            else
            {
                mUID = sID++;
            }
        }

        ~TezPUID()
        {
            sPool.Enqueue(mUID);
        }
    }
}