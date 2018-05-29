using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.UI;
using UnityEngine;

namespace tezcat.Core
{
    public abstract class TezcatFramework : TezWidget
    {
        public static TezcatFramework instance { get; private set; }

        #region Window
        List<TezWindow> m_WindowList = new List<TezWindow>();
        Dictionary<string, int> m_WindowDic = new Dictionary<string, int>();
        Queue<int> m_FreeWindowID = new Queue<int>();
        #endregion

        #region Layer
        List<TezLayer> m_LayerList = new List<TezLayer>();
        Dictionary<string, int> m_LayerDic = new Dictionary<string, int>();
        #endregion

        protected override void preInit()
        {
            instance = this;
        }

        protected override void initWidget()
        {
            foreach (RectTransform child in this.transform)
            {
                var layer = child.GetComponent<TezLayer>();
                if (layer != null)
                {
                    this.addLayer(layer);
                }
            }
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onRefresh()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onShow()
        {

        }

        public override void reset()
        {

        }

        #region Layer
        public void addLayer(TezLayer layer)
        {
            if (!m_LayerDic.ContainsKey(layer.name))
            {
                while (m_LayerList.Count <= layer.ID)
                {
                    m_LayerList.Add(null);
                }

                m_LayerDic.Add(layer.name, layer.ID);
                m_LayerList[layer.ID] = layer;

#if UNITY_EDITOR
                TezDebug.info("UIRoot", "Add Layer: " + layer.name + " ID: " + layer.ID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIRoot", "Repeat to add layer " + layer.name);
            }
#endif
        }
        #endregion

        #region Window
        public Window createWindow<Window>(string name, int layer) where Window : TezWindow, ITezPrefab
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(name, out id))
            {
                return (Window)m_WindowList[id];
            }

            if (m_FreeWindowID.Count > 0)
            {
                id = m_FreeWindowID.Dequeue();
            }
            else
            {
                id = m_WindowList.Count;
                m_WindowList.Add(null);
            }
            var window = Instantiate(TezPrefabDatabase.get<Window>(), m_LayerList[layer].transform, false);
            window.windowID = id;
            window.windowName = name;
            window.layer = m_LayerList[layer];
            window.transform.localPosition = Vector3.zero;

            m_WindowList[id] = window;
            m_WindowDic.Add(name, id);
            this.onCreateWindow(typeof(Window), window);
            return window;
        }

        public void removeWindow(TezWindow window)
        {
            m_FreeWindowID.Enqueue(window.windowID);
            m_WindowList[window.windowID] = null;
            m_WindowDic.Remove(window.windowName);
        }

        protected virtual void onCreateWindow(System.Type type, TezWindow window)
        {

        }
        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class TezcatFramework<T> : TezcatFramework where T : TezcatGameEngine, new()
    {
        public static T engine { get; private set; }

        protected override void preInit()
        {
            base.preInit();

            engine = new T();
            engine.preInit();
        }

        protected override void initWidget()
        {
            base.initWidget();
            StartCoroutine(engine.startEngine());
        }

        public override void clear()
        {
            engine.clear();
            engine = null;
        }
    }
}