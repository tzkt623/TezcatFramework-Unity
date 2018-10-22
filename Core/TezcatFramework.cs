using System.Collections;
using System.Collections.Generic;
using System.IO;
using tezcat.DataBase;
using tezcat.Extension;
using tezcat.Math;
using tezcat.Signal;
using tezcat.UI;
using tezcat.Utility;
using tezcat.Wrapper;
using UnityEngine;

namespace tezcat.Core
{
    public abstract class TezcatFramework
        : TezUIWidget
        , ITezService
    {
        #region Static Data
        static string m_RootDir = null;
        public static string rootPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_RootDir))
                {
                    m_RootDir = Application.dataPath;
                    int index = m_RootDir.IndexOf(Application.productName);
                    m_RootDir = m_RootDir.Substring(0, index + Application.productName.Length) + "/GameData";
                }

                return m_RootDir;
            }
        }
        public static string localizationFile { get; private set; } = "/Localization.json";
        public static string localizationPath
        {
            get
            {
                return rootPath + localizationFile;
            }
        }

        public static string databaseFile { get; private set; } = "/Database.json";
        public static string databasePath
        {
            get
            {
                return rootPath + "/Data";
            }
        }

        public static string saveFile { get; private set; } = "/Save.json";
        public static string savePath
        {
            get
            {
                return rootPath + "/Save";
            }
        }

        //         public static string getPath(string dir, string file_name)
        //         {
        //             var dir_path = rootPath + "/" + dir;
        //             if (!Directory.Exists(dir_path))
        //             {
        //                 Directory.CreateDirectory(dir_path);
        //             }
        // 
        //             var file_path = dir_path + "/" + file_name;
        //             if (!File.Exists(file_path))
        //             {
        //                 File.Create(file_path);
        //             }
        // 
        //             File.ReadAllText()
        //         }

        public static void checkNeedFile()
        {
            if (!TezPath.directoryExist(rootPath))
            {
                var info = TezPath.createDirectory(rootPath);
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
            if (!TezPath.fileExist(path))
            {
                var writer = TezPath.createTextFile(path);
                action(writer);
                writer.Close();
            }
        }
        #endregion

        #region Engine
        List<TezGameObjectMB> m_ObjectMBList = new List<TezGameObjectMB>();

        protected override void preInit()
        {
            DontDestroyOnLoad(this);
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

        protected override void onRefresh()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onShow()
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
            this.registerService();
            this.registerClassFactory(TezService.get<TezClassFactory>());
        }

        protected virtual void registerService()
        {
            TezService.register(new TezDebug());
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

        #region Window
        List<TezWindow> m_WindowList = new List<TezWindow>();
        Dictionary<string, int> m_WindowDic = new Dictionary<string, int>();
        Queue<int> m_FreeWindowID = new Queue<int>();

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

        public TezWidget createWidget(TezWidget prefab, string name, RectTransform parent)
        {
            var widget = Instantiate(prefab, parent, false);
            widget.transform.localPosition = Vector3.zero;
            widget.name = name;
            return widget;
        }

        public Widget createWidget<Widget>(string name, RectTransform parent) where Widget : TezWidget
        {
            var widget = Instantiate(TezService.get<TezPrefabDatabase>().get<Widget>(), parent, false);
            widget.transform.localPosition = Vector3.zero;
            widget.name = name;
            return widget;
        }

        public Widget createWidget<Widget>(RectTransform parent) where Widget : TezWidget
        {
            var widget = Instantiate(TezService.get<TezPrefabDatabase>().get<Widget>(), parent, false);
            widget.transform.localPosition = Vector3.zero;
            return widget;
        }

        private Window createWindow<Window>(Window prefab, string name, int id, TezLayer layer) where Window : TezWindow, ITezPrefab
        {
            var window = Instantiate(prefab, layer.transform, false);
            window.windowID = id;
            window.windowName = name;
            window.layer = layer;
            window.transform.localPosition = Vector3.zero;

            m_WindowList[id] = window;
            m_WindowDic.Add(name, id);
            return window;
        }

        public TezWindow createWindow(ITezPrefab prefab, string name, TezLayer layer)
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(name, out id))
            {
                return m_WindowList[id];
            }

            return this.createWindow(prefab as TezWindow, name, this.giveID(), layer);
        }

        public Window createWindow<Window>(string name, TezLayer layer) where Window : TezWindow, ITezPrefab
        {
            int id = -1;
            if (m_WindowDic.TryGetValue(name, out id))
            {
                return (Window)m_WindowList[id];
            }

            return this.createWindow(TezService.get<TezPrefabDatabase>().get<Window>(), name, this.giveID(), layer);
        }

        public void removeWindow(TezWindow window)
        {
            m_FreeWindowID.Enqueue(window.windowID);
            m_WindowList[window.windowID] = null;
            m_WindowDic.Remove(window.windowName);
        }

        protected virtual void onCreateWindow(System.Type type, TezWindow window)
        {

        }
        #endregion

        protected virtual void Update() { }
    }
}