using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 智能对象
    /// 当引用计数为0时会自动销毁数据
    /// </summary>
    public class TezSmartObject<T>
        : ITezCloseable
        where T : ITezCloseable
    {
        class Ref
        {
            public T myObject;
            private int mRef = 0;

            public void retain()
            {
                mRef += 1;
            }

            public void release()
            {
                mRef -= 1;
                if (mRef == 0)
                {
                    myObject.close();
                    myObject = default;
                }
            }

        }

        Ref mRef = null;

        /// <summary>
        /// 实际包含的对象
        /// </summary>
        public T value
        {
            get { return mRef.myObject; }
        }

        /// <summary>
        /// 创建一个新对象
        /// </summary>
        public TezSmartObject(T value)
        {
            mRef = new Ref() { myObject = value };
            mRef.retain();
        }

        /// <summary>
        /// 从另一个对象中复制
        /// </summary>
        public TezSmartObject(TezSmartObject<T> other)
        {
            mRef = other.mRef;
            mRef.retain();
        }

        /// <summary>
        /// 创建一个新对象
        /// </summary>
        public void set(T value)
        {
            mRef?.release();
            mRef = new Ref() { myObject = value };
            mRef.retain();
        }

        /// <summary>
        /// 从另一个对象中复制
        /// </summary>
        public void set(TezSmartObject<T> other)
        {
            mRef?.release();
            mRef = other.mRef;
            mRef.retain();
        }

        void ITezCloseable.deleteThis()
        {
            mRef.release();
            mRef = null;
        }
    }
}