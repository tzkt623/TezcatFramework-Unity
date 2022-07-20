using System.Collections.Generic;
using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTComposite : TezBTNode
    {
        public override Category category => Category.Composite;
        public abstract int childrenCount { get; }
        bool m_Locked = false;

        public virtual void addNode(TezBTNode node)
        {
            node.tree = this.tree;
            node.parent = this;
            node.deep = this.deep + 1;
        }

        protected override void report(Result result)
        {
            this.parent.onReport(this, result);
        }

        public void unlockNode()
        {
            m_Locked = false;
            this.tree.popNode(this);
        }

        public void lockNode()
        {
            m_Locked = true;
            this.tree.pushNode(this);
        }

        public sealed override void execute()
        {
//             if(!m_Locked)
//             {
//                 this.lockNode();
//             }

            this.onExecute();
        }

        protected abstract void onExecute();

        public override void loadConfig(TezReader reader)
        {
            reader.beginArray("Nodes");

            var count = reader.count;
            for (int i = 0; i < count; i++)
            {
                reader.beginObject(i);
                var node = TezBehaviorTree.create(reader.readString("CID"));
                this.addNode(node);
                node.loadConfig(reader);
                reader.endObject(i);
            }

            reader.endArray("Nodes");
        }
    }

    /// <summary>
    /// List
    /// </summary>
    public abstract class TezBTComposite_List : TezBTComposite
    {
        public override int childrenCount => m_List.Count;
        protected List<TezBTNode> m_List = new List<TezBTNode>();
        protected int m_Index = 0;

        public override void init()
        {
            m_List.TrimExcess();
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].init();
            }
        }

        public override void close()
        {
            base.close();

            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].close();
            }
            m_List.Clear();
            m_List = null;
        }

        public override void addNode(TezBTNode node)
        {
            base.addNode(node);
            node.index = m_List.Count;
            m_List.Add(node);
        }

        public override void reset()
        {
            m_Index = 0;
        }

        protected override void onExecute()
        {
            m_List[m_Index].execute();
        }
    }
}