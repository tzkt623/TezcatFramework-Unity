using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 行为树
    /// <para>启动流程</para>
    /// <para>LoadConfig (or) ManualBuildTree->setContext->init</para>
    /// 
    /// <para>====节点介绍====</para>
    /// <para>组合节点(Composite)</para>
    /// <para>--顺序节点Sequence</para>
    /// <para>--选择节点Selector</para>
    /// <para>--随机选择节点RandomSelector</para>
    /// <para>--并行节点Parallel</para>
    /// <para>--监察并行节点ObserveParallel</para>
    /// <para>--强制节点Force</para>
    /// <para>修饰节点(Decorator)</para>
    /// <para>--翻转节点Inverter</para>
    /// <para>--成功节点Succeeder</para>
    /// <para>--失败节点Failure</para>
    /// <para>行动节点(Action)</para>
    /// <para>--自己实现</para>
    /// <para>条件节点(Condition)</para>
    /// <para>--自己实现</para>
    /// <para>=============</para>
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
            register<TezBTObserveParallel>("ObserveParallel");
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
        public ITezBTContext context => m_Context;

        ITezBTContext m_Context = null;
        TezBTComposite m_Root = null;

//         int m_ActionIDGenerator = 0;
//         List<int> m_DeleteActionList = new List<int>();

        List<TezBTNode> m_RunningActionList = new List<TezBTNode>();
        List<TezBTCondition> m_RunningConditionList = new List<TezBTCondition>();

        List<TezBTAction> m_ActionList = new List<TezBTAction>();

        Stack<TezBTComposite> m_NodeStack = new Stack<TezBTComposite>();

        public override void init()
        {
            //             if (m_Observer == null)
            //             {
            //                 m_Observer = TezBTObserver.empty;
            //                 m_Observer.init();
            //             }

            m_Root.init();
        }

        public override void loadConfig(TezReader reader)
        {
            m_Root = (TezBTComposite)create(reader.readString("CID"));
            m_Root.tree = this;
            m_Root.parent = this;
            m_Root.loadConfig(reader);
        }

        public void setContext(ITezBTContext context)
        {
            m_Context = context;
        }

        public Context getContext<Context>() where Context : ITezBTContext
        {
            return (Context)this.context;
        }

        public Node createRoot<Node>() where Node : TezBTComposite, new()
        {
            var node = new Node();
            m_Root = node;
            m_Root.parent = this;
            m_Root.tree = this;
            return node;
        }

        public void pushNode(TezBTComposite node)
        {
            m_NodeStack.Push(node);
        }

        public void popNode(TezBTComposite node)
        {
            if (node != m_NodeStack.Pop())
            {
                throw new Exception();
            }
        }

        public void close()
        {
            m_Root.close();
            m_Context.close();

            m_Root = null;
            m_Context = null;
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
            m_Root.execute();
#if false
            ///如果没有正在Running状态的Node
            ///则思考策略
            if (m_RunningActionList.Count == 0)
            {
                m_Root.execute();
            }

            if(m_RunningConditionList.Count > 0)
            {
                for (int i = 0; i < m_RunningConditionList.Count; i++)
                {
                    m_RunningConditionList[i].execute();
                }
            }

            ///如果有正在Running状态的Node
            ///则执行当前任务
            if (m_RunningActionList.Count > 0)
            {
                for (int i = 0; i < m_RunningActionList.Count; i++)
                {
                    m_RunningActionList[i].execute();
                }
            }
#endif
        }

        public override Result newExecute()
        {
            switch (m_Root.newExecute())
            {
                case Result.Success:
                    this.reset();
                    onTraversalComplete?.Invoke(Result.Success);
                    return Result.Success;
                case Result.Fail:
                    this.reset();
                    onTraversalComplete?.Invoke(Result.Fail);
                    return Result.Fail;
            }

            return Result.Running;
        }

        public void addRunningNode(TezBTNode node)
        {
            m_RunningActionList.Add(node);
        }

        public bool removeRunningNode(TezBTNode node)
        {
            return m_RunningActionList.Remove(node);
        }

        public void addRunningConditionNode(TezBTCondition node)
        {
            m_RunningConditionList.Add(node);
        }

        public bool removeRunningConditionNode(TezBTCondition node)
        {
            return m_RunningConditionList.Remove(node);
        }

        public void registerAction(TezBTAction node)
        {
            node.actionIndex = m_ActionList.Count;
            m_ActionList.Add(node);
        }

        public override void onReport(TezBTNode node, Result result)
        {
            if (result != Result.Running)
            {
                this.reset();
                onTraversalComplete?.Invoke(result);
            }
        }

        public override void removeSelfFromTree()
        {
            throw new NotImplementedException();
        }
    }
}