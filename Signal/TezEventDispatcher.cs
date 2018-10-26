﻿using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Signal
{
    public interface ITezEventData : ITezCloseable
    {
        string name { get; }
    }

    public sealed class TezEventDispatcher : ITezService
    {
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

        List<Dictionary<object, TezEventExtension.Action<ITezEventData>>> m_Listeners = new List<Dictionary<object, TezEventExtension.Action<ITezEventData>>>();
        Queue<QueuePair> m_Queue = new Queue<QueuePair>();
        Queue<KeyValuePair<int, object>> m_DeleteQueue = new Queue<KeyValuePair<int, object>>();

        private void register<EventData>() where EventData : ITezEventData
        {
            EventID<EventData>.setID(m_Listeners.Count);
            m_Listeners.Add(new Dictionary<object, TezEventExtension.Action<ITezEventData>>());
        }

        public void subscribe<EventData>(object obj, TezEventExtension.Action<ITezEventData> function) where EventData : ITezEventData
        {
            if (EventID<EventData>.ID == TezTypeInfo.ErrorID)
            {
                this.register<EventData>();
            }

            m_Listeners[EventID<EventData>.ID].Add(obj, function);
        }

        public void unsubscribe<EventData>(object obj) where EventData : ITezEventData
        {
            m_DeleteQueue.Enqueue(new KeyValuePair<int, object>(EventID<EventData>.ID, obj));

            /*            m_Listeners[EventID<EventData>.ID].Remove(obj);*/
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
            TezService.get<ITezLog>().info(string.Format("{0}", data.name));

            var dic = m_Listeners[id];
            foreach (var pair in dic)
            {
                pair.Value(data);
            }

            data.close();

            while (m_DeleteQueue.Count > 0)
            {
                var pair = m_DeleteQueue.Dequeue();
                m_Listeners[pair.Key].Remove(pair.Value);
            }
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