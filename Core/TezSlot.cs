namespace tezcat.Core
{
    /// <summary>
    /// Item管理器的槽位
    /// 
    /// 在此框架下
    /// 所有的Item类物品
    /// 必须处于一个Item管理器中
    /// 由此类Slot持有
    /// 
    /// 所以你不能直接获得一个Item
    /// 而是要先获得他的Slot
    /// 
    /// </summary>
    public abstract class TezSlot : ITezCloseable
    {
        public int ID { get; set; }

        public abstract void close();

        public static bool operator true(TezSlot slot)
        {
            return !object.ReferenceEquals(slot, null);
        }

        public static bool operator false(TezSlot slot)
        {
            return object.ReferenceEquals(slot, null);
        }

        public static bool operator !(TezSlot slot)
        {
            return object.ReferenceEquals(slot, null);
        }
    }
}
