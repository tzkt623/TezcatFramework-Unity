﻿using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    public class TezLocalizationMenu : TezSubwindow
    {
        [SerializeField]
        TezButton m_Refresh = null;
        [SerializeField]
        TezButton m_Save = null;

        public TezLocalizationNameList nameList { get; set; }
        public TezLocalizationDescriptionList descriptionList { get; set; }

        protected override void preInit()
        {
            base.preInit();
            m_Refresh.onClick += onRefreshClick;
            m_Save.onClick += onSaveClick;
        }

        private void onSaveClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                TezJsonWriter writer = new TezJsonWriter();
//                TezService.get<TezTranslator>().serialization(writer);
            }
        }

        private void onRefreshClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.nameList.refreshPhase = TezRefreshPhase.Refresh;
                this.descriptionList.refreshPhase = TezRefreshPhase.Refresh;
            }
        }

        protected override void onHide()
        {

        }
    }
}