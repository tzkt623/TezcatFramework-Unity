using System;
using System.Collections.Generic;
using tezcat.Core;

namespace tezcat.Signal
{
    public abstract class TezSignalCategory
    {

    }

    public abstract class TezBasicSignal
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

        protected TezBasicSignal(int id, string name)
        {
            this.ID = id;
            this.name = name;
        }
    }

    public class TezSignal<Category> : TezBasicSignal where Category : TezSignalCategory
    {
        #region Manaer
        static List<TezSignal<Category>> Manager = new List<TezSignal<Category>>();

        public static TezSignal<Category> register(string name)
        {
            TezSignal<Category> signal = new TezSignal<Category>(Manager.Count, name);
            Manager.Add(signal);
            return signal;
        }
        #endregion

        protected TezSignal(int id, string name) : base(id, name)
        {

        }
    }

    public abstract class TezSignalSystem<Category> : ITezClearable where Category : TezSignalCategory
    {
        public delegate void OnSignal();
        public delegate void OnSignal<V1>(V1 v1);
        public delegate void OnSignal<V1, V2>(V1 v1, V2 v2);

        List<Delegate> m_List = new List<Delegate>();

        #region V0
        public void emit(TezSignal<Category> signal)
        {
            ((OnSignal)m_List[signal.ID])?.Invoke();
        }

        public void subscribe(TezSignal<Category> signal, OnSignal on_signal)
        {
            this.check(signal);
            m_List[signal.ID] = (OnSignal)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe(TezSignal<Category> signal, OnSignal on_signal)
        {
            m_List[signal.ID] = (OnSignal)m_List[signal.ID] - on_signal;
        }
        #endregion

        #region V1
        public void emit<V1>(TezSignal<Category> signal, V1 v1)
        {
            ((OnSignal<V1>)m_List[signal.ID])?.Invoke(v1);
        }

        public void subscribe<V1>(TezSignal<Category> signal, OnSignal<V1> on_signal)
        {
            this.check(signal);
            m_List[signal.ID] = (OnSignal<V1>)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe<V1>(TezSignal<Category> signal, OnSignal<V1> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1>)m_List[signal.ID] - on_signal;
        }
        #endregion

        #region V2
        public void emit<V1, V2>(TezSignal<Category> signal, V1 v1, V2 v2)
        {
            ((OnSignal<V1, V2>)m_List[signal.ID])?.Invoke(v1, v2); 
        }

        public void subscribe<V1, V2>(TezSignal<Category> signal, OnSignal<V1, V2> on_signal)
        {
            this.check(signal);
            m_List[signal.ID] = (OnSignal<V1, V2>)m_List[signal.ID] + on_signal;
        }

        public void unsubscribe<V1, V2>(TezSignal<Category> signal, OnSignal<V1, V2> on_signal)
        {
            m_List[signal.ID] = (OnSignal<V1, V2>)m_List[signal.ID] - on_signal;
        }
        #endregion


        private void check(TezSignal<Category> signal)
        {
            while (signal.ID >= m_List.Count)
            {
                m_List.Add(null);
            }
        }

        public void clear()
        {
            m_List.Clear();
        }
    }
}