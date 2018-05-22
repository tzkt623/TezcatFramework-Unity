using System.Collections.Generic;
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
        
        public class Switcher
        {
            TezEventBus.Action<object>[] m_OnEvent = null;

            public Switcher()
            {
                m_OnEvent = new TezEventBus.Action<object>[m_List.Count];
            }

            ~Switcher()
            {
                for (int i = 0; i < m_OnEvent.Length; i++)
                {
                    m_OnEvent[i] = null;
                }
            }

            public void register(int event_id, TezEventBus.Action<object> action)
            {
                m_OnEvent[event_id] = action;
            }

            public void invoke(int event_id, object data)
            {
                m_OnEvent[event_id](data);
            }
        }


        #region BuildInEvent
        public static int ShowWindow { get; private set; } = -1;
        public static int HideWindow { get; private set; } = -1;
        public static int ShowArea { get; private set; } = -1;
        #endregion

        public static void initEvent()
        {
            ShowWindow = TezWidgetEvent.register("ShowWindow");
            HideWindow = TezWidgetEvent.register("HideWindow");
            ShowArea = TezWidgetEvent.register("ShowSubwindow");
        }
    }
}

