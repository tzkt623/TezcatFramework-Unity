using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLocalizationDescriptionItem
        : TezToolWidget
        , IPointerEnterHandler
        , IPointerExitHandler
        , ITezClickableWidget
        , ITezSinglePrefab
    {
        [SerializeField]
        TezButton m_Edit = null;

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

        protected override void onClose(bool self_close)
        {
            m_Flag = null;
            m_KeyName = null;
            listArea = null;
        }

        private void onEditClick(TezButton button, PointerEventData eventData)
        {
            listArea.editItem(m_KeyName.text);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
//             string value = null;
//             if (TezService.get<TezTranslator>().translateDescription(m_KeyName.text, out value))
//             {
//                 TezService.get<TezTipController>()
//                     .setDescription(value)
//                     .show();
//             }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            TezService.get<TezTipController>().hide();
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