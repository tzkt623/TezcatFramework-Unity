using System.Collections.Generic;
using UnityEngine;

namespace tezcat.UI
{
    [RequireComponent(typeof(TezLayer))]
    public class TezUICenter : TezWidget
    {
        public static TezUICenter instance { get; private set; } = null;

        [SerializeField]
        List<TezWindow> m_Prefabs = new List<TezWindow>();

        Dictionary<System.Type, TezWindow> m_WindowPrefabDic = new Dictionary<System.Type, TezWindow>();
        Dictionary<string, int> m_LayerDic = new Dictionary<string, int>();

        List<TezWindow> m_WindowList = new List<TezWindow>();
        Dictionary<string, int> m_WindowDic = new Dictionary<string, int>();
        Queue<int> m_FreeWindowID = new Queue<int>();

        List<TezLayer> m_LayerList = new List<TezLayer>();

        public void addLayer(TezLayer layer)
        {
            if (!m_LayerDic.ContainsKey(layer.name))
            {
                while (m_LayerList.Count <= layer.layerID)
                {
                    m_LayerList.Add(null);
                }

                m_LayerDic.Add(layer.name, m_LayerList.Count);
                m_LayerList[layer.layerID] = layer;

#if UNITY_EDITOR
                TezDebug.info("UIRoot", "Add Layer: " + layer.name + " ID: " + layer.layerID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIRoot", "Repeat to add layer " + layer.name);
            }
#endif
        }

        public Transform getLayer(int layer_id)
        {
            return m_LayerList[layer_id].transform;
        }

        public TezWindow getWindow(int layer_id, int window_id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(layer_id < m_LayerList.Count, "UIRoot", "Layer id out of range");
#endif
            return m_LayerList[layer_id].getWindow(window_id);
        }

        public T createWindow<T>(T prefab, int layer_id) where T : TezWindow
        {
#if UNITY_EDITOR
            TezDebug.isTrue(layer_id < m_LayerList.Count, "UIRoot", "Layer id out of range");
#endif
            return Instantiate(prefab, m_LayerList[layer_id].transform);
        }

        public void openWindow(int layer_id, int window_id)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(layer_id < m_LayerList.Count, "UIRoot", "Layer id out of range");
#endif
            m_LayerList[layer_id].openWindow(window_id);
        }

        public void openWindow(int layer_id, string window_name)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(layer_id < m_LayerList.Count, "UIRoot", "Layer id out of range");
#endif
            m_LayerList[layer_id].openWindow(window_name);
        }

        public void openWindow(string layer_name, int window_id)
        {
            int id = -1;
            if (m_LayerDic.TryGetValue(layer_name, out id))
            {
                m_LayerList[id].openWindow(window_id);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIRoot", "Layer (" + layer_name + ") not found");
            }
#endif
        }

        public void openWindow(string layer_name, string window_name)
        {
            int id = -1;
            if (m_LayerDic.TryGetValue(layer_name, out id))
            {
                m_LayerList[id].openWindow(window_name);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIRoot", "Layer (" + layer_name + ") not found");
            }
#endif
        }

        public T createPopup<T>(T prefab) where T : TezPopup
        {
            var popup = Instantiate(prefab, this.transform, false);
            popup.transform.localPosition = Vector3.zero;
            return popup;
        }

        protected override void preInit()
        {
            instance?.close();
            instance = this;

            foreach (var item in m_Prefabs)
            {
                m_WindowPrefabDic.Add(item.GetType(), item);
            }
            m_Prefabs.Clear();
            m_Prefabs = null;

            this.addLayer(this.GetComponent<TezLayer>());
            foreach (RectTransform child in this.transform)
            {
                var layer = child.GetComponent<TezLayer>();
                if (layer != null)
                {
                    this.addLayer(layer);
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

        protected override void onRefresh()
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

        public override void clear()
        {
            foreach (var layer in m_LayerList)
            {
                layer.close();
            }

            m_LayerList.Clear();
            m_LayerList = null;

            m_LayerDic.Clear();
            m_LayerDic = null;
        }

        public void register(TezWindow window)
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

            window.windowID = id;
            window.transform.localPosition = Vector3.zero;
            window.open();

            m_WindowList[id] = window;
            m_WindowDic.Add(name, id);
        }

        public Window createWindow<Window>(string name, int layer) where Window : TezWindow
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

            var window = Instantiate(m_WindowPrefabDic[typeof(Window)], m_LayerList[layer].transform, false);
            window.windowID = id;
            window.layer = m_LayerList[layer];
            window.transform.localPosition = Vector3.zero;
            window.open();

            m_WindowList[id] = window;
            m_WindowDic.Add(name, id);
            return (Window)window;
        }

        public void removeWindow(TezWindow window)
        {
            m_FreeWindowID.Enqueue(window.windowID);
            m_WindowList[window.windowID] = null;
            m_WindowDic.Remove(window.windowName);
        }
    }
}