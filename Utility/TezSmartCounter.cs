using UnityEngine;
using System.Collections;

namespace tezcat.MyRPG
{
    public class TezRefManager<T> where T : class
    {
        public T myObject { get; private set; } = null;
        private int m_Ref = 0;

        public TezRefManager(T my_object)
        {
            this.myObject = my_object;
        }

        public void retain()
        {
            m_Ref += 1;
        }

        public void release()
        {
            m_Ref -= 1;
            if (m_Ref == 0)
            {
                myObject = null;
            }
        }
    }

    public class TezSmartPtr<T> where T : class
    {
        TezRefManager<T> m_Manager = null;

        public T myObject
        {
            get { return m_Manager.myObject; }
        }

        public TezSmartPtr(T value)
        {
            m_Manager = new TezRefManager<T>(value);
            m_Manager.retain();
        }

        public void assgin(T value)
        {
            if (m_Manager == null)
            {
                m_Manager = new TezRefManager<T>(value);
                m_Manager.retain();
                return;
            }

            if (m_Manager.myObject == value)
            {
                m_Manager.retain();
            }
            else
            {
                m_Manager.release();
                m_Manager = new TezRefManager<T>(value);
                m_Manager.retain();
            }
        }

        public void close()
        {
            m_Manager?.release();
            m_Manager = null;
        }

        public static implicit operator TezSmartPtr<T>(T value)
        {
            return new TezSmartPtr<T>(value);
        }
    }
}