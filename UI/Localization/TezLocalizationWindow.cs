using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat.UI
{
    public class TezLocalizationWindow : TezWindow
    {
        TezLocalizationMenu m_Menu = null;
        TezLocalizationDescriptionEditor m_DescriptionEditor = null;
        TezLocalizationNameEditor m_NameEditor = null;

        protected override void Awake()
        {
            base.Awake();

            m_Menu = this.GetComponentInChildren<TezLocalizationMenu>();
            m_DescriptionEditor = this.GetComponentInChildren<TezLocalizationDescriptionEditor>();
            m_NameEditor = this.GetComponentInChildren<TezLocalizationNameEditor>();
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