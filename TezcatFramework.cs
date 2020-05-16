using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;
using tezcat.Framework.Event;
using tezcat.Framework.Extension;
using tezcat.Framework.Game;
using tezcat.Framework.GraphicSystem;
using tezcat.Framework.InputSystem;
using tezcat.Framework.Math;
using tezcat.Framework.Threading;
using tezcat.Framework.UI;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework
{
    public abstract class TezcatFramework
        : TezUIWidget
        , ITezService
    {
        #region Static Data
        static string m_DataPath = null;
        public static string dataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_DataPath))
                {
                    m_DataPath = TezFilePath.rootPath + "/GameData";
                }

                return m_DataPath;
            }
        }

        public static string localizationPath
        {
            get { return dataPath + "/Localization"; }
        }

        public static string databasePath
        {
            get { return dataPath + "/DataBase"; }
        }

        public static string savePath
        {
            get { return dataPath + "/Save"; }
        }

        public static void checkNeedFile()
        {
            if (!TezFilePath.directoryExist(dataPath))
            {
                var info = TezFilePath.createDirectory(dataPath);
            }

            checkFile(
                localizationPath,

                (StreamWriter writer) =>
                {
                    writer.Write(
                        "{" +
                        "\"name\":{}," +
                        "\"description\":{}" +
                        "}");
                });

            checkFile(
                databasePath,

                (StreamWriter writer) =>
                {
                    writer.Write("[]");
                });

            checkFile(
                savePath,

                (StreamWriter writer) =>
                {

                });
        }

        private static void checkFile(string path, TezEventExtension.Action<StreamWriter> action)
        {
            if (!TezFilePath.fileExist(path))
            {
                var writer = TezFilePath.createTextFile(path);
                action(writer);
                writer.Close();
            }
        }
        #endregion

        #region Engine
        protected override void preInit()
        {
            this.register();
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
            StartCoroutine(loadResources());
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
            TezService.register(this);
            this.registerVersions();
            this.registerService();
            this.registerClassFactory(TezService.get<TezClassFactory>());
        }

        protected virtual void registerService()
        {
            TezService.register<ITezEntityManager>(new TezEntityManager());

            TezService.register(new TezThread());
            TezService.register(new TezTranslator());

            TezService.register(new TezGraphicSystem());
            TezService.register(new TezEventDispatcher());
            TezService.register(new TezInputController());

            TezService.register(new TezClassFactory());
            TezService.register(new TezSaveManager());

            TezService.register(new TezRandom());
            TezService.register(new TezTextureDatabase());
            TezService.register(new TezPrefabDatabase());
            TezService.register(new TezTipController());
        }


        protected virtual void registerClassFactory(TezClassFactory factory)
        {

        }

        protected abstract void registerVersions();
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
            where Renderer : TezRenderer, ITezSinglePrefab
        {
            var prefab = TezService.get<TezPrefabDatabase>().get<Renderer>();
            var go = MonoBehaviour.Instantiate(prefab, parent);
            return go;
        }

        public Renderer createRenderer<Renderer>(Transform parent, string prefab_name)
            where Renderer : TezRenderer, ITezMultiPrefab
        {
            var prefab = TezService.get<TezPrefabDatabase>().get<Renderer>(prefab_name);
            var go = MonoBehaviour.Instantiate(prefab, parent);
            return go;
        }

        public GameMonoObject createGMO<GameMonoObject>(Transform parent)
            where GameMonoObject : TezGameMonoObject, ITezSinglePrefab
        {
            var prefab = TezService.get<TezPrefabDatabase>().get<GameMonoObject>();
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

        public Widget getTypeOnlyWidget<Widget>() where Widget : TezBaseWidget, ITezSinglePrefab
        {
            TezBaseWidget widget = null;
            m_WidgetWithType.TryGetValue(typeof(Widget), out widget);
            return (Widget)widget;
        }

        public TezBaseWidget createWidget(TezBaseWidget prefab, RectTransform parent, TezWidgetLife life_form)
        {
            TezBaseWidget widget = null;
            switch (life_form)
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

            widget.transform.localPosition = Vector3.zero;
            widget.life = life_form;
            return widget;
        }

        public Widget createWidget<Widget>(RectTransform parent, TezWidgetLife life_form = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return (Widget)this.createWidget(TezService.get<TezPrefabDatabase>().get<Widget>(), parent, life_form);
        }

        public Widget createWidget<Widget>(TezLayer layer, TezWidgetLife life_form = TezWidgetLife.Normal) where Widget : TezBaseWidget, ITezSinglePrefab
        {
            return this.createWidget<Widget>(layer.rectTransform, life_form);
        }

        private Window createWindow<Window>(Window prefab
            , string name
            , TezLayer layer
            , TezWidgetLife life_form) where Window : TezWindow, ITezSinglePrefab
        {
            TezWindow window = null;

            switch (life_form)
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
            window.transform.localPosition = Vector3.zero;
            window.life = life_form;

            m_WindowList[id] = window;
            return (Window)window;
        }

        public Window createWindow<Window>(string name, TezLayer layer, TezWidgetLife life_form = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return this.createWindow(TezService.get<TezPrefabDatabase>().get<Window>(), name, layer, life_form);
        }

        public TezWindow createWindow(TezWindow prefab, TezLayer layer, TezWidgetLife life_form = TezWidgetLife.Normal)
        {
            return this.createWindow(prefab, prefab.GetType().Name, layer, life_form);
        }

        public Window createWindow<Window>(TezLayer layer, TezWidgetLife life_form = TezWidgetLife.Normal) where Window : TezWindow, ITezSinglePrefab
        {
            return this.createWindow<Window>(typeof(Window).Name, layer, life_form);
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