using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    [RequireComponent(typeof(TezLayer))]
    public class TezRoot : TezWidget
    {
        public static TezRoot instance { get; private set; } = null;

        List<TezLayer> m_LayerList = new List<TezLayer>();
        Dictionary<string, int> m_LayerDic = new Dictionary<string, int>();

        protected override void Awake()
        {
            base.Awake();
            instance?.close();
            instance = this;

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

        protected override void Start()
        {
            base.Start();
        }

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

        protected override void onRefresh()
        {

        }

        protected override void clear()
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
    }
}