using System.Collections.Generic;
namespace tezcat.UI
{
    public class TezUIEvent
    {
        static List<TezUIEvent> m_List = new List<TezUIEvent>();
        public static int register(string name)
        {
            var id = m_List.Count;
            m_List.Add(new TezUIEvent(name, id));
            return id;
        }

        public string name { get; private set; }
        public int id { get; private set; }
        TezUIEvent(string name, int id)
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
            ShowWindow = TezUIEvent.register("ShowWindow");
            HideWindow = TezUIEvent.register("HideWindow");
            ShowArea = TezUIEvent.register("ShowSubwindow");
        }
    }
}

