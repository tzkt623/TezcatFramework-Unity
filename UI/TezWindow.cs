using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace tezcat
{
    public abstract class TezWindow
        : TezUINodeMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerDownHandler
        , IPointerUpHandler
        , IDropHandler
    {
        public class BuildInEventName
        {
            public const string Update = "Update";
            public const string ShowSubwindow = "Show";
        }

        public class Event
        {
            public string name { get; private set; }
            public object data { get; private set; }

            public Event(string name, object data)
            {
                this.name = name;
                this.data = data;
            }
        }

        public TezUILayer layer { get; set; } = null;
        public int id { get; set; }

        TezWindowTitle m_Title = null;

        List<TezSubwindow> m_SubwindowList = new List<TezSubwindow>();
        Dictionary<string, int> m_SubwindowDic = new Dictionary<string, int>();

        TezSubwindow m_FocusSubwindow = null;

        public bool isOpen
        {
            get { return this.gameObject.activeSelf; }
        }


        protected override void Start()
        {
            base.Start();
            this.registerSubwindow();
        }

        private void registerSubwindow()
        {
            List<TezSubwindow> list = new List<TezSubwindow>();
            this.GetComponentsInChildren<TezSubwindow>(true, list);
            foreach (var sub in list)
            {
                this.addSubWindow(sub);
            }
        }

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            if (this.onHide())
            {
                this.gameObject.SetActive(false);
            }
        }

        protected virtual bool onHide()
        {
            return true;
        }

        public void close()
        {
            if (this.checkForClose())
            {
                this.onClose();
                Destroy(this.gameObject);
            }
        }

        protected virtual bool checkForClose()
        {
            List<TezEventBus.Action> close_function_list = new List<TezEventBus.Action>(m_SubwindowList.Count);

            bool result = true;
            foreach (var subwindow in m_SubwindowList)
            {
                result &= subwindow.checkOnClose();
                if (result)
                {
                    close_function_list.Add(subwindow.onClose);
                }
                else
                {
                    return false;
                }
            }

            foreach (var function in close_function_list)
            {
                function();
            }

            return true;
        }

        protected virtual void onClose()
        {

        }

        public void setTitle(string title)
        {
            if (m_Title)
            {
                m_Title.setName(title);
            }
        }

        public void setTile(TezWindowTitle title)
        {
            m_Title = title;
        }

        public void onFocusSubwindow(TezSubwindow subwindow)
        {
            m_FocusSubwindow = subwindow;
        }

        public void addSubWindow(TezSubwindow subwindow)
        {
            if(!m_SubwindowDic.ContainsKey(subwindow.windowName))
            {
                subwindow.window = this;
                subwindow.windowID = m_SubwindowList.Count;
                m_SubwindowList.Add(subwindow);
                m_SubwindowDic.Add(subwindow.windowName, subwindow.windowID);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("重复子窗口", subwindow);
            }
#endif
        }

        public void removeSubwindow(TezSubwindow subwindow)
        {
            if (subwindow.windowID == m_SubwindowList.Count - 1)
            {
                m_SubwindowList.RemoveAt(subwindow.windowID);
            }
            else
            {
                var last_window = m_SubwindowList[m_SubwindowList.Count - 1];
                m_SubwindowList[subwindow.windowID] = last_window;
                last_window.windowID = last_window.windowID;
                m_SubwindowList.RemoveAt(m_SubwindowList.Count - 1);
            }

            m_SubwindowDic.Remove(subwindow.windowName);
        }

        protected void showSubwindow(string name)
        {
            foreach (var subwindow in m_SubwindowList)
            {
                if (subwindow.windowName == name)
                {
                    subwindow.show();
                    return;
                }
            }
        }

        protected void showSubwindow(int id)
        {
            m_SubwindowList[id].show();
        }

        public T getSubwindow<T>(string name) where T : TezSubwindow
        {
            int id = -1;
            if(m_SubwindowDic.TryGetValue(name, out id))
            {
                return (T)m_SubwindowList[id];
            }

            return null;
        }

        public T getSubwindow<T>(int id) where T : TezSubwindow
        {
            if (id > m_SubwindowList.Count || id < 0)
            {
                return null;
            }

            return (T)m_SubwindowList[id];
        }

        public void dispathEvent(Event evt)
        {
            foreach (var sub in m_SubwindowList)
            {
                sub.onWindowEvent(evt);
            }
        }

        protected override void onRefresh()
        {
            foreach (var sub in m_SubwindowList)
            {
                sub.dirty = true;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            m_FocusSubwindow?.onPointerDown(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            m_FocusSubwindow?.onPointerUp(eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            m_FocusSubwindow?.onDrop(eventData);
        }
    }
}