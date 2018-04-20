using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat.UI
{
    public class TezLocEditor : TezWindow
    {
        TezLocMenu m_Menu = null;
        TezLocDescriptionEditor m_DescriptionEditor = null;
        TezLocNameEditor m_NameEditor = null;

        protected override void Awake()
        {
            base.Awake();

            m_Menu = this.GetComponentInChildren<TezLocMenu>();
            m_DescriptionEditor = this.GetComponentInChildren<TezLocDescriptionEditor>();
            m_NameEditor = this.GetComponentInChildren<TezLocNameEditor>();
        }

        protected override void Start()
        {
            base.Start();

            this.dirty = true;
        }


        protected override void onRefresh()
        {
            base.onRefresh();
        }
    }
}