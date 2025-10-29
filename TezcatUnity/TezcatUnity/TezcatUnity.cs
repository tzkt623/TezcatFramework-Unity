using System.Collections;
using System.Collections.Generic;
using tezcat.Framework;
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
        }

        /// <summary>
        /// 必须要对framework里的工具进行初始化
        /// </summary>
        protected virtual void registerFramework()
        {
            //TezcatFramework.set(new TezProtoDatabase());
            //TezcatFramework.set(new TezRunTimeDatabase());
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
            where Renderer : MonoBehaviour, ITezPrefab
        {
            var prefab = TezPrefabDatabase.get<Renderer>();
            return Instantiate(prefab, parent);
        }

        public static Renderer createRenderer<Renderer>(Transform parent, string prefabName)
            where Renderer : MonoBehaviour, ITezPrefab
        {
            var prefab = TezPrefabDatabase.get<Renderer>(prefabName);
            return Instantiate(prefab, parent);
        }

        public static GMO createGMO<GMO>(Transform parent)
            where GMO : TezGameMonoObject, ITezPrefab
        {
            var prefab = TezPrefabDatabase.get<GMO>();
            return Instantiate(prefab, parent);
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