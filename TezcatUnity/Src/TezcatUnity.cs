using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
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
        #region Tool
        public static readonly TezPrefabDatabase DBPrefab = new TezPrefabDatabase();
        #endregion

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

        protected virtual void beforeRegister()
        {

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

        protected override void onDelayInit(float dt)
        {
            if (!mResourceInited)
            {
                mResourceInited = true;
                StartCoroutine(startGame());
            }
        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {

        }
        #endregion

        #region Registery
        private void register()
        {
            TezFilePath.setUnityPath(Application.dataPath);

            this.beginRegister();
            this.processRegistery();
            this.endRegister();
        }

        protected virtual void processRegistery()
        {
            this.registerVersions();
            this.registerClassFactory();
            this.registerService();
            this.registerFramework();
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
        }

        /// <summary>
        /// 必须要对framework里的工具进行初始化
        /// </summary>
        protected virtual void registerFramework()
        {

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
            where Renderer : MonoBehaviour, ITezPrefab
        {
            var prefab = TezcatUnity.DBPrefab.get<Renderer>();
            return Instantiate(prefab, parent, true);
        }

        public static Renderer createRenderer<Renderer>(Transform parent, string prefabName)
            where Renderer : MonoBehaviour, ITezPrefab
        {
            var prefab = TezcatUnity.DBPrefab.get<Renderer>(prefabName);
            return Instantiate(prefab, parent, true);
        }

        public static GMO createGMO<GMO>(Transform parent)
            where GMO : TezGameMonoObject, ITezPrefab
        {
            var prefab = TezcatUnity.DBPrefab.get<GMO>();
            return Instantiate(prefab, parent, true);
        }
        #endregion



        #region Refresh
        static Queue<ITezRefreshHandler> sUIRefreshQueue = new Queue<ITezRefreshHandler>();
        static Queue<ITezRefreshHandler> sRendererRefreshQueue = new Queue<ITezRefreshHandler>();
        static Queue<ITezDelayInitHandler> sInitQueue = new Queue<ITezDelayInitHandler>();

        public static void pushRendererToRefresh(TezGameRenderer gameRenderer)
        {
            //sRendererRefreshQueue.Enqueue(gameRenderer);
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
    }
}