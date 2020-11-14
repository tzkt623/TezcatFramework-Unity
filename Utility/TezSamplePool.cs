using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    public class TezSamplePool<T>
        : ITezCloseable
        where T : class, new()
    {
        bool m_AutoRelease = false;
        TezEventExtension.Action<T> m_OnRelease;

        Stack<T> m_Pool = new Stack<T>();

        public TezSamplePool(TezEventExtension.Action<T> on_release = null)
        {
            m_AutoRelease = on_release != null;
            m_OnRelease = on_release;
        }

        public T create()
        {
            if (m_Pool.Count > 0)
            {
                return m_Pool.Pop();
            }

            return new T();
        }

        public void release(T obj)
        {
            if(obj == null)
            {
                return;
            }

            if (m_AutoRelease)
            {
                m_OnRelease(obj);
            }
            m_Pool.Push(obj);
        }

        public void close()
        {
            m_Pool.Clear();
            m_Pool = null;

            m_OnRelease = null;
        }
    }
}