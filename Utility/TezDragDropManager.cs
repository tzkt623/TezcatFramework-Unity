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
            void onBeginDrag(ITezWrapper info);
            void onDragging(PointerEventData event_data);
            void onEndDrag();
        }

        static VirtualItem m_CurrentVirtualItem = null;
        static ITezWrapper m_DragInfo = null;

        static TezEventCenter.Action<ITezWrapper> m_ItemDropFunction = null;

        public static void setVirtualItem(VirtualItem item)
        {
            m_CurrentVirtualItem = item;
        }

        public static void beginDrag(ITezWrapper wrapper)
        {
            m_DragInfo = wrapper;
            m_Drag = true;
            m_CurrentVirtualItem.onBeginDrag(m_DragInfo);
        }

        public static void beginDrag(ITezDragableItemWidget slot)
        {
            m_DragInfo = slot.wrapper;
            m_Drag = true;
            m_CurrentVirtualItem.onBeginDrag(m_DragInfo);
        }

        public static void dragging(PointerEventData event_data)
        {
            if (m_Drag)
            {
                m_CurrentVirtualItem.onDragging(event_data);
            }
        }

        public static bool endDrag(PointerEventData eventData)
        {
            if (m_Drag)
            {
                m_ItemDropFunction?.Invoke(m_DragInfo);
                m_ItemDropFunction = null;

                m_DragInfo = null;
                m_Drag = false;

                m_CurrentVirtualItem.onEndDrag();

                return true;
            }

            return false;
        }

        public static void drop(PointerEventData eventData, TezEventCenter.Action<ITezWrapper> drop)
        {
            if (m_Drag)
            {
                drop(m_DragInfo);
            }
        }

        public static void drop(ITezDropableWidget widget, PointerEventData eventData)
        {
            if (m_Drag)
            {
                m_ItemDropFunction = widget.checkDrop(m_DragInfo, eventData);
            }
        }
    }
}