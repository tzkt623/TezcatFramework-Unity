using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryItemConfig : ITezCloseable
    {
        public int stackCount { get; private set; }
        public bool stackable
        {
            get { return this.stackCount > 0; }
        }

        public TezInventoryItemConfig(int stack_count = 0)
        {
            this.stackCount = stack_count;
        }

        public virtual void close()
        {

        }
    }
}