﻿using tezcat.Framework.Core;

namespace tezcat.Framework.UI
{
    public class TezLocalizationWindow : TezToolWindow
    {
        TezLocalizationMenu m_Menu = null;
        TezLocalizationNameList m_NameList = null;
        TezLocalizationDescriptionList m_DescriptionList = null;

        protected override void preInit()
        {
            base.preInit();

            m_Menu = this.GetComponentInChildren<TezLocalizationMenu>();
            m_NameList = this.GetComponentInChildren<TezLocalizationNameList>();
            m_DescriptionList = this.GetComponentInChildren<TezLocalizationDescriptionList>();

            m_Menu.nameList = m_NameList;
            m_Menu.descriptionList = m_DescriptionList;
        }

        protected override void onClose(bool self_close)
        {
            m_Menu = null;
            m_NameList = null;
            m_DescriptionList = null;

            base.onClose(self_close);
            TezService.get<TezcatFramework>().createWindow<TezcatToolWindow>("TezcatToolWindow", TezLayer.last).open();
        }
    }
}