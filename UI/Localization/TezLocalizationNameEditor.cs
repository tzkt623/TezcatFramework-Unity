using tezcat.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationNameEditor : TezToolWidget
    {
        [SerializeField]
        InputField m_KeyInput = null;
        [SerializeField]
        InputField m_LocalizationInput = null;

        [SerializeField]
        TezLabelButtonWithBG m_Confirm = null;
        [SerializeField]
        TezLabelButtonWithBG m_Cancel = null;

        public TezLocalizationNameList listArea { get; set; }

        protected override void preInit()
        {
            m_Confirm.onClick += onConfirmClick;
            m_Cancel.onClick += onCancelClick;

            m_KeyInput.onEndEdit.AddListener(this.checkKey);
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

        public override void clear()
        {
            m_Confirm.onClick -= onConfirmClick;
            m_Cancel.onClick -= onCancelClick;

            m_KeyInput.onEndEdit.RemoveListener(this.checkKey);

            listArea = null;
        }

        private void checkKey(string key)
        {
            string value;
            if (TezTranslator.translateName(key, out value))
            {
                m_LocalizationInput.text = value;
            }
        }

        public void set(string key)
        {
            m_KeyInput.readOnly = true;
            m_KeyInput.text = key;
            this.dirty = true;
        }

        public void newItem()
        {
            m_KeyInput.readOnly = false;
        }

        private void onCancelClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.close();
            }
        }

        private void onConfirmClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                TezTranslator.saveName(m_KeyInput.text, m_LocalizationInput.text);
                listArea.dirty = true;
                this.close();
            }
        }

        protected override void onRefresh()
        {
            string value;
            if (TezTranslator.translateName(m_KeyInput.text, out value))
            {
                m_LocalizationInput.text = value;
            }
            else
            {
                m_LocalizationInput.text = m_KeyInput.text;
            }
        }
    }
}