using tezcat.Signal;
using tezcat.UI;
using tezcat.Wrapper;
using UnityEngine.EventSystems;

namespace tezcat.Utility
{
    public class TezDragDropManager
    {
        static bool m_Drag = false;

        public interface VirtualItem
        {
            void onBeginDrag(ITezItemWrapper info);
            void onDragging(PointerEventData event_data);
            void onEndDrag();
        }


        static VirtualItem m_CurrentVirtualItem = null;
        static ITezItemWrapper m_DragInfo = null;
        static ITezDropableWidget m_Slot = null;

        static TezEventBus.Action<ITezItemWrapper> m_ItemDropFunction = null;

        public static void setVirtualItem(VirtualItem item)
        {
            m_CurrentVirtualItem = item;
        }

        public static void beginDragItem(ITezDragableItemWidget slot)
        {
            m_DragInfo = slot.wrapper;
            m_Drag = true;
            m_CurrentVirtualItem.onBeginDrag(m_DragInfo);
        }

        public static void draggingItem(PointerEventData event_data)
        {
            if (m_Drag)
            {
                m_CurrentVirtualItem.onDragging(event_data);
            }
        }

        public static void endDragItem(PointerEventData eventData)
        {
            if (m_Drag)
            {
                m_ItemDropFunction?.Invoke(m_DragInfo);
                m_ItemDropFunction = null;

                m_DragInfo = null;
                m_Drag = false;

                m_CurrentVirtualItem.onEndDrag();
            }
        }

        public static void dropItem(ITezDropableWidget widget, PointerEventData eventData)
        {
            if (m_Drag)
            {
                m_ItemDropFunction = widget.checkItemToDrop(m_DragInfo, eventData);
            }
        }
    }
}