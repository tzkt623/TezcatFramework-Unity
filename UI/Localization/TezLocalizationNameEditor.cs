using tezcat.Framework.Core;
using tezcat.Framework.DataBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLocalizationNameEditor
        : TezToolWidget
        , ITezPrefab
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

        private void refreshData()
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
            this.refresh = RefreshPhase.Custom1;
        }

        protected override void onRefresh(RefreshPhase phase)
        {
            switch (phase)
            {
                case RefreshPhase.Custom1:
                    this.refreshData();
                    break;
                case RefreshPhase.Custom2:
                    break;
                case RefreshPhase.Custom3:
                    break;
                case RefreshPhase.Custom4:
                    break;
                case RefreshPhase.Custom5:
                    break;
                default:
                    break;
            }
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
                listArea.refresh = RefreshPhase.Custom3;
                this.close();
            }
        }
    }
}