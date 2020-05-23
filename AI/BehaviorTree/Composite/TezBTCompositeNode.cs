using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    public abstract class TezBTCompositeNode : TezBTNode
    {
        public override Category category => Category.Composite;

        public virtual void addNode(TezBTNode node)
        {
            node.tree = this.tree;
            node.parent = this;
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

        public sealed override void execute()
        {
            this.onExecute();
        }

        protected abstract void onExecute();
    }
}