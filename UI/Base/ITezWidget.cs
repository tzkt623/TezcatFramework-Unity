using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.Wrapper;
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
    /// 此控件可以接受Drop操作
    /// </summary>
    public interface ITezDropableWidget
    {
        void onDrop(PointerEventData event_data);

        /// <summary>
        /// 检测一个Item是否能Drop,并设置Drop的方法
        /// </summary>
        /// <param name="wrapper">DropItem的包装器</param>
        /// <param name="event_data">当前的Pointer数据</param>
        /// <returns>DropItem的方法,如果为null,则表示不能Drop</returns>
        TezEventExtension.Action<ITezWrapper> checkDrop(ITezWrapper wrapper, PointerEventData event_data);
    }

    /// <summary>
    /// 此控件可以被拖拽
    /// </summary>
    public interface ITezDragableWidget
        : IBeginDragHandler
        , IEndDragHandler
        , IDragHandler
    {

    }

    /// <summary>
    /// 此控件中的Item可以被拖拽
    /// </summary>
    public interface ITezDragableItemWidget
        : IBeginDragHandler
        , IEndDragHandler
        , IDragHandler
    {
        /// <summary>
        /// 物品的包装器
        /// </summary>
        ITezItemWrapper wrapper { get; }
    }

    /// <summary>
    /// 此控件可以被点击
    /// </summary>
    public interface ITezClickable
        : IPointerUpHandler
        , IPointerDownHandler
    {

    }

    /// <summary>
    /// 此控件可以接受事件
    /// </summary>
    public interface ITezEventHandler
    {
        TezWidgetEvent.Dispatcher m_EventSwitcher { get; }
        void onEvent(int event_id, object data);
    }

    /// <summary>
    /// 此控件可以发送事件
    /// </summary>
    public interface ITezEventDispather
    {
        List<ITezEventHandler> handlers { get; }
        void dispathEvent(int event_id, object data);
    }
}