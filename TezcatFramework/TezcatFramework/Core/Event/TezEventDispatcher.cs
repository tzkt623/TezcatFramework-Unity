using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public sealed class TezEventDispatcher : ITezCloseable
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

            void ITezCloseable.deleteThis()
            {
                data = null;
            }
        }

        List<Dictionary<object, TezEventExtension.Action<ITezEventData>>> mListeners = new List<Dictionary<object, TezEventExtension.Action<ITezEventData>>>();
        Queue<QueuePair> mQueue = new Queue<QueuePair>();
        Queue<KeyValuePair<int, object>> mDeleteQueue = new Queue<KeyValuePair<int, object>>();

        private void register<EventData>() where EventData : ITezEventData
        {
            EventID<EventData>.setID(mListeners.Count);
            mListeners.Add(new Dictionary<object, TezEventExtension.Action<ITezEventData>>());
        }

        public void subscribe<EventData>(object listener, TezEventExtension.Action<ITezEventData> function) where EventData : ITezEventData
        {
            if (EventID<EventData>.ID == TezTypeInfo.ErrorID)
            {
                this.register<EventData>();
            }

            mListeners[EventID<EventData>.ID].Add(listener, function);
        }

        public void unsubscribe<EventData>(object listener) where EventData : ITezEventData
        {
            mDeleteQueue.Enqueue(new KeyValuePair<int, object>(EventID<EventData>.ID, listener));

            /*            m_Listeners[EventID<EventData>.ID].Remove(obj);*/
        }

        public void dispatchEvent<EventData>(EventData data) where EventData : ITezEventData
        {
            switch (EventID<EventData>.ID)
            {
                case TezTypeInfo.ErrorID:
                    data.close();
                    break;
                default:
                    this.dispatchEvent(EventID<EventData>.ID, data);
                    break;
            }
        }

        private void dispatchEvent(int id, ITezEventData data)
        {
            var dic = mListeners[id];
            foreach (var pair in dic)
            {
                pair.Value(data);
            }

            data.close();

            while (mDeleteQueue.Count > 0)
            {
                var pair = mDeleteQueue.Dequeue();
                mListeners[pair.Key].Remove(pair.Value);
            }
        }

        public void pushEvent<EventData>(EventData data) where EventData : ITezEventData
        {
            switch (EventID<EventData>.ID)
            {
                case TezTypeInfo.ErrorID:
                    data.close();
                    break;
                default:
                    mQueue.Enqueue(new QueuePair(EventID<EventData>.ID, data));
                    break;
            }
        }

        public void update()
        {
            while (mQueue.Count > 0)
            {
                var pair = mQueue.Dequeue();
                this.dispatchEvent(pair.ID, pair.data);
                pair.close();
            }
        }

        void ITezCloseable.deleteThis()
        {
            foreach (var listener in mListeners)
            {
                listener.Clear();
            }
            mListeners.Clear();
            mQueue.Clear();

            mListeners = null;
            mQueue = null;
        }
    }
}