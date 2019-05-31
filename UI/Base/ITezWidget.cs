using tezcat.Framework.Core;
using UnityEngine.EventSystems;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// 基础控件
    /// </summary>
    public interface ITezWidget : ITezCloseable
    {

    }

    /// <summary>
    /// 此控件可以接受鼠标焦点操作
    /// </summary>
    public interface ITezFocusableWidget
        : IPointerEnterHandler
        , IPointerExitHandler
    {

    }

    public interface ITezDropableWidgetNew
        : IDropHandler
    {

    }

    public interface ITezDragableWidgetNew
        : IBeginDragHandler
        , IEndDragHandler
        , IDragHandler
    {

    }

    /// <summary>
    /// 此控件可以被点击
    /// </summary>
    public interface ITezClickable
        : IPointerUpHandler
        , IPointerDownHandler
    {

    }

//     /// <summary>
//     /// 此控件可以接受事件
//     /// </summary>
//     public interface ITezEventHandler
//     {
//         TezWidgetEvent.Dispatcher m_EventSwitcher { get; }
//         void onEvent(int event_id, object data);
//     }
// 
//     /// <summary>
//     /// 此控件可以发送事件
//     /// </summary>
//     public interface ITezEventDispather
//     {
//         List<ITezEventHandler> handlers { get; }
//         void dispathEvent(int event_id, object data);
//     }
}