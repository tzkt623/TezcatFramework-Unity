using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public abstract class TezBTComposite 
        : TezBTNode
        , ITezBTParentNode
    {
        public override Category category => Category.Composite;
        public abstract int childrenCount { get; }

        public void addChild(TezBTNode node)
        {
            this.onAddChild(node);
        }

        protected virtual void onAddChild(TezBTNode node)
        {
            node.tree = this.tree;
            node.parent = this;
            node.deep = this.deep + 1;
        }

        public T createNode<T>() where T : TezBTNode, new()
        {
            var node = new T();
            this.onAddChild(node);
            return node;
        }

        public override void loadConfig(TezReader reader)
        {
            reader.beginArray("Nodes");

            var count = reader.count;
            for (int i = 0; i < count; i++)
            {
                reader.beginObject(i);
                var node = TezBehaviorTree.create(reader.readString("CID"));
                this.onAddChild(node);
                node.loadConfig(reader);
                reader.endObject(i);
            }

            reader.endArray("Nodes");
        }

        void ITezBTParentNode.childReport(Result result)
        {
            this.onChildReport(result);
        }

        protected abstract void onChildReport(Result result);
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

        protected override void onClose()
        {
            base.onClose();

            for (int i = 0; i < mList.Count; i++)
            {
                mList[i].close();
            }
            mList.Clear();
            mList = null;
        }

        protected override void onAddChild(TezBTNode node)
        {
            base.onAddChild(node);
            node.index = mList.Count;
            mList.Add(node);
        }

        public override void reset()
        {
            base.reset();
            mIndex = 0;
        }
    }
}