using tezcat.Framework.Core;
using tezcat.Framework.Database;
using System.Collections.Generic;
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

        TezBTNode m_Root = null;
        public ITezBTContext context { get; set; }

        public void setRoot(TezBTNode root)
        {
            m_Root = root;
        }

        public void init()
        {
            m_Root.init(this.context);
        }

        public void close(bool self_close = true)
        {
            m_Root.close();
            this.context.close();
        }

        public void addNode(TezBTCompositeNode composite, TezBTNode node)
        {
            composite.addNode(node);
        }

        public void execute()
        {
            m_Root.execute(this.context);
        }

        public void loadConfig(TezReader reader)
        {

        }
    }
}
