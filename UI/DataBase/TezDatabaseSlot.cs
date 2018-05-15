using tezcat.Core;
using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezDatabaseSlot
        : TezWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        [SerializeField]
        Image m_Icon = null;

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

        protected override void clear()
        {
            m_Wrapper?.clear();
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
                    TezStateController.add(TezBuildInState.PickAnItem);
                    TezSelectController.select(new TezItemSelector(m_Wrapper.mySlot));
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {

        }
    }
}

