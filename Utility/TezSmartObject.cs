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
            private int m_Ref = 0;

            public void retain()
            {
                m_Ref += 1;
            }

            public void release()
            {
                m_Ref -= 1;
                if (m_Ref == 0)
                {
                    myObject.close();
                    myObject = default;
                }
            }

        }

        Ref m_Ref = null;

        /// <summary>
        /// 实际包含的对象
        /// </summary>
        public T value
        {
            get { return m_Ref.myObject; }
        }

        /// <summary>
        /// 创建一个新对象
        /// </summary>
        public TezSmartObject(T value)
        {
            m_Ref = new Ref() { myObject = value };
            m_Ref.retain();
        }

        /// <summary>
        /// 从另一个对象中复制
        /// </summary>
        public TezSmartObject(TezSmartObject<T> other)
        {
            m_Ref = other.m_Ref;
            m_Ref.retain();
        }

        /// <summary>
        /// 创建一个新对象
        /// </summary>
        public void set(T value)
        {
            m_Ref?.release();
            m_Ref = new Ref() { myObject = value };
            m_Ref.retain();
        }

        /// <summary>
        /// 从另一个对象中复制
        /// </summary>
        public void set(TezSmartObject<T> other)
        {
            m_Ref?.release();
            m_Ref = other.m_Ref;
            m_Ref.retain();
        }

        public void close(bool self_close = true)
        {
            m_Ref.release();
            m_Ref = null;
        }
    }
}