﻿using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;
using tezcat.Framework.Event;
using tezcat.Framework.GraphicSystem;
using tezcat.Framework.Threading;
using tezcat.Framework.UI;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework
{
    public abstract class TezcatFramework
        : TezUIWidget
    {
        #region Instance
        static TezcatFramework m_Instance = null;
        public static TezcatFramework instance => m_Instance;
        #endregion

        #region Tools
        static TezGraphicSystem m_GraphicSystem = null;
        static TezPrefabDatabase m_PrefabDatabase = null;
        static TezThread m_Thread = null;
        static TezEventDispatcher m_EventDispatcher = null;
        static UnityKeyConfigSystem m_UnityKeyConfigSystem = null;

        public static TezGraphicSystem graphicSystem => m_GraphicSystem;
        public static TezPrefabDatabase prefabDatabase => m_PrefabDatabase;
        public static TezEventDispatcher eventDispatcher => m_EventDispatcher;
        public static TezThread thread => m_Thread;
        public static UnityKeyConfigSystem unityKeyConfigSystem => m_UnityKeyConfigSystem;
        #endregion

        #region Engine
        bool m_ResourceInited = false;
        protected override void preInit()
        {
            this.register();
            m_Instance = this;
        }

        protected override void initWidget()
        {
            foreach (RectTransform child in this.transform)
            {
                var layer = child.GetComponent<TezLayer>();
                if (layer != null)
                {
                    this.addLayer(layer);
                }
            }

            TezLayer.sortLayers();
        }

        protected override void onRefresh()
        {
            if (!m_ResourceInited)
            {
                m_ResourceInited = true;
                StartCoroutine(loadResources());
            }
        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }
        #endregion

        #region Register
        private void register()
        {
            this.registerVersions();
            this.registerService();
            this.registerClassFactory();
        }

        protected virtual void registerService()
        {
            m_GraphicSystem = new TezGraphicSystem();
            m_PrefabDatabase = new TezPrefabDatabase();
            m_Thread = new TezThread();
            m_EventDispatcher = new TezEventDispatcher();
            m_UnityKeyConfigSystem = new UnityKeyConfigSystem();
        }

        protected virtual void registerClassFactory()
        {

        }

        protected abstract void registerVersions();

        /// <summary>
        /// 注册管理UI
        /// </summary>
        public virtual void registerManagerWidget(TezUIWidget widget)
        {

        }
        #endregion

        #region Loading
        protected abstract IEnumerator onLoadResources();

        private IEnumerator loadResources()
        {
            yield return this.onLoadResources();
            yield return this.startMyGame();
        }

        public abstract IEnumerator startMyGame();
        #endregion

        #region Layer
        List<TezLayer> m_LayerList = new List<TezLayer>();
        Dictionary<string, int> m_LayerDic = new Dictionary<string, int>();

        public void addLayer(TezLayer layer)
        {
            TezLayer.register(layer);
        }
        #endregion

        #region Renderer
        public Renderer createRenderer<Renderer>(Transform parent)
            where Renderer : TezComBaseRenderer, ITezSinglePrefab
        {
            var prefab = m_PrefabDatabase.get<Renderer>();
            var go = MonoBehaviour.Instantiate(prefab, parent);
            return go;
        }

        public Renderer createRenderer<Renderer>(Transform parent, string prefab_name)
            where Renderer : TezComBaseRenderer, ITezMultiPrefab
        {
            var prefab = m_PrefabDatabase.get<Renderer>(prefab_name);
            var go = MonoBehaviour.Instantiate(prefab, parent);
            return go;
        }

        public GameMonoObject createGMO<GameMonoObject>(Transform parent)
            where GameMonoObject : TezGameMonoObject, ITezSinglePrefab
        {
            var prefab = m_PrefabDatabase.get<GameMonoObject>();
            var go = MonoBehaviour.Instantiate(prefab, parent);
            return go;
        }
        #endregion

        #region Window
        List<TezWindow> m_WindowList = new List<TezWindow>();
        Queue<int> m_FreeWindowID = new Queue<int>();

        Dictionary<Type, TezBaseWidget> m_WidgetWithType = new Dictionary<Type, TezBaseWidget>();

        private int giveID()
        {
            int id = -1;
            if (m_FreeWindowID.Count > 0)
            {
                id = m_FreeWindowID.Dequeue();
            }
            else
            {
                id = m_WindowList.Count;
                m_WindowList.Add(null);
            }
            return id;
        }

        /// <summary>
        /// 获得一个类型唯一的控件
        /// </summary>
        public Widget getTypeOnlyWidget<Widget>() where Widget : TezBaseWidget, ITezSinglePrefab
        {
            m_WidgetWithType.TryGetValue(typeof(Widget), out TezBaseWidget widget);
            return (Widget)widget;
        }

        /// <summary>
        /// 用Prefab创建一个Widget
        /// </summary>
        public TezBaseWidget createWidget(TezBaseWidget prefab, RectTransform parent, TezWidgetLife life)
        {
            TezBaseWidget widget = null;
            switch (life)
            {
                case TezWidgetLife.Normal:
                    widget = Instantiate(prefab, parent, false);
                    break;
                case TezWidgetLife.TypeOnly:
                    var type = prefab.GetType();
                    if (m_WidgetWithType.TryGetValue(type, out widget))
                    {
                        widget.reset();
                        return widget;
                    }
                    else
                    {
                        widget = Instantiate(prefab, parent, false);
                        m_WidgetWithType.Add(type, widget);
                    }
                    break;
                default:
                    break;
            }

            widget.life = life;
            return widget;
        }

        /// <summary>
        /// 创建一个Widget
        /// </summary>
        /// <typeparam name="Widget">类类型</typeparam>
        /// <param name="parent">父级</param>
        /// <param name="life">控件类型(普通类型,还是类型唯一类型)</param>
        /// <returns></returns>
        public Widget createWidget<Widget>(RectTransform parent, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return (Widget)this.createWidget(m_PrefabDatabase.get<Widget>(), parent, life);
        }

        public Widget createWidget<Widget>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return this.createWidget<Widget>(layer.rectTransform, life);
        }

        private Window createWindow<Window>(Window prefab
            , string name
            , TezLayer layer
            , TezWidgetLife life) where Window : TezWindow, ITezSinglePrefab
        {
            TezWindow window = null;

            switch (life)
            {
                case TezWidgetLife.Normal:
                    window = Instantiate(prefab, layer.transform, false);
                    break;
                case TezWidgetLife.TypeOnly:
                    TezBaseWidget widget = null;
                    var type = typeof(Window);
                    if (m_WidgetWithType.TryGetValue(type, out widget))
                    {
                        widget.reset();
                        return (Window)widget;
                    }
                    else
                    {
                        window = Instantiate(prefab, layer.transform, false);
                        m_WidgetWithType.Add(type, window);
                    }
                    break;
                default:
                    break;
            }

            int id = this.giveID();
            window.windowID = id;
            window.windowName = name;
            window.layer = layer;
            window.life = life;

            m_WindowList[id] = window;
            return (Window)window;
        }

        public Window createWindow<Window>(string name, TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return this.createWindow(m_PrefabDatabase.get<Window>(), name, layer, life);
        }

        public TezWindow createWindow(TezWindow prefab, TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal)
        {
            return this.createWindow(prefab, prefab.GetType().Name, layer, life);
        }

        public Window createWindow<Window>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return this.createWindow<Window>(typeof(Window).Name, layer, life);
        }

        public void removeWindow(TezWindow window)
        {
            m_FreeWindowID.Enqueue(window.windowID);
            m_WindowList[window.windowID] = null;
        }

        public void removeTypeOnlyWidget(TezBaseWidget widget)
        {
            m_WidgetWithType.Remove(widget.GetType());
        }
        #endregion

        #region Refresh
        Queue<ITezRefreshHandler> m_RefreshQueue = new Queue<ITezRefreshHandler>();

        public void pushRefreshHandler(ITezRefreshHandler handler)
        {
            m_RefreshQueue.Enqueue(handler);
        }
        #endregion

        protected virtual void Update() { }

        protected virtual void LateUpdate()
        {
            while (m_RefreshQueue.Count > 0)
            {
                m_RefreshQueue.Dequeue().refresh();
            }
        }
    }
}