using tezcat.Core;
using tezcat.DataBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLocalizationDescriptionEditor
        : TezToolWidget
        , ITezPrefab
    {
        [SerializeField]
        InputField m_KeyInput = null;
        [SerializeField]
        InputField m_DescriptionInput = null;

        [SerializeField]
        TezLabelButtonWithBG m_Confirm = null;
        [SerializeField]
        TezLabelButtonWithBG m_Cancel = null;

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

        protected override void onOpenAndRefresh()
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
                this.refresh = RefreshPhase.Custom3;
            }
        }

        public void set(string key)
        {
            m_KeyInput.readOnly = true;
            m_KeyInput.text = key;
            this.refresh = RefreshPhase.Custom3;
        }

        public void newItem()
        {
            m_KeyInput.readOnly = false;
        }

        private void onCancelClick(TezButton button, PointerEventData eventData)
        {
            this.close();
        }

        private void onConfirmClick(TezButton button, PointerEventData eventData)
        {
            TezTranslator.saveDescription(m_KeyInput.text, m_DescriptionInput.text);
            listArea.refresh = RefreshPhase.Custom3;
            this.close();
        }

        protected override void refreshAfterInit()
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