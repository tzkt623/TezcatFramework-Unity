using System;
using System.Collections.Generic;
using tezcat.Framework.Database;
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
        static Dictionary<string, TezEventExtension.Function<TezBTNode>> m_Creator = new Dictionary<string, TezEventExtension.Function<TezBTNode>>();

        public static void register<T>() where T : TezBTNode, new()
        {
            register<T>(typeof(T).Name);
        }

        public static void register<T>(string name) where T : TezBTNode, new()
        {
            m_Creator.Add(name, () =>
            {
                return new T();
            });
        }

        public static TezBTNode create(string name)
        {
            if (m_Creator.TryGetValue(name, out var function))
            {
                return function();
            }

            throw new Exception(name);
        }

        public static T create<T>() where T : TezBTNode
        {
            return (T)create(typeof(T).Name);
        }

        static TezBehaviorTree()
        {
            ///Composite
            register<TezBTParallel>("Parallel");
            register<TezBTSequence>("Sequence");
            register<TezBTSelector>("Selector");
            register<TezBTRandomSelector>("RandomSelector");
            register<TezBTForce>("Force");

            ///Decorator
            register<TezBTInverter>("Inverter");
            register<TezBTSucceeder>("Succeeder");
            register<TezBTFailure>("Failure");
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
            return (Context)this.context;
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
            //             for (int i = 0; i < m_RunningActionList.Count; i++)
            //             {
            //                 m_RunningActionList[i].reset();
            //             }
            //             m_RunningActionList.Clear();
        }

        public override void execute()
        {
            mRoot.execute();
        }

        public override Result imdExecute()
        {
            switch (mRoot.imdExecute())
            {
                case Result.Success:
                    this.reset();
                    onTraversalComplete?.Invoke(Result.Success);
                    break;
                case Result.Fail:
                    this.reset();
                    onTraversalComplete?.Invoke(Result.Fail);
                    break;
            }

            return Result.Running;
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