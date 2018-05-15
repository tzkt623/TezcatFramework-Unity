using tezcat.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace tezcat.UI
{
    public class TezLocalizationNameItem
        : TezWidget
        , ITezClickable
    {
        [SerializeField]
        Text m_KeyName = null;
        [SerializeField]
        Text m_LocalizationName = null;
        [SerializeField]
        TezImageButton m_Edit = null;

        public TezLocalizationNameList listArea { get; set; }

        public int index { get; private set; } = -1;

        private void onEditClick(PointerEventData.InputButton button)
        {
            listArea.edit(this, index);
        }

        public void set(int index)
        {
            this.index = index;
            this.dirty = true;
        }

        protected override void clear()
        {
            m_KeyName = null;
            m_LocalizationName = null;
            this.listArea = null;
        }

        protected override void onRefresh()
        {
            string key = null, value = null;
            if (TezLocalization.getName(index, out key, out value))
            {
                m_KeyName.text = key;
                m_LocalizationName.text = value;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.listArea.onFocus(this);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

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
    }
}