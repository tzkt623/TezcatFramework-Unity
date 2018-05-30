﻿using System.Collections.Generic;

namespace tezcat.UI
{
    public class TezLayer : TezBasicWidget
    {
        #region Manater
        static List<TezLayer> Manager = new List<TezLayer>();

        public static int last
        {
            get { return Manager.Count - 1; }
        }

        public static TezLayer overlay { get; protected set; } = null;

        private static void register(TezLayer layer)
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
        #endregion

        public int ID { get; private set; } = -1;

        protected override void preInit()
        {
            register(this);
        }

        protected override void initWidget()
        {
            this.transform.SetSiblingIndex(ID);
            this.name = "Layer_" + ID;
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

        public override void clear()
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