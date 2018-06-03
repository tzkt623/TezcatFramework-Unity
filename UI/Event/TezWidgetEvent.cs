using System.Collections.Generic;
using tezcat.Signal;

namespace tezcat.UI
{
    public class TezWidgetEvent
    {
        static List<TezWidgetEvent> m_List = new List<TezWidgetEvent>();
        public static int register(string name)
        {
            var id = m_List.Count;
            m_List.Add(new TezWidgetEvent(name, id));
            return id;
        }

        public string name { get; private set; }
        public int id { get; private set; }
        TezWidgetEvent(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
        
        public class Dispatcher
        {
            List<TezEventBus.Action<object>> m_OnEvent = new List<TezEventBus.Action<object>>();

            public void register(int event_id, TezEventBus.Action<object> action)
            {
                while(m_OnEvent.Count <= event_id)
                {
                    m_OnEvent.Add(this.defaultFunction);
                }

                m_OnEvent[event_id] = action;
            }

            void defaultFunction(object obj)
            {

            }

            public void invoke(int event_id, object data)
            {
                m_OnEvent[event_id](data);
            }

            public void clear()
            {
                m_OnEvent.Clear();
            }
        }

        #region BuildInEvent
        public static readonly int CloseWidget = TezWidgetEvent.register("CloseWidget");
        public static readonly int ShowWindow = TezWidgetEvent.register("ShowWindow");
        public static readonly int HideWindow = TezWidgetEvent.register("HideWindow");
        public static readonly int ShowArea = TezWidgetEvent.register("ShowArea");
        public static readonly int HideArea = TezWidgetEvent.register("HideArea");
        #endregion
    }
}

