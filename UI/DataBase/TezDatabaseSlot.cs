using tezcat.Core;
using tezcat.ShipProject;
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
                    TezStateController.add(GameState.PickAnItem);
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

        protected override void preInit()
        {
            throw new System.NotImplementedException();
        }

        protected override void initWidget()
        {
            throw new System.NotImplementedException();
        }

        protected override void linkEvent()
        {
            throw new System.NotImplementedException();
        }

        protected override void unLinkEvent()
        {
            throw new System.NotImplementedException();
        }

        protected override void onShow()
        {
            throw new System.NotImplementedException();
        }

        protected override void onHide()
        {
            throw new System.NotImplementedException();
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }
    }
}

