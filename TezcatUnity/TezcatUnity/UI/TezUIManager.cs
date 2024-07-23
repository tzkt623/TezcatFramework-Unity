using System;
using System.Collections.Generic;
using tezcat.Unity.Database;
using UnityEngine;

namespace tezcat.Unity.UI
{
    public static class TezUIManager
    {
        #region Window
        static List<TezWindow> sWindowList = new List<TezWindow>();
        static Queue<int> sFreeWindowID = new Queue<int>();
        static Dictionary<Type, TezBaseWidget> sWidgetWithType = new Dictionary<Type, TezBaseWidget>();

        private static int giveID()
        {
            int id = -1;
            if (sFreeWindowID.Count > 0)
            {
                id = sFreeWindowID.Dequeue();
            }
            else
            {
                id = sWindowList.Count;
                sWindowList.Add(null);
            }
            return id;
        }

        /// <summary>
        /// 获得一个类型唯一的控件
        /// </summary>
        public static Widget getTypeOnlyWidget<Widget>() where Widget : TezBaseWidget, ITezSinglePrefab
        {
            sWidgetWithType.TryGetValue(typeof(Widget), out TezBaseWidget widget);
            return (Widget)widget;
        }

        /// <summary>
        /// 用Prefab创建一个Widget
        /// </summary>
        public static TezBaseWidget createWidget(TezBaseWidget prefab, RectTransform parent, TezWidgetLife life)
        {
            TezBaseWidget widget = null;
            switch (life)
            {
                case TezWidgetLife.Normal:
                    widget = UnityEngine.Object.Instantiate(prefab, parent, false);
                    break;
                case TezWidgetLife.TypeOnly:
                    var type = prefab.GetType();
                    if (sWidgetWithType.TryGetValue(type, out widget))
                    {
                        widget.reset();
                        return widget;
                    }
                    else
                    {
                        widget = UnityEngine.Object.Instantiate(prefab, parent, false);
                        sWidgetWithType.Add(type, widget);
                    }
                    break;
                default:
                    break;
            }

            widget.life = life;
            return widget;
        }

        public static void addWindow(TezWindow window)
        {
            int id = giveID();
            window.windowID = id;
            sWindowList[id] = window;
        }

        /// <summary>
        /// 创建一个Widget
        /// </summary>
        /// <typeparam name="Widget">类类型</typeparam>
        /// <param name="parent">父级</param>
        /// <param name="life">控件类型(普通类型,还是类型唯一类型)</param>
        /// <returns></returns>
        public static Widget createWidget<Widget>(RectTransform parent, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return (Widget)createWidget(TezPrefabDatabase.get<Widget>(), parent, life);
        }

        public static Widget createWidget<Widget>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return createWidget<Widget>(layer.rectTransform, life);
        }

        private static Window createWindow<Window>(Window prefab
            , TezLayer layer
            , TezWidgetLife life) where Window : TezWindow, ITezSinglePrefab
        {
            TezWindow window = null;

            switch (life)
            {
                case TezWidgetLife.Normal:
                    window = UnityEngine.Object.Instantiate(prefab, layer.transform, false);
                    break;
                case TezWidgetLife.TypeOnly:
                    TezBaseWidget widget = null;
                    var type = typeof(Window);
                    if (sWidgetWithType.TryGetValue(type, out widget))
                    {
                        widget.reset();
                        return (Window)widget;
                    }
                    else
                    {
                        window = UnityEngine.Object.Instantiate(prefab, layer.transform, false);
                        sWidgetWithType.Add(type, window);
                    }
                    break;
                default:
                    break;
            }


            window.layer = layer;
            window.life = life;

            return (Window)window;
        }

        public static Window createWindow<Window>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return createWindow(TezPrefabDatabase.get<Window>(), layer, life);
        }

        public static TezWindow createWindow(TezWindow prefab, TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal)
        {
            return createWindow(prefab, layer, life);
        }

        public static void removeWindow(TezWindow window)
        {
            Debug.Log($"removeWindow:{window.windowID}");
            sFreeWindowID.Enqueue(window.windowID);
            sWindowList[window.windowID] = null;
        }

        public static void removeTypeOnlyWidget(TezBaseWidget widget)
        {
            sWidgetWithType.Remove(widget.GetType());
        }
        #endregion
    }
}