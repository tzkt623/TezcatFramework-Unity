using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezDatabaseSlot
        : TezToolWidget
        , ITezFocusableWidget
        , ITezClickableWidget
    {
        [SerializeField]
        Image m_Icon = null;

        public TezDatabaseItemContainer container { get; set; }


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

        protected override void onClose()
        {
//             m_Wrapper?.close();
//             m_Wrapper = null;
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

        private void refreshData()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

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
            this.refreshPhase = TezRefreshPhase.P_Custom1;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}

