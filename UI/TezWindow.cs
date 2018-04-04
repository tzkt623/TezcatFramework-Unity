using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using tezcat.Utility;
using UnityEngine;

namespace tezcat.UI
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

        public TezLayer layer { get; set; } = null;


        [SerializeField]
        int m_WindowID = -1;
        public int windowID
        {
            get { return m_WindowID; }
            set { m_WindowID = value; }
        }

        [SerializeField]
        private string m_WindowName = null;
        public string windowName
        {
            get
            {
                return m_WindowName;
            }
            set
            {
                this.layer?.onWindowNameChanged(m_WindowName, value);
                m_WindowName = value;
                this.name = m_WindowName;
            }
        }

        TezWindowTitle m_Title = null;

        List<TezSubwindow> m_SubwindowList = new List<TezSubwindow>();
        Dictionary<string, int> m_SubwindowDic = new Dictionary<string, int>();
        TezSubwindow m_FocusSubwindow = null;

        protected override void Awake()
        {
            base.Awake();
            List<TezSubwindow> list = new List<TezSubwindow>();
            this.GetComponentsInChildren<TezSubwindow>(true, list);
            foreach (var sub in list)
            {
                this.registerSubwindow(sub);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        private void registerSubwindow(TezSubwindow subwindow)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(subwindow.subwindowID >= 0, "UIWindow (" + m_WindowName + ")", "Window (" + subwindow.windowName + ") ID Must EqualGreater Than 0");
#endif
            while (m_SubwindowList.Count <= subwindow.subwindowID)
            {
                m_SubwindowList.Add(null);
            }

            if (subwindow.windowName == null)
            {
                subwindow.windowName = m_WindowName + "_SubWindow_" + m_SubwindowList.Count;
            }

            subwindow.window = this;
            m_SubwindowList[subwindow.subwindowID] = subwindow;
            m_SubwindowDic.Add(subwindow.windowName, subwindow.subwindowID);

#if UNITY_EDITOR
            TezDebug.info("UIWindow (" + m_WindowName + ")", "Register Subwindow: " + subwindow.windowName + " ID:" + subwindow.subwindowID);
#endif
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

        public void onSubwindowNameChanged(string old_name, string new_name)
        {
#if UNITY_EDITOR
            TezDebug.info("UIWindow (" + m_WindowName + ")", "Subwindow Name: " + old_name + " Change To: " + new_name);
#endif

            int id = m_SubwindowDic[old_name];
            m_SubwindowDic.Remove(old_name);
            m_SubwindowDic.Add(new_name, id);
        }

        public void addSubWindow(TezSubwindow subwindow)
        {
            if (subwindow.windowName == null)
            {
                subwindow.windowName = m_WindowName + "_SubWindow_" + m_SubwindowList.Count;
            }

            if (!m_SubwindowDic.ContainsKey(subwindow.windowName))
            {
                subwindow.window = this;
                subwindow.subwindowID = m_SubwindowList.Count;
                m_SubwindowList.Add(subwindow);
                m_SubwindowDic.Add(subwindow.windowName, subwindow.subwindowID);
#if UNITY_EDITOR
                TezDebug.info("UIWindow (" + m_WindowName + ")", "Add Subwindow: " + subwindow.windowName + " ID:" + subwindow.subwindowID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIWindow(" + m_WindowName + ")", "Subwindow Didn't Exist : " + subwindow.windowName + " ID: " + subwindow.subwindowID);
            }
#endif
        }

        public void removeSubwindow(int id)
        {
            m_SubwindowList.Remove(
                id,

                (TezSubwindow remove, TezSubwindow last) =>
                {
                    last.subwindowID = remove.subwindowID;
                    m_SubwindowDic.Remove(remove.windowName);
                    m_SubwindowDic[last.windowName] = last.subwindowID;
                },

                (TezSubwindow remove) =>
                {
                    m_SubwindowDic.Remove(remove.windowName);
                });
        }

        public void removeSubwindow(TezSubwindow subwindow)
        {
            this.removeSubwindow(subwindow.subwindowID);
        }

        protected void showSubwindow(string name)
        {
            int id = -1;
            if (m_SubwindowDic.TryGetValue(name, out id))
            {
                this.showSubwindow(id);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIWindow(" + m_WindowName + ")", "Subwindow(" + name + ") Didn't Exist");
            }
#endif
        }

        protected void showSubwindow(int id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(id < m_SubwindowList.Count, "Window(" + m_WindowName + ")", "Subwindow ID Is Out Of Range : " + id);
            TezDebug.info("UIWindow(" + m_WindowName + ")", "Show Subwindow : " + m_SubwindowList[id].windowName + " ID: " + id);
#endif
            m_SubwindowList[id].show();
        }

        public T getSubwindow<T>(string name) where T : TezSubwindow
        {
            int id = -1;
            if (m_SubwindowDic.TryGetValue(name, out id))
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

        public void dispathEvent(Event evt, bool include_self = false)
        {
            if(include_self)
            {
                this.onWindowEvent(evt);
            }

            foreach (var sub in m_SubwindowList)
            {
                sub.onWindowEvent(evt);
            }
        }

        protected virtual void onWindowEvent(Event evt)
        {

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