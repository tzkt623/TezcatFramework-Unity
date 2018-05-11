using tezcat.DataBase;

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
    public abstract class TezItemSlot : TezSingleDataSlot<TezItem>
    {
        /// <summary>
        /// 槽ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 管理器
        /// </summary>
        public ITezItemSlotManager manager { get; set; }

        /// <summary>
        /// 槽物品
        /// </summary>
        public TezItem item
        {
            get { return this.myData; }
            set { this.myData = value; }
        }


        public override void clear()
        {
            this.manager = null;
            base.clear();
        }
    }
}
