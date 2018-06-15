using System.Collections;
using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.Debug;
using tezcat.UI;
using UnityEngine;

namespace tezcat.Core
{
    public abstract class TezcatFramework : TezGameWidget
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
        private int giveID()
        {
            int id = -1;
            if (m_FreeWindowID.Count > 0)
            {
                id = m_FreeWindowID.Dequeue();
            }
            else
            {
                id = m_WindowList.Count;
                m_WindowList.Add(null);
            }
            return id;
        }

        public TezWidget createWidget(TezWidget prefab, string name, RectTransform parent)
        {
            var widget = Instantiate(prefab, parent, false);
            widget.transform.localPosition = Vector3.zero;
            widget.name = name;
            return widget;
        }

        public Widget createWidget<Widget>(string name, RectTransform parent) where Widget : TezWidget
        {
            var widget = Instantiate(TezPrefabDatabase.get<Widget>(), parent, false);
            widget.transform.localPosition = Vector3.zero;
            widget.name = name;
            return widget;
        }

        public Widget createWidget<Widget>(RectTransform parent) where Widget : TezWidget
        {
            var widget = Instantiate(TezPrefabDatabase.get<Widget>(), parent, false);
            widget.transform.localPosition = Vector3.zero;
            return widget;
        }

        private Window createWindow<Window>(Window prefab, string name, int id, TezLayer layer) where Window : TezWindow, ITezPrefab
        {
            var window = Instantiate(prefab, layer.transform, false);
            window.windowID = id;
            window.windowName = name;
            window.layer = layer;
            window.transform.localPosition = Vector3.zero;

            m_WindowList[id] = window;
            m_WindowDic.Add(name, id);
            return window;
        }

        public TezWindow createWindow(ITezPrefab prefab, string name, TezLayer layer)
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(name, out id))
            {
                return m_WindowList[id];
            }

            return this.createWindow(prefab as TezWindow, name, this.giveID(), layer);
        }

        public Window createWindow<Window>(string name, TezLayer layer) where Window : TezWindow, ITezPrefab
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(name, out id))
            {
                return (Window)m_WindowList[id];
            }

            return this.createWindow(TezPrefabDatabase.get<Window>(), name, this.giveID(), layer);
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

        #region Function
        public void startCoroutine(IEnumerator enumerator)
        {
            StartCoroutine(enumerator);
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
            StartCoroutine(loading());
        }

        private IEnumerator loading()
        {
            yield return engine.launch();
            this.startMyGame();
        }

        public abstract void startMyGame();

        public override void clear()
        {
            engine.close();
            engine = null;
        }
    }
}