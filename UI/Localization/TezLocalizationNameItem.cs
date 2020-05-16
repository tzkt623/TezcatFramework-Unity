using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace tezcat.Framework.UI
{
    public class TezLocalizationNameItem
        : TezToolWidget
        , ITezClickableWidget
        , ITezSinglePrefab
    {
        [SerializeField]
        Text m_KeyName = null;
        [SerializeField]
        Text m_LocalizationName = null;
        [SerializeField]
        TezButton m_Edit = null;

        public TezLocalizationNameList listArea { get; set; }
        public string key
        {
            get { return m_KeyName.text; }
        }

        protected override void preInit()
        {
            m_Edit.onClick += onEditClick;
        }

        protected override void initWidget()
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
            m_Edit.onClick -= onEditClick;
            m_KeyName = null;
            m_LocalizationName = null;
            this.listArea = null;
        }

        private void refreshData()
        {
            string value = null;
            if (TezService.get<TezTranslator>().translateName(m_KeyName.text, out value))
            {
                m_LocalizationName.text = value;
            }
            else
            {
                m_LocalizationName.text = m_KeyName.text;
            }
        }

        private void onEditClick(TezButton button, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                listArea.edit(m_KeyName.text);
            }
        }

        public void set(string key)
        {
            m_KeyName.text = key;
            this.refreshPhase = TezRefreshPhase.Refresh;
        }

        public void set(string key, string value)
        {
            m_KeyName.text = key;
            m_LocalizationName.text = value;
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