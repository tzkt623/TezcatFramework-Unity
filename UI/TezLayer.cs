using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using tezcat.Utility;

namespace tezcat.UI
{
    public class TezLayer : UIBehaviour
    {
        [SerializeField]
        int m_LayerID = -1;
        public int layerID
        {
            get { return m_LayerID; }
        }

        List<TezWindow> m_WindowsList = new List<TezWindow>();
        Dictionary<string, int> m_WindowDic = new Dictionary<string, int>();

        protected override void Awake()
        {
            base.Awake();
            foreach (RectTransform child in this.transform)
            {
                var window = child.GetComponent<TezWindow>();
                if (window != null)
                {
                    this.registerWindow(window);
                }
            }
        }

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

        public void showWindow(int window_id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(window_id < m_WindowsList.Count, "UILayer (" + this.name + ")", "Window ID Out Of Range");
            TezDebug.info("UILayer (" + this.name + ")", "Show Window: " + m_WindowsList[window_id].name + " ID: " + window_id);
#endif
            m_WindowsList[window_id].show();
        }

        public void showWindow(string window_name)
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(window_name, out id))
            {
                this.showWindow(id);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UILayer (" + this.name + ")", "Window: " + window_name + " Didn't Exist");
            }
#endif
        }
    }
}