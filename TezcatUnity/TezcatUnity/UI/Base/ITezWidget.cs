using tezcat.Framework.Core;
using tezcat.Unity.Core;
using tezcat.Unity.Database;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.Unity.UI
{



    /// <summary>
    /// 基础控件
    /// </summary>
    public interface ITezBaseWidget
        : ITezCloseable
        , ITezPrefab
    {
        RectTransform rectTransform { get; }
        void show();
        void hide();
    }

    public interface ITezUIWidget
        : ITezBaseWidget
        //, ITezDelayInitHandler
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