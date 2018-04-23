﻿using UnityEngine;
using UnityEngine.EventSystems;

using tezcat.Utility;
using tezcat.Serialization;
namespace tezcat.UI
{
    public class TezLocalizationMenu : TezArea
    {
        [SerializeField]
        TezImageLabelButton m_LoadProperty = null;

        [SerializeField]
        TezImageLabelButton m_Refresh = null;
        [SerializeField]
        TezImageLabelButton m_Save = null;

        public TezLocalizationNameList nameList { get; set; }
        public TezLocalizationDescriptionList descriptionList { get; set; }

        protected override void Awake()
        {
            base.Awake();

            m_Refresh.onClick += onRefreshClick;
            m_Save.onClick += onSaveClick;

            m_LoadProperty.onClick += onLoadPropertyClick;
        }

        private void onLoadPropertyClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                TezPropertyManager.foreachProperty((TezPropertyName name) =>
                {
                    TezLocalization.tryAddName(name.key_name, name.key_name);
                });

                this.nameList.dirty = true;
            }
        }

        private void onSaveClick(PointerEventData.InputButton button)
        {
            if(button == PointerEventData.InputButton.Left)
            {
                TezJsonWriter writer = new TezJsonWriter();
                TezLocalization.serialization(writer);
                writer.save(TezcatFramework.localizationPath);
            }
        }

        private void onRefreshClick(PointerEventData.InputButton button)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                this.nameList.dirty = true;
                this.descriptionList.dirty = true;
            }
        }

        protected override void onRefresh()
        {

        }
    }
}