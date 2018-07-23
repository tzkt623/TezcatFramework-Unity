using System.Collections.Generic;
using tezcat.Core;

namespace tezcat.Signal
{
    public class TezEventCenter
    {
        public delegate void Action();
        public delegate void Action<T>(T arg);
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

        static int m_Count = 0; 
        public static Event register(string name)
        {
            return new Event(name, m_Count++);
        }

        public sealed class Event
        {
            public string name { get; private set; }
            public int ID { get; private set; }

            public Event(string name, int id)
            {
                this.name = name;
                this.ID = id;
            }
        }

        public abstract class EventData : ITezCloseable
        {
            public abstract Event ID { get; }
            public abstract void close();
        }

        class QueuePair : ITezCloseable
        {
            public QueuePair(Event evt, EventData data)
            {
                this.evt = evt;
                this.data = data;
            }

            public Event evt;
            public EventData data;

            public void close()
            {
                evt = null;
                data = null;
            }
        }

        List<Dictionary<object, Action<EventData>>> m_Listeners = new List<Dictionary<object, Action<EventData>>>();
        Queue<QueuePair> m_Queue = new Queue<QueuePair>();

        public void init()
        {
            for (int i = 0; i < m_Count; i++)
            {
                m_Listeners.Add(new Dictionary<object, Action<EventData>>());
            }
        }

        public void subscribe(object obj, Event evt, Action<EventData> function)
        {
            m_Listeners[evt.ID].Add(obj, function);
        }

        public void unsubscribe(object obj, Event evt)
        {
            m_Listeners[evt.ID].Remove(obj);
        }

        public void dispatchEvent(EventData data)
        {
            this.dispatchEvent(data.ID, data);
        }

        private void dispatchEvent(Event evt, EventData data)
        {
            var dic = m_Listeners[evt.ID];
            foreach (var pair in dic)
            {
                pair.Value(data);
            }

            data.close();
        }

        public void pushEvent(EventData data)
        {
            this.pushEvent(data.ID, data);
        }

        private void pushEvent(Event evt, EventData data)
        {
            m_Queue.Enqueue(new QueuePair(evt, data));
        }

        public void update()
        {
            while(m_Queue.Count > 0)
            {
                var e = m_Queue.Dequeue();
                this.dispatchEvent(e.evt, e.data);
                e.close();
            }
        }
    }
}