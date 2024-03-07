using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Unity.Core;
using tezcat.Unity.Database;
using tezcat.Unity.GraphicSystem;
using tezcat.Unity.UI;
using tezcat.Unity.Utility;
using UnityEngine;

namespace tezcat.Unity
{
    public abstract class TezcatUnity
        : TezUIWidget
    {
        #region Engine
        static TezcatUnity mInstance = null;
        public static TezcatUnity instance => mInstance;

        bool mResourceInited = false;
        protected override void preInit()
        {
            if (mInstance != null)
            {
                Application.Quit();
            }

            mInstance = this;
            this.register();
        }

        protected override void initWidget()
        {
            foreach (RectTransform child in this.transform)
            {
                var layer = child.GetComponent<TezLayer>();
                if (layer != null)
                {
                    TezLayer.register(layer);
                }
            }

            TezLayer.sortLayers();
        }

        protected override void onDelayInit()
        {
            if (!mResourceInited)
            {
                mResourceInited = true;
                StartCoroutine(startGame());
            }
        }

        protected override void onRefresh()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected virtual void Update() { }

        protected virtual void LateUpdate()
        {
            while (sInitQueue.Count > 0)
            {
                sInitQueue.Dequeue().delayInit();
            }

            while (sUIRefreshQueue.Count > 0)
            {
                sUIRefreshQueue.Dequeue().refresh();
            }

            while (sRendererRefreshQueue.Count > 0)
            {
                sRendererRefreshQueue.Dequeue().refresh();
            }
        }
        #endregion

        #region Register
        private void register()
        {
            TezFilePath.setUnityPath(Application.dataPath);

            this.beginRegister();
            this.registerVersions();
            this.registerService();
            this.registerFramework();

            this.registerComponent();
            this.registerClassFactory();
            this.endRegister();
        }

        protected virtual void beginRegister()
        {

        }

        protected virtual void endRegister()
        {

        }

        protected virtual void registerService()
        {
            sUnityKeyConfigSystem = new UnityKeyConfigSystem();
            sGraphicSystem = new TezGraphicSystem();
            sPrefabDatabase = new TezPrefabDatabase();
        }

        /// <summary>
        /// 必须要对framework里的工具进行初始化
        /// </summary>
        protected virtual void registerFramework()
        {
            TezcatFramework.set(new TezItemDatabase());
            TezcatFramework.set(new TezRunTimeDatabase());
        }

        protected virtual void registerClassFactory()
        {

        }

        protected virtual void registerComponent()
        {
            //            TezDataComponent.SComUID = TezComponentManager.register<TezDataComponent>();
            //            TezInfoComponent.SComUID = TezComponentManager.register<TezInfoComponent>();
            //            TezRendererComponent.SComUID = TezComponentManager.register<TezRendererComponent>();
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
        private IEnumerator startGame()
        {
            yield return this.loadResources();
            yield return this.startMyGame();
        }

        protected abstract IEnumerator loadResources();

        protected abstract IEnumerator startMyGame();
        #endregion

        #region Renderer
        public static Renderer createRenderer<Renderer>(Transform parent)
            where Renderer : MonoBehaviour, ITezSinglePrefab
        {
            var prefab = sPrefabDatabase.get<Renderer>();
            return Instantiate(prefab, parent);
        }

        public static Renderer createRenderer<Renderer>(Transform parent, string prefabName)
            where Renderer : MonoBehaviour, ITezMultiPrefab
        {
            var prefab = sPrefabDatabase.get<Renderer>(prefabName);
            return Instantiate(prefab, parent);
        }

        public static GMO createGMO<GMO>(Transform parent)
            where GMO : TezGameMonoObject, ITezSinglePrefab
        {
            var prefab = sPrefabDatabase.get<GMO>();
            return Instantiate(prefab, parent);
        }
        #endregion

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
                    widget = Instantiate(prefab, parent, false);
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
                        widget = Instantiate(prefab, parent, false);
                        sWidgetWithType.Add(type, widget);
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
        public static Widget createWidget<Widget>(RectTransform parent, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return (Widget)createWidget(sPrefabDatabase.get<Widget>(), parent, life);
        }

        public static Widget createWidget<Widget>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return createWidget<Widget>(layer.rectTransform, life);
        }

        private static Window createWindow<Window>(Window prefab
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
                    if (sWidgetWithType.TryGetValue(type, out widget))
                    {
                        widget.reset();
                        return (Window)widget;
                    }
                    else
                    {
                        window = Instantiate(prefab, layer.transform, false);
                        sWidgetWithType.Add(type, window);
                    }
                    break;
                default:
                    break;
            }

            int id = giveID();
            window.windowID = id;
            window.windowName = name;
            window.layer = layer;
            window.life = life;

            sWindowList[id] = window;
            return (Window)window;
        }

        public static Window createWindow<Window>(string name, TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return createWindow(sPrefabDatabase.get<Window>(), name, layer, life);
        }

        public static TezWindow createWindow(TezWindow prefab, TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal)
        {
            return createWindow(prefab, prefab.GetType().Name, layer, life);
        }

        public static Window createWindow<Window>(TezLayer layer, TezWidgetLife life = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return createWindow<Window>(typeof(Window).Name, layer, life);
        }

        public static void removeWindow(TezWindow window)
        {
            sFreeWindowID.Enqueue(window.windowID);
            sWindowList[window.windowID] = null;
        }

        public static void removeTypeOnlyWidget(TezBaseWidget widget)
        {
            sWidgetWithType.Remove(widget.GetType());
        }
        #endregion

        #region Refresh
        static Queue<ITezRefreshHandler> sUIRefreshQueue = new Queue<ITezRefreshHandler>();
        static Queue<ITezRefreshHandler> sRendererRefreshQueue = new Queue<ITezRefreshHandler>();
        static Queue<ITezDelayInitHandler> sInitQueue = new Queue<ITezDelayInitHandler>();

        public static void pushRendererToRefresh(TezGameRenderer gameRenderer)
        {
            sRendererRefreshQueue.Enqueue(gameRenderer);
        }

        public static void pushRefreshHandler(ITezRefreshHandler handler)
        {
            sUIRefreshQueue.Enqueue(handler);
        }

        public static void pushDelayInitHandler(ITezDelayInitHandler handler)
        {
            sInitQueue.Enqueue(handler);
        }
        #endregion

        #region KeyConfig
        static UnityKeyConfigSystem sUnityKeyConfigSystem = null;
        public static UnityKeyConfigSystem unityKeyConfigSystem => sUnityKeyConfigSystem;
        #endregion

        #region Graphic
        static TezGraphicSystem sGraphicSystem = null;
        public static TezGraphicSystem graphicSystem => sGraphicSystem;
        #endregion

        #region Prefab
        static TezPrefabDatabase sPrefabDatabase = null;
        public static TezPrefabDatabase prefabDatabase => sPrefabDatabase;
        #endregion
    }
}