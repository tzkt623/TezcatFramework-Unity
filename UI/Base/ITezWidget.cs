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

    public interface ITezDropableWidget
        : IDropHandler
    {

    }

    public interface ITezDragableWidget
        : IBeginDragHandler
        , IEndDragHandler
        , IDragHandler
    {

    }

    /// <summary>
    /// 此控件可以被点击
    /// </summary>
    public interface ITezClickableWidget
        : IPointerUpHandler
        , IPointerDownHandler
    {

    }
}