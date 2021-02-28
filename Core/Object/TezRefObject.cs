using UnityEngine;
using System.Collections;

namespace tezcat.Framework.Core
{
    public interface ITezRefObject : ITezCloseable
    {
        TezRefObject.Ref refHolder { get; }
    }

    public class TezWeakRef<T>
        : ITezCloseable
        where T : class, ITezRefObject
    {
        T m_Object = null;
        TezRefObject.Ref m_Ref = null;

        public TezWeakRef(T obj)
        {
            m_Ref = obj.refHolder;
            m_Object = obj;
        }

        public T get()
        {
            return m_Object;
        }

        public bool tryGet(out T obj)
        {
            obj = m_Object;
            return !m_Ref.isClosed;
        }

        public void close()
        {
            m_Ref = null;
            m_Object = null;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        public static implicit operator TezWeakRef<T>(T value)
        {
            return new TezWeakRef<T>(value);
        }
    }

    public abstract class TezRefObject : ITezRefObject
    {
        public class Ref
        {
            public bool isClosed { get; set; }
        }

        Ref m_RefHolder = new Ref();
        Ref ITezRefObject.refHolder => m_RefHolder;

        public virtual void close()
        {
            m_RefHolder.isClosed = true;
            m_RefHolder = null;
        }
    }
}
