using System.Collections.Generic;
using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTComposite : TezBTNode
    {
        public override Category category => Category.Composite;
        public abstract int childrenCount { get; }

        public virtual void addNode(TezBTNode node)
        {
            node.tree = this.tree;
            node.parent = this;
            node.deep = this.deep + 1;
        }

        public T createNode<T>() where T : TezBTNode, new()
        {
            var node = new T();
            this.addNode(node);
            return node;
        }

        protected override void reportToParent(Result result)
        {
            throw new System.Exception(string.Format("TezBTComposite : Obsolete Method {0}", nameof(reportToParent)));
//            this.parent.onReport(this, result);
        }

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
    public abstract class TezBTCompositeList : TezBTComposite
    {
        public override int childrenCount => mList.Count;
        protected List<TezBTNode> mList = new List<TezBTNode>();

        /// <summary>
        /// 索引值被动记录了当前正在运行的路径
        /// </summary>
        protected int mIndex = 0;

        public override void init()
        {
            mList.TrimExcess();
            for (int i = 0; i < mList.Count; i++)
            {
                mList[i].init();
            }
        }

        public override void close()
        {
            base.close();

            for (int i = 0; i < mList.Count; i++)
            {
                mList[i].close();
            }
            mList.Clear();
            mList = null;
        }

        public override void addNode(TezBTNode node)
        {
            base.addNode(node);
            node.index = mList.Count;
            mList.Add(node);
        }

        public override void reset()
        {
            mIndex = 0;
        }
    }
}