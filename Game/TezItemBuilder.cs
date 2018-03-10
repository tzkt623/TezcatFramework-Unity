

namespace tezcat
{
    public abstract class TezItemBuilder<Item> where Item : ITezItem
    {
        public abstract Item build(Item item);
    }

    public class TezEmptyItemBuilder : TezItemBuilder<GameItem>
    {
        public override GameItem build(GameItem item)
        {
            return item;
        }
    }
}