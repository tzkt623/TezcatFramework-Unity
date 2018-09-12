using System.Collections.Generic;
using tezcat.Core;
using tezcat.TypeTraits;

namespace tezcat.Signal
{
    public interface ITezEventData : ITezCloseable
    {
        string name { get; }
    }

    public sealed class TezEventDispatcher : ITezService
    {
        #region 内置事件
        public delegate void Action();
        public delegate void Action<in T>(T arg);
        public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
        public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
        public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        public delegate R Function<R>();
        public delegate R Function<R, T>(T arg);
        public delegate R Function<R, T1, T2>(T1 arg1, T2 arg2);
        public delegate R Function<R, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
        public delegate R Function<R, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate R Function<R, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate R Function<R, T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        #endregion

        public sealed class EventID<EventData>
            : TezTypeInfo<EventData, TezEventDispatcher>
            where EventData : ITezEventData
        {
            /// <summary>
            /// 萃取类
            /// 不需要实例化
            /// </summary>
            private EventID() { }
        }

        class QueuePair : ITezCloseable
        {
            public QueuePair(int id, ITezEventData data)
            {
                this.ID = id;
                this.data = data;
            }

            public int ID;
            public ITezEventData data;

            public void close()
            {
                data = null;
            }
        }

        List<Dictionary<object, Action<ITezEventData>>> m_Listeners = new List<Dictionary<object, Action<ITezEventData>>>();
        Queue<QueuePair> m_Queue = new Queue<QueuePair>();

        private void register<EventData>() where EventData : ITezEventData
        {
            EventID<EventData>.setID(m_Listeners.Count);
            m_Listeners.Add(new Dictionary<object, Action<ITezEventData>>());
        }

        public void subscribe<EventData>(object obj, Action<ITezEventData> function) where EventData : ITezEventData
        {
            if (EventID<EventData>.ID == TezTypeInfo.ErrorID)
            {
                this.register<EventData>();
            }

            m_Listeners[EventID<EventData>.ID].Add(obj, function);
        }

        public void unsubscribe<EventData>(object obj) where EventData : ITezEventData
        {
            m_Listeners[EventID<EventData>.ID].Remove(obj);
        }

        public void dispatchEvent<EventData>(EventData data) where EventData : ITezEventData
        {
            switch (EventID<EventData>.ID)
            {
                case TezTypeInfo.ErrorID:
                    TezService.get<ITezLog>().info(string.Format("{0} : no one subscribe this event", EventID<EventData>.Name));
                    data.close();
                    break;
                default:
                    this.dispatchEvent(EventID<EventData>.ID, data);
                    break;
            }
        }

        private void dispatchEvent(int id, ITezEventData data)
        {
            var dic = m_Listeners[id];
            foreach (var pair in dic)
            {
                pair.Value(data);
            }

            data.close();
        }

        public void pushEvent<EventData>(EventData data) where EventData : ITezEventData
        {
            switch (EventID<EventData>.ID)
            {
                case TezTypeInfo.ErrorID:
                    TezService.get<ITezLog>().info(string.Format("{0} : no one subscribe this event", EventID<EventData>.Name));
                    data.close();
                    break;
                default:
                    m_Queue.Enqueue(new QueuePair(EventID<EventData>.ID, data));
                    break;
            }
        }

        public void update()
        {
            while (m_Queue.Count > 0)
            {
                var pair = m_Queue.Dequeue();
                this.dispatchEvent(pair.ID, pair.data);
                pair.close();
            }
        }

        public void close()
        {
            foreach (var listener in m_Listeners)
            {
                listener.Clear();
            }
            m_Listeners.Clear();
            m_Queue.Clear();

            m_Listeners = null;
            m_Queue = null;
        }
    }
}