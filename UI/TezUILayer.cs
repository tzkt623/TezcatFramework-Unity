using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using tezcat.Utility;

namespace tezcat
{
    public class TezUILayer : UIBehaviour
    {
        [SerializeField]
        int m_LayerID = -1;
        public int id
        {
            get { return m_LayerID; }
        }

        List<TezWindow> m_Windows = new List<TezWindow>();

        protected override void Start()
        {
            base.Start();

            foreach (RectTransform child in this.transform)
            {
                var window = child.GetComponent<TezWindow>();
                this.addWindow(window);
            }

            add(this.name, m_LayerID);
        }

        public void addWindow(TezWindow window)
        {
            window.layer = this;
            window.id = m_Windows.Count;
            m_Windows.Add(window);
        }

        public void removeWindow(TezWindow window)
        {
            this.removeWindow(window.id);
        }

        public void removeWindow(int window_id)
        {
            m_Windows.Remove(window_id, (TezWindow remove, TezWindow last) =>
            {
                last.id = remove.id;
            });
        }

        public void showWindow(int window_id)
        {
            m_Windows[window_id].show();
        }

        public void showWindow(string window_name)
        {
            foreach (var window in m_Windows)
            {
                if(window.name == window_name)
                {
                    window.show();
                }
            }
        }


        #region 静态
        static Dictionary<string, int> m_NameToID = new Dictionary<string, int>();
        void add(string name, int id)
        {
            m_NameToID.Add(name, id);
        }

        public static int toID(string name)
        {
            return m_NameToID[name];
        }
        #endregion
    }
}