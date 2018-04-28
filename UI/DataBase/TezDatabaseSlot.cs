using tezcat.Utility;
using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezDatabaseSlot
        : TezWidget
        , ITezFocusableWidget
        , ITezDragableItemWidget
        , ITezWrapperBinder<TezDatabaseItemWrapper>
    {
        [SerializeField]
        Image m_Icon = null;

        TezDatabaseItemWrapper m_Wrapper = null;
        public ITezItemWrapper wrapper
        {
            get { return m_Wrapper; }
        }

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

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (wrapper != null)
                    {
                        TezDragDropManager.beginDragItem(this);
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

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    TezDragDropManager.draggingItem(eventData);
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    TezDragDropManager.endDragItem(eventData);
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    break;
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
    }
}

