﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using tezcat.Utility;
namespace tezcat.UI
{
    public class TezLocalizationDescriptionItem
        : TezWidget
        , IPointerEnterHandler
        , IPointerExitHandler
        , ITezClickable
    {
        [SerializeField]
        TezImageButton m_Edit = null;

        public TezLocalizationDescriptionList listArea { get; set; }

        Image m_Flag = null;
        Text m_KeyName = null;
        public string key
        {
            get { return m_KeyName.text; }
        }

        protected override void preInit()
        {
            m_Flag = this.GetComponentInChildren<Image>();
            m_KeyName = this.GetComponentInChildren<Text>();

            m_Edit.onClick += onEditClick;
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

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        public void set(string key)
        {
            m_KeyName.text = key;
        }

        public override void clear()
        {
            m_Flag = null;
            m_KeyName = null;
            listArea = null;
        }

        private void onEditClick(PointerEventData.InputButton button)
        {
            listArea.editItem(m_KeyName.text);
        }

        protected override void onRefresh()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            string value = null;
            if (TezTranslater.translateDescription(m_KeyName.text, out value))
            {
                TezTipManager.instance
                    .setDescription(value)
                    .show();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            TezTipManager.instance.hide();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.listArea.onFocus(this);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}