using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.UI
{
    public class TezLayer : TezWidget
    {
        #region Manater
        static List<TezLayer> Manager = new List<TezLayer>();

        public static TezLayer get(int index)
        {
            return Manager[index];
        }

        public static TezLayer last
        {
            get { return Manager[Manager.Count - 1]; }
        }

        public static TezLayer overlay { get; protected set; } = null;

        public static void register(TezLayer layer)
        {
            if (layer.ID == -1)
            {
                layer.ID = Manager.Count;
                Manager.Add(layer);
            }
            else
            {
                while (Manager.Count <= layer.ID)
                {
                    Manager.Add(null);
                }

                Manager[layer.ID] = layer;
            }
        }

        public static void sortLayers()
        {
            for (int i = 0; i < Manager.Count; i++)
            {
                Manager[i].sort();
            }
        }
        #endregion

        public int ID { get; private set; } = -1;

        public RectTransform rectTransform
        {
            get { return (RectTransform)this.transform; }
        }

        protected override void preInit()
        {

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

        protected virtual void sort()
        {
            this.transform.SetSiblingIndex(ID);
            this.name = "Layer_" + ID;
        }

        public override void clear()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected override void onRefresh(TezRefreshPhase phase)
        {

        }

    }
}