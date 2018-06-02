using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationDescriptionEditor : TezToolWidget
    {
        [SerializeField]
        InputField m_KeyInput = null;
        [SerializeField]
        InputField m_DescriptionInput = null;

        [SerializeField]
        TezImageLabelButton m_Confirm = null;
        [SerializeField]
        TezImageLabelButton m_Cancel = null;

        public TezLocalizationDescriptionList listArea { get; set; }

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
            if (TezTranslator.translateDescription(key, out value))
            {
                m_DescriptionInput.text = value;
            }
            else
            {
                TezTranslator.addDescription(key, value);
                this.dirty = true;
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

        private void onCancelClick(PointerEventData.InputButton button)
        {
            this.close();
        }

        private void onConfirmClick(PointerEventData.InputButton button)
        {
            TezTranslator.saveDescription(m_KeyInput.text, m_DescriptionInput.text);
            listArea.dirty = true;
            this.close();
        }

        protected override void onRefresh()
        {
            string value;
            if (TezTranslator.translateDescription(m_KeyInput.text, out value))
            {
                m_DescriptionInput.text = value;
            }
            else
            {
                m_DescriptionInput.text = m_KeyInput.text;
            }
        }
    }
}