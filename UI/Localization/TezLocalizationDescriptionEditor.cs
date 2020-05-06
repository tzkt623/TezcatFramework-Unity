using tezcat.Framework.Core;
using tezcat.Framework.Database;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLocalizationDescriptionEditor
        : TezToolWidget
        , ITezSinglePrefab
    {
        [SerializeField]
        InputField m_KeyInput = null;
        [SerializeField]
        InputField m_DescriptionInput = null;

        [SerializeField]
        TezButton m_Confirm = null;
        [SerializeField]
        TezButton m_Cancel = null;

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

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected override void onClose(bool self_close = true)
        {
            m_Confirm.onClick -= onConfirmClick;
            m_Cancel.onClick -= onCancelClick;

            m_KeyInput.onEndEdit.RemoveListener(this.checkKey);

            listArea = null;
        }

        private void checkKey(string key)
        {
            string value;
            if (TezService.get<TezTranslator>().translateDescription(key, out value))
            {
                m_DescriptionInput.text = value;
            }
            else
            {
                TezService.get<TezTranslator>().addDescription(key, value);
                this.refreshPhase = TezRefreshPhase.P_Custom3;
            }
        }

        public void set(string key)
        {
            m_KeyInput.readOnly = true;
            m_KeyInput.text = key;
            this.refreshPhase = TezRefreshPhase.P_Custom3;
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
            TezService.get<TezTranslator>().saveDescription(m_KeyInput.text, m_DescriptionInput.text);
            listArea.refreshPhase = TezRefreshPhase.P_Custom3;
            this.close();
        }

        private void refreshData()
        {
            string value;
            if (TezService.get<TezTranslator>().translateDescription(m_KeyInput.text, out value))
            {
                m_DescriptionInput.text = value;
            }
            else
            {
                m_DescriptionInput.text = m_KeyInput.text;
            }
        }

        protected override void onRefresh(TezRefreshPhase phase)
        {
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    this.refreshData();
                    break;
                case TezRefreshPhase.P_OnEnable:
                    break;
                default:
                    break;
            }
        }
    }
}