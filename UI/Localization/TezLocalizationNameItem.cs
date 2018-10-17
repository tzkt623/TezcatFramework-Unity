using tezcat.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace tezcat.UI
{
    public class TezLocalizationNameItem
        : TezToolWidget
        , ITezClickable
    {
        [SerializeField]
        Text m_KeyName = null;
        [SerializeField]
        Text m_LocalizationName = null;
        [SerializeField]
        TezImageButton m_Edit = null;

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
            m_Edit.onClick -= onEditClick;
            m_KeyName = null;
            m_LocalizationName = null;
            this.listArea = null;
        }

        protected override void onRefresh()
        {
            string value = null;
            if (TezTranslator.translateName(m_KeyName.text, out value))
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
            this.dirty = true;
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