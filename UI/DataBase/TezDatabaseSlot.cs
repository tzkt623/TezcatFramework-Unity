using tezcat.Core;
using tezcat.DataBase;
using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
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
            this.dirty = true;
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
            m_Wrapper?.close();
            m_Wrapper = null;
        }

        protected override void onRefresh()
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

                        TezStateController.add(TezStateSet.PickAnItem);
                        TezSelectController.select(m_Wrapper.mySlot);
                    }
                    else
                    {
                        container.onSelectSlot(this, m_Wrapper.getItem());
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
            TezDatabase.unregisterItem(m_Wrapper.getItem());
            m_Wrapper.close();
            m_Wrapper = null;
            this.dirty = true;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}

