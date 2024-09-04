using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 行为树
    /// 
    /// 行为树有三种状态,成功,失败,运行中
    /// 当行为树返回成功或者失败时,都代表行为树要重新进行判断
    /// 
    /// <para>启动流程</para>
    /// <para>LoadConfig (or) ManualBuildTree->setContext->init</para>
    /// <para>请参考TestBehaviorTree</para>
    /// <para>====节点介绍====</para>
    /// <para>组合节点(Composite)</para>
    /// <para>--顺序节点Sequence</para>
    /// <para>--选择节点Selector</para>
    /// <para>--随机选择节点RandomSelector</para>
    /// <para>--并行节点Parallel</para>
    /// <para>--强制节点Force</para>
    /// <para>==============</para>
    /// <para>修饰节点(Decorator)</para>
    /// <para>--翻转节点Inverter</para>
    /// <para>--成功节点Succeeder</para>
    /// <para>--失败节点Failure</para>
    /// <para>==============</para>
    /// <para>行动节点(Action)</para>
    /// <para>--自己实现</para>
    /// <para>==============</para>
    /// <para>条件节点(Condition)</para>
    /// <para>--自己实现</para>
    /// </summary>
    public class TezBehaviorTree
        : ITezBTParentNode
        , ITezCloseable
    {
        #region Factory
        static Dictionary<string, Func<TezBTNode>> sCreator = new Dictionary<string, Func<TezBTNode>>();

        public static void register<T>() where T : TezBTNode, new()
        {
            var objs = typeof(T).GetCustomAttributes(true);
            if (objs.Length == 0)
            {
                throw new IndexOutOfRangeException(string.Format("TezBehaviorTree : You Must Use [{0}] to Register BTNode", typeof(TezBTRegisterAttribute).Name));
            }

            var meta = (TezBTRegisterAttribute)objs[0];
            register<T>(meta.name);
        }

        public static void register<T>(string name) where T : TezBTNode, new()
        {
            sCreator.Add(name, () =>
            {
                return new T();
            });
        }

        public static TezBTNode create(string name)
        {
            if (sCreator.TryGetValue(name, out var function))
            {
                return function();
            }

            throw new Exception(name);
        }

        public static T create<T>() where T : TezBTNode
        {
            var objs = typeof(T).GetCustomAttributes(true);
            var meta = (TezBTRegisterAttribute)objs[0];
            return (T)create(meta.name);
        }

        static TezBehaviorTree()
        {
            ///Composite
            register<TezBTParallel>();
            register<TezBTSequence>();
            register<TezBTSelector>();
            register<TezBTRandomSelector>();
            register<TezBTForceSequence>();

            ///Decorator
            register<TezBTInverter>();
            register<TezBTSucceeder>();
            register<TezBTFailure>();
        }
        #endregion

        public event Action<TezBTNode.Result> evtBehaviorComplete;
        public event Action<TezBTNode.Result> evtBehaviorRunning;

        ITezBTContext mContext = null;
        public ITezBTContext context => mContext;

        TezBTComposite mRoot = null;

        LinkedList<TezBTAction> mActionList = new LinkedList<TezBTAction>();

        public void loadConfig(TezReader reader)
        {
            mRoot = (TezBTComposite)create(reader.readString("Node"));
            mRoot.tree = this;
            mRoot.parent = this;
            mRoot.loadConfig(reader);
        }

        public void build()
        {
            if(mRoot == null)
            {
                throw new Exception("BehaviorTree : Root Node Must be Set Before Build");
            }

            mRoot.init();
        }

        public void setContext(ITezBTContext context)
        {
            mContext = context;
        }

        public Context getContext<Context>() where Context : ITezBTContext
        {
            return (Context)mContext;
        }

        public Node createRoot<Node>() where Node : TezBTComposite, new()
        {
            var node = new Node();
            mRoot = node;
            mRoot.parent = this;
            mRoot.tree = this;
            return node;
        }

        protected void onClose()
        {
            mRoot.close();
            mContext.close();

            mRoot = null;
            mContext = null;

            this.evtBehaviorComplete = null;
        }

        public void execute()
        {
            mRoot.execute();
        }

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        void ITezBTParentNode.addChild(TezBTNode node)
        {

        }

        void ITezBTParentNode.childReport(TezBTNode.Result result)
        {
            switch (result)
            {
                case TezBTNode.Result.Success:
                    evtBehaviorComplete?.Invoke(result);
                    break;
                case TezBTNode.Result.Fail:
                    evtBehaviorComplete?.Invoke(result);
                    break;
                default:
                    evtBehaviorRunning?.Invoke(result);
                    break;
            }
        }
    }
}