

using tezcat.DataBase;

namespace tezcat
{
    public abstract class TezItemBuilder<Item> where Item : ITezItem
    {
        public abstract Item build(Item item);
    }

    public class TezEmptyItemBuilder : TezItemBuilder<ShipProject.GameItem>
    {
        public override ShipProject.GameItem build(ShipProject.GameItem item)
        {
            return item;
        }
    }
}