using tezcat.Framework.Core;
using tezcat.Framework.Database;
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
            if (TezService.get<TezTranslator>().translateName(m_KeyInput.text, out value))
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

        protected override void onClose()
        {
            m_Confirm.onClick -= onConfirmClick;
            m_Cancel.onClick -= onCancelClick;

            m_KeyInput.onEndEdit.RemoveListener(this.checkKey);

            listArea = null;
        }

        private void checkKey(string key)
        {
            string value;
            if (TezService.get<TezTranslator>().translateName(key, out value))
            {
                m_LocalizationInput.text = value;
            }
        }

        public void set(string key)
        {
            m_KeyInput.readOnly = true;
            m_KeyInput.text = key;
            this.refreshPhase = TezRefreshPhase.P_Custom1;
        }

        protected override void onRefresh(TezRefreshPhase phase)
        {
            switch (phase)
            {
                case TezRefreshPhase.P_Custom1:
                    this.refreshData();
                    break;
                case TezRefreshPhase.P_Custom2:
                    break;
                case TezRefreshPhase.P_Custom3:
                    break;
                case TezRefreshPhase.P_Custom4:
                    break;
                case TezRefreshPhase.P_Custom5:
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
                TezService.get<TezTranslator>().saveName(m_KeyInput.text, m_LocalizationInput.text);
                listArea.refreshPhase = TezRefreshPhase.P_Custom3;
                this.close();
            }
        }
    }
}