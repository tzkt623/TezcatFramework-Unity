using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using tezcat.Framework.DataBase;
using tezcat.Framework.ECS;
using tezcat.Framework.Event;
using tezcat.Framework.Extension;
using tezcat.Framework.GraphicSystem;
using tezcat.Framework.Math;
using tezcat.Framework.Threading;
using tezcat.Framework.UI;
using tezcat.Framework.Utility;
using tezcat.Framework.Wrapper;
using UnityEngine;

namespace tezcat.Framework.Core
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
        List<TezRenderer> m_ObjectMBList = new List<TezRenderer>();

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

            StartCoroutine(loadResources());
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        private void OnApplicationQuit()
        {

        }
        #endregion

        #region Register
        private void register()
        {
            TezService.register(this);
            this.registerVersions();
            this.registerComponent();
            this.registerService();
            this.registerClassFactory(TezService.get<TezClassFactory>());
        }

        protected virtual void registerService()
        {
            TezService.register<ITezEntityManager>(new TezEntityManager());

            TezService.register(new TezThread());
            TezService.register(new TezTranslator());

            TezService.register(new TezDebug());
            TezService.register(new TezGraphicSystem());
            TezService.register(new TezEventDispatcher());

            TezService.register(new TezClassFactory());
            TezService.register(new TezSaveManager());

            TezService.register(new TezRandom());
            TezService.register(new TezTextureDatabase());
            TezService.register(new TezPrefabDatabase());
            TezService.register(new TezDatabase());
            TezService.register(new TezTipController());
            TezService.register(new TezDragDropManager());
        }


        protected virtual void registerClassFactory(TezClassFactory factory)
        {

        }

        protected virtual void registerComponent()
        {
            TezComponentManager.register<TezDataObject>();
            TezComponentManager.register<TezRenderer>();
            TezComponentManager.register<TezWrapper>();
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
            if (!m_LayerDic.ContainsKey(layer.name))
            {
                while (m_LayerList.Count <= layer.ID)
                {
                    m_LayerList.Add(null);
                }

                m_LayerDic.Add(layer.name, layer.ID);
                m_LayerList[layer.ID] = layer;

#if UNITY_EDITOR
                TezService.get<TezDebug>().info("UIRoot", "Add Layer: " + layer.name + " ID: " + layer.ID);
#endif
            }
#if UNITY_EDITOR
            else
            {
                TezService.get<TezDebug>().waring("UIRoot", "Repeat to add layer " + layer.name);
            }
#endif
        }
        #endregion

        #region Renderer
        public Renderer createRenderer<Renderer>(Transform parent) where Renderer : TezRenderer
        {
            var prefab = TezService.get<TezPrefabDatabase>().get<Renderer>();
            return MonoBehaviour.Instantiate(prefab, parent);
        }

        public GameMonoObject createGMO<GameMonoObject>(Transform parent) where GameMonoObject : TezGameMonoObject
        {
            var prefab = TezService.get<TezPrefabDatabase>().get<GameMonoObject>();
            return MonoBehaviour.Instantiate(prefab, parent);
        }
        #endregion

        #region Window
        List<TezWindow> m_WindowList = new List<TezWindow>();
        Queue<int> m_FreeWindowID = new Queue<int>();

        Dictionary<Type, TezWidget> m_WidgetWithType = new Dictionary<Type, TezWidget>();

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

        public Widget getTypeOnlyWidget<Widget>() where Widget : TezWidget, ITezPrefab
        {
            TezWidget widget = null;
            m_WidgetWithType.TryGetValue(typeof(Widget), out widget);
            return (Widget)widget;
        }

        public TezWidget createWidget(TezWidget prefab, string name, RectTransform parent, TezWidgetLifeForm life_form)
        {
            TezWidget widget = null;
            switch (life_form)
            {
                case TezWidgetLifeForm.Normal:
                    widget = Instantiate(prefab, parent, false);
                    break;
                case TezWidgetLifeForm.TypeOnly:
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
            widget.lifeForm = life_form;
            widget.name = name;
            return widget;
        }

        public Widget createWidget<Widget>(string name, RectTransform parent, TezWidgetLifeForm life_form = TezWidgetLifeForm.Normal) where Widget : TezWidget, ITezPrefab
        {
            return (Widget)this.createWidget(TezService.get<TezPrefabDatabase>().get<Widget>(), name, parent, life_form);
        }

        /// <summary>
        /// 创建一个控件
        /// </summary>
        /// <typeparam name="Widget">控件类型</typeparam>
        /// <param name="parent">控件的父级</param>
        /// <param name="type_only">这类控件是否只能有一个</param>
        /// <returns></returns>
        public Widget createWidget<Widget>(RectTransform parent, TezWidgetLifeForm life_form = TezWidgetLifeForm.Normal) where Widget : TezWidget, ITezPrefab
        {
            return this.createWidget<Widget>(typeof(Widget).Name, parent, life_form);
        }

        private Window createWindow<Window>(Window prefab
            , string name
            , TezLayer layer
            , TezWidgetLifeForm life_form) where Window : TezWindow, ITezPrefab
        {
            TezWindow window = null;

            switch (life_form)
            {
                case TezWidgetLifeForm.Normal:
                    window = Instantiate(prefab, layer.transform, false);
                    break;
                case TezWidgetLifeForm.TypeOnly:
                    TezWidget widget = null;
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
            window.lifeForm = life_form;

            m_WindowList[id] = window;
            return (Window)window;
        }

        public Window createWindow<Window>(string name, TezLayer layer, TezWidgetLifeForm life_form = TezWidgetLifeForm.Normal) where Window : TezWindow, ITezPrefab
        {
            return this.createWindow(TezService.get<TezPrefabDatabase>().get<Window>(), name, layer, life_form);
        }

        public TezWindow createWindow(TezWindow prefab, TezLayer layer, TezWidgetLifeForm life_form = TezWidgetLifeForm.Normal)
        {
            return this.createWindow(prefab, prefab.GetType().Name, layer, life_form);
        }

        public Window createWindow<Window>(TezLayer layer, TezWidgetLifeForm life_form = TezWidgetLifeForm.Normal) where Window : TezWindow, ITezPrefab
        {
            return this.createWindow<Window>(typeof(Window).Name, layer, life_form);
        }

        public void removeWindow(TezWindow window)
        {
            m_FreeWindowID.Enqueue(window.windowID);
            m_WindowList[window.windowID] = null;
        }

        public void removeWidget(TezWidget widget)
        {
            switch (widget.lifeForm)
            {
                case TezWidgetLifeForm.Normal:
                    break;
                case TezWidgetLifeForm.TypeOnly:
                    m_WidgetWithType.Remove(widget.GetType());
                    break;
                default:
                    break;
            }
        }


        protected virtual void onCreateWindow(Type type, TezWindow window)
        {

        }
        #endregion

        #region Refresher
        Queue<ITezRefresher> m_RefreshQueue = new Queue<ITezRefresher>();
        ITezRefresher m_Root = null;
        ITezRefresher m_Current = null;

        public void pushRefresher(ITezRefresher refresher)
        {
            if (m_Root == null)
            {
                m_Root = refresher;
                m_Current = refresher;
            }
            else
            {
                m_Current.next = refresher;
                m_Current = refresher;
            }
        }
        #endregion

        protected virtual void Update() { }

        protected virtual void LateUpdate()
        {
            if (m_Root != null)
            {
                Debug.Log("刷新中......");
                while (m_Root != null)
                {
                    m_Root.refresh();
                    m_Root = m_Root.next;
                }
                Debug.Log("刷新结束!");
            }
        }
    }
}