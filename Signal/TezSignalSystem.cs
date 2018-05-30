using System;
using System.Collections.Generic;
using tezcat.Core;

namespace tezcat.Signal
{
    public abstract class TezSignal
    {
        public int ID { get; private set; }
        public string name { get; private set; }

        public void setID(int id)
        {
            this.ID = id;
        }

        public void setName(string name)
        {
            this.name = name;
        }
    }

    public abstract class TezSignalSystem<Category> : ITezClearable where Category : TezSignal, new()
    {
        #region Manaer
        static List<Category> Manager = new List<Category>();

        public static Category register(string name)
        {
            Category signal = new Category();
            signal.setName(name);
            signal.setID(Manager.Count);
            Manager.Add(signal);
            return signal;
        }
        #endregion

        public delegate void OnSignal();
        public delegate void OnSignal<V1>(V1 v1);
        public delegate void OnSignal<V1, V2>(V1 v1, V2 v2);

        Delegate[] m_List = null;

        public TezSignalSystem()
        {
            m_List = new Delegate[Manager.Count];
        }

        #region V0
        public void emit(Category signal)
        {
            ((OnSignal)m_List[signal.ID])?.Invoke();
        }

        public void subscribe(Category signal, OnSignal on_signal)
        {
            m_List[signal.ID] = (OnSignal)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe(Category signal, OnSignal on_signal)
        {
            m_List[signal.ID] = (OnSignal)m_List[signal.ID] - on_signal;
        }
        #endregion

        #region V1
        public void emit<V1>(Category signal, V1 v1)
        {
            ((OnSignal<V1>)m_List[signal.ID])?.Invoke(v1);
        }

        public void subscribe<V1>(Category signal, OnSignal<V1> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1>)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe<V1>(Category signal, OnSignal<V1> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1>)m_List[signal.ID] - on_signal;
        }
        #endregion

        #region V2
        public void emit<V1, V2>(Category signal, V1 v1, V2 v2)
        {
            ((OnSignal<V1, V2>)m_List[signal.ID])?.Invoke(v1, v2); 
        }

        public void subscribe<V1, V2>(Category signal, OnSignal<V1, V2> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1, V2>)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe<V1, V2>(Category signal, OnSignal<V1, V2> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1, V2>)m_List[signal.ID] - on_signal;
        }
        #endregion

        public void clear()
        {
            for (int i = 0; i < m_List.Length; i++)
            {
                m_List[i] = null;
            }

            m_List = null;
        }
    }
}