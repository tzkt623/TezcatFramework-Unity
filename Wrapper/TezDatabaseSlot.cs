using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using tezcat.UI;
using UnityEngine.EventSystems;
using tezcat.Wrapper;
using tezcat.Utility;

namespace tezcat
{
    public class TezDatabaseSlot
        : TezWidget
        , ITezFocusableWidget
        , ITezDragableWidget
    {
        [SerializeField]
        Image m_Icon = null;

        public ITezStorageItemWrapper wrapper
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

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

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }
    }
}

