using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat
{
    [RequireComponent(typeof(TezUILayer))]
    public class TezUIRoot : UIBehaviour
    {
        public static TezUIRoot instance { get; private set; } = null;

        List<TezUILayer> m_LayerList = new List<TezUILayer>();
        Dictionary<string, int> m_LayerDic = new Dictionary<string, int>();

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            this.addLayer(this.GetComponent<TezUILayer>());
            foreach (RectTransform child in this.transform)
            {
                var layer = child.GetComponent<TezUILayer>();
                this.addLayer(layer);
            }
        }

        public void addLayer(TezUILayer layer)
        {
            if (!m_LayerDic.ContainsKey(layer.name))
            {
                while (m_LayerList.Count <= layer.id)
                {
                    m_LayerList.Add(null);
                }

                m_LayerDic.Add(layer.name, m_LayerList.Count);
                m_LayerList[layer.id] = layer;
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

        public T createWindow<T>(T prefab, int layer_id) where T : TezWindow
        {
            return Instantiate(prefab, m_LayerList[layer_id].transform);
        }

        public void showWindow(int layer_id, int window_id)
        {
            m_LayerList[layer_id].showWindow(window_id);
        }

        public void showWindow(string layer_name, string window_name)
        {
            int id = -1;
            if(m_LayerDic.TryGetValue(layer_name, out id))
            {
                m_LayerList[id].showWindow(window_name);
            }
#if UNITY_EDITOR
            else
            {
                TezDebug.waring("UIRoot", "Layer " + layer_name + " not found");
            }
#endif
        }
    }
}