using tezcat.Utility;
using tezcat.Wrapper;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezStorageSlot
        : TezWidget
        , ITezWrapperBinder<ITezItemWrapper>
        , ITezFocusableWidget
        , ITezDragableWidget
        , ITezDropableWidget

    {
        TezStorageItem m_StorageItem = null;
        public virtual ITezItemWrapper wrapper { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            m_StorageItem = this.GetComponent<TezStorageItem>();
        }

        public void bind(ITezItemWrapper wrapper)
        {
            if (wrapper.count <= 0)
            {
                this.wrapper = null;
            }
            else
            {
                this.wrapper.clear();
                this.wrapper = wrapper;
            }

            this.dirty = true;
        }

        protected override void OnDestroy()
        {
            wrapper.clear();
        }

        protected override void clear()
        {
            wrapper.clear();
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
            wrapper?.showTip();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            wrapper?.hideTip();
        }

        void ITezDropableWidget.onDrop(PointerEventData eventData)
        {
            TezDragDropManager.dropItem(this, eventData);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="event_data"></param>
        /// <returns></returns>
        public virtual TezEventBus.Action<ITezItemWrapper> checkItemToDrop(ITezItemWrapper wrapper, PointerEventData event_data)
        {
            return null;
        }

        protected override void onRefresh()
        {

        }
    }
}