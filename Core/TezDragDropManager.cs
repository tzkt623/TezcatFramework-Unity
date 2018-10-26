using tezcat.Framework.Extension;
using UnityEngine.EventSystems;

namespace tezcat.Framework.Core
{
    public class TezDragDropManager : ITezService
    {
        public interface IVirtualObject
        {
            void onBeginDrag(PointerEventData event_data);
            void onDragging(PointerEventData event_data);
            void onEndDrag(bool success, PointerEventData event_data);
        }

        IVirtualObject m_VirtualObject = null;
        TezEventExtension.Function<bool, object> m_DropFunction = null;
        object m_DragObject = null;


        public TezDragDropManager()
        {
            m_DropFunction = defaultDropFunction;
        }

        private bool defaultDropFunction(object obj)
        {
            return false;
        }

        public void setVirtualObject(IVirtualObject obj)
        {
            m_VirtualObject = obj;
        }

        public void beginDrag(PointerEventData event_data, object obj)
        {
            m_DragObject = obj;
            m_VirtualObject.onBeginDrag(event_data);
        }

        public void dragging(PointerEventData event_data)
        {
            m_VirtualObject.onDragging(event_data);
        }

        public void endDrag(PointerEventData event_data)
        {
            m_VirtualObject.onEndDrag(m_DropFunction(m_DragObject), event_data);
            m_DropFunction = defaultDropFunction;
        }

        public void drop(TezEventExtension.Function<bool, object> function)
        {
            m_DropFunction = function;
        }

        public void close()
        {

        }
    }
}