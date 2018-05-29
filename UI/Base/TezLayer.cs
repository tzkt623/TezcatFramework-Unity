using System.Collections.Generic;
using UnityEngine;

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

        private static void register(TezLayer layer)
        {
            if(layer.ID == -1)
            {
                layer.m_ID = Manager.Count;
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

        [SerializeField]
        int m_ID = -1;
        public int ID
        {
            get { return m_ID; }
        }

        protected override void preInit()
        {
            register(this);
        }

        protected override void initWidget()
        {
            this.transform.SetSiblingIndex(m_ID);
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