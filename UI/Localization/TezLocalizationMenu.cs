﻿using tezcat.Core;
using tezcat.DataBase;
using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezLocalizationMenu : TezArea
    {
        [SerializeField]
        TezLabelButtonWithBG m_Refresh = null;
        [SerializeField]
        TezLabelButtonWithBG m_Save = null;

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
                TezTranslator.serialization(writer);
                writer.save(TezcatFramework.localizationPath);
            }
        }

        private void onRefreshClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.nameList.refresh = RefreshPhase.Custom3;
                this.descriptionList.refresh = RefreshPhase.Custom3;
            }
        }

        protected override void refreshAfterInit()
        {

        }

        protected override void onOpenAndRefresh()
        {

        }

        protected override void onHide()
        {

        }
    }
}