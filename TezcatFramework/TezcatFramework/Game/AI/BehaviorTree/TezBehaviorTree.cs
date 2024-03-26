using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 行为树
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
    public class TezBehaviorTree : TezBTNode
    {
        #region Factory
        static Dictionary<string, TezEventExtension.Function<TezBTNode>> sCreator = new Dictionary<string, TezEventExtension.Function<TezBTNode>>();

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
            register<TezBTForce>();

            ///Decorator
            register<TezBTInverter>();
            register<TezBTSucceeder>();
            register<TezBTFailure>();
        }
        #endregion

        public event TezEventExtension.Action<Result> onTraversalComplete;

        public override Category category => throw new Exception("Tree Don`t has Category");
        public ITezBTContext context => mContext;

        ITezBTContext mContext = null;
        TezBTComposite mRoot = null;

        public override void loadConfig(TezReader reader)
        {
            mRoot = (TezBTComposite)create(reader.readString("CID"));
            mRoot.tree = this;
            mRoot.parent = this;
            mRoot.loadConfig(reader);
        }

        public override void init()
        {
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

        public void close()
        {
            mRoot.close();
            mContext.close();

            mRoot = null;
            mContext = null;
        }

        public override void reset()
        {
            //             m_DeleteActionList.Clear();
            // 
            //             for (init i = 0; i < m_RunningActionList.Count; i++)
            //             {
            //                 m_RunningActionList[i].reset();
            //             }
            //             m_RunningActionList.Clear();
        }

        public override Result imdExecute()
        {
            switch (mRoot.imdExecute())
            {
                case Result.Success:
                    mRoot.reset();
                    onTraversalComplete?.Invoke(Result.Success);
                    break;
                case Result.Fail:
                    mRoot.reset();
                    onTraversalComplete?.Invoke(Result.Fail);
                    break;
            }

            return Result.Running;
        }

        public override void execute()
        {
            mRoot.execute();
        }

        public override void onReport(TezBTNode node, Result result)
        {
            ///如果没有在运行中
            ///才返回状态给外界查询
            if (result != Result.Running)
            {
                this.reset();
                onTraversalComplete?.Invoke(result);
            }
        }
    }
}