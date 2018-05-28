using System.Collections.Generic;
using tezcat.Utility;
using UnityEngine;

namespace tezcat.UI
{
    public class TezLayer : TezWidget
    {
        [SerializeField]
        int m_LayerID = -1;
        public int layerID
        {
            get { return m_LayerID; }
        }

        List<TezWindow> m_WindowsList = new List<TezWindow>();

        Dictionary<string, int> m_WindowDic = new Dictionary<string, int>();

        private void registerWindow(TezWindow window)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(window.windowID >= 0, "UILayer (" + this.name + ")", "Window (" + window.windowName + ") ID Must EqualGreater Than 0");
#endif
            while (m_WindowsList.Count <= window.windowID)
            {
                m_WindowsList.Add(null);
            }

            if (window.windowName == null)
            {
                window.windowName = this.name + "_Window_" + m_WindowsList.Count;
            }

            window.layer = this;
            m_WindowsList[window.windowID] = window;
            m_WindowDic.Add(window.name, window.windowID);

#if UNITY_EDITOR
            TezDebug.info("UILayer (" + this.name + ")", "Register Window: " + window.windowName + " ID: " + window.windowID);
#endif
        }

        public void addWindow(TezWindow window)
        {
#if UNITY_EDITOR
            TezDebug.isNotNull(window, "UILayer (" + this.name + ")", "Add Null Window");
#endif
            if (window.windowName == null)
            {
                window.windowName = this.name + "_Window_" + m_WindowsList.Count;
            }

            if (!m_WindowDic.ContainsKey(window.windowName))
            {
                window.layer = this;
                window.windowID = m_WindowsList.Count;
                window.transform.SetParent(this.transform, false);
                m_WindowsList.Add(window);
                m_WindowDic.Add(window.windowName, window.windowID);
#if UNITY_EDITOR
                TezDebug.info("UILayer (" + this.name + ")", "Add Window: " + window.windowName + " ID: " + window.windowID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.info("UILayer (" + this.name + ")", "Repeat To Add Window: " + window.windowName + " ID: " + window.windowID);
            }
#endif
        }

        public TezWindow getWindow(int window_id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(window_id < m_WindowsList.Count, "UILayer (" + this.name + ")", "Window ID Out Of Range");
#endif
            return m_WindowsList[window_id];
        }

        public void removeWindow(TezWindow window)
        {
            this.removeWindow(window.windowID);
        }

        public void removeWindow(int window_id)
        {
            m_WindowsList.Remove(
                window_id,

                (TezWindow remove, TezWindow last) =>
                {
                    last.windowID = remove.windowID;
                    m_WindowDic.Remove(remove.windowName);
                    m_WindowDic[last.windowName] = last.windowID;
                },

                (TezWindow remove) =>
                {
                    m_WindowDic.Remove(remove.windowName);
                });
        }

        public void onWindowNameChanged(string old_name, string new_name)
        {
            int id = m_WindowDic[old_name];
            m_WindowDic.Remove(old_name);
            m_WindowDic.Add(new_name, id);

#if UNITY_EDITOR
            TezDebug.info("UILayer (" + this.name + ")", "Window Name: " + old_name + " Change To: " + new_name);
#endif
        }

        public void openWindow(int window_id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(window_id < m_WindowsList.Count, "UILayer (" + this.name + ")", "Window ID Out Of Range");
            TezDebug.info("UILayer (" + this.name + ")", "Show Window: " + m_WindowsList[window_id].name + " ID: " + window_id);
#endif
            m_WindowsList[window_id].open();
        }

        public void openWindow(string window_name)
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(window_name, out id))
            {
                this.openWindow(id);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UILayer (" + this.name + ")", "Window: " + window_name + " Didn't Exist");
            }
#endif
        }

        protected override void onRefresh()
        {

        }

        public override void clear()
        {
            foreach (var window in m_WindowsList)
            {
                window.close();
            }

            m_WindowsList.Clear();
            m_WindowsList = null;

            m_WindowDic.Clear();
            m_WindowDic = null;
        }

        protected override void preInit()
        {
            foreach (RectTransform child in this.transform)
            {
                var window = child.GetComponent<TezWindow>();
                if (window)
                {
                    this.registerWindow(window);
                }
            }
        }

        protected override void initWidget()
        {

        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }
    }
}