using tezcat.Framework.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezDatabaseSlot
        : TezToolWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        [SerializeField]
        Image m_Icon = null;

        public TezDatabaseItemContainer container { get; set; }

        TezDatabaseItemWrapper m_Wrapper = null;

        public void bind(TezDatabaseItemWrapper wrapper)
        {
            m_Wrapper = wrapper;
            this.refresh = RefreshPhase.Custom1;
        }

        protected override void preInit()
        {

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

        public override void clear()
        {
//             m_Wrapper?.close();
//             m_Wrapper = null;
        }

        protected override void onRefresh(RefreshPhase phase)
        {
            switch (phase)
            {
                case RefreshPhase.OnInit:
                    this.refreshData();
                    break;
                case RefreshPhase.OnEnable:
                    break;
                default:
                    break;
            }
        }

        private void refreshData()
        {
            if (m_Wrapper != null)
            {
                m_Icon.gameObject.SetActive(true);
                m_Icon.sprite = m_Wrapper.getIcon();
            }
            else
            {
                m_Icon.gameObject.SetActive(false);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_Wrapper?.showTip();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_Wrapper?.hideTip();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if(eventData.clickCount == 2)
                    {

//                         TezStateController.add(TezStateSet.PickAnItem);
//                         TezSelectController.select(m_Wrapper.mySlot);
                    }
                    else
                    {
//                        container.onSelectSlot(this, m_Wrapper.myItem);
                    }
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
            }
        }

        public void removeItem()
        {
//             TezService.DB.unregisterItem(m_Wrapper.myItem);
//             m_Wrapper.close();
//             m_Wrapper = null;
            this.refresh = RefreshPhase.Custom1;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}

