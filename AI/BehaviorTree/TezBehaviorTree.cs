using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.AI
{
    public class TezBehaviorTree : ITezCloseable
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
            return m_Creator[name]();
        }

        public static T create<T>() where T : TezBTNode
        {
            return (T)create(typeof(T).Name);
        }

        static TezBehaviorTree()
        {
            register<TezBTParallel>("Parallel");
            register<TezBTSequence>("Sequence");
            register<TezBTSelector>("Selector");
            register<TezBTRandomSelector>("RandomSelector");
            register<TezBTForce>("Force");
        }
        #endregion

        public event TezEventExtension.Action<TezBTNode.Result> onTraversalComplete;

        public ITezBTContext context { get; protected set; }

        TezBTNode m_Root = null;
        TezBTObserver m_Observer = null;

        int m_TaskIDGenerator = 0;
        List<TezBTActionNode> m_ActionList = new List<TezBTActionNode>();
        List<int> m_DeleteActionList = new List<int>();

        public Context getContext<Context>() where Context : ITezBTContext
        {
            return (Context)this.context;
        }

        public Node createRoot<Node>() where Node : TezBTNode, new()
        {
            var node = new Node();
            m_Root = node;
            return node;
        }

        public void init(ITezBTContext context, TezReader reader)
        {
            this.loadConfig(reader);
            this.init(context);
        }

        public void init(ITezBTContext context)
        {
            this.context = context;

            if (m_Observer == null)
            {
                m_Observer = TezBTObserver.empty;
                m_Observer.init();
            }

            m_Root.init();
        }

        public void setObserver(TezBTObserver observer)
        {
            m_Observer = observer;
            m_Observer.tree = this;
            m_Observer.init();
        }

        public void close(bool self_close = true)
        {
            m_Observer.close(false);
            m_Root.close(false);
            this.context.close(false);
        }

        public void reset()
        {
            for (int i = 0; i < m_ActionList.Count; i++)
            {
                m_ActionList[i].reset();
            }
            m_ActionList.Clear();
        }

        public void execute()
        {
            ///如果情况有变
            if (m_Observer.update(context))
            {
                ///重置整个树重新思考
                this.reset();
            }

            ///如果有任务
            ///执行当前任务
            if (m_ActionList.Count > 0)
            {
                for (int i = 0; i < m_ActionList.Count; i++)
                {
                    switch (m_ActionList[i].execute())
                    {
                        case TezBTNode.Result.Running:
                            break;
                        default:
                            m_DeleteActionList.Add(i);
                            break;
                    }
                }

                if (m_DeleteActionList.Count > 0)
                {
                    for (int i = m_DeleteActionList.Count - 1; i >= 0; i--)
                    {
                        m_ActionList.RemoveAt(m_DeleteActionList[i]);
                    }

                    m_DeleteActionList.Clear();
                }
            }
            ///否则寻找策略
            else
            {
                m_Root.execute();
            }
        }

        public void addTask(TezBTActionNode node)
        {
            m_ActionList.Add(node);
            m_ActionList.Sort((TezBTActionNode a, TezBTActionNode b) =>
            {
                return a.taskIndex.CompareTo(b.taskIndex);
            });
        }

        public void registerAction(TezBTActionNode node)
        {
            node.taskIndex = m_TaskIDGenerator++;
        }

        public void loadConfig(TezReader reader)
        {
            m_Root = create(reader.readString("CID"));
            m_Root.tree = this;
            m_Root.loadConfig(reader);
        }

        public void addActionNode(TezBTActionNode node)
        {
            m_ActionList.Add(node);
        }

        public void onReport(TezBTNode node, TezBTNode.Result result)
        {
            if (result != TezBTNode.Result.Running)
            {
                this.reset();
                onTraversalComplete?.Invoke(result);
            }
        }
    }
}
