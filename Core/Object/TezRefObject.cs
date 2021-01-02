using UnityEngine;
using System.Collections;

namespace tezcat.Framework.Core
{
    public interface ITezRefObject : ITezCloseable
    {
        TezRefObject.Ref refMark { get; }
    }

    public class TezWeakRef<T>
        : ITezCloseable
        where T : class, ITezRefObject
    {
        T m_Object = null;
        TezRefObject.Ref m_Ref = null;

        public TezWeakRef(T obj)
        {
            m_Ref = obj.refMark;
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
    }

    public abstract class TezRefObject : ITezRefObject
    {
        public class Ref
        {
            public bool isClosed { get; set; }
        }

        Ref m_RefMark = new Ref();
        Ref ITezRefObject.refMark => m_RefMark;

        public virtual void close()
        {
            m_RefMark.isClosed = true;
            m_RefMark = null;
        }
    }
}
