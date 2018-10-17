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

    public abstract class TezGOSlot
        : ITezCloseable
    {
        public abstract TezGameObject getGameObject();

        public abstract void close();

        public static bool operator true(TezGOSlot slot)
        {
            return !object.ReferenceEquals(slot, null);
        }

        public static bool operator false(TezGOSlot slot)
        {
            return object.ReferenceEquals(slot, null);
        }

        public static bool operator !(TezGOSlot slot)
        {
            return object.ReferenceEquals(slot, null);
        }
    }

    public abstract class TezGOSlot<T>
        : TezGOSlot
        where T : TezGameObject
    {
        public T myGO { get; private set; }

        public TezGOSlot(T my_go)
        {
            this.myGO = my_go;
        }

        public override TezGameObject getGameObject()
        {
            return this.myGO;
        }
    }
}
