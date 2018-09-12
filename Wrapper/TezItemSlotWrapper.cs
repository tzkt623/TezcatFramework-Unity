using tezcat.Core;
using tezcat.DataBase;

namespace tezcat.Wrapper
{
    /// <summary>
    /// 
    /// TezItemSlot专用Wrapper
    /// 
    /// 用于取得Slot中Item的特定数据
    /// 例如 本地化文本 图片 模型等等
    /// 
    /// 继承此类以完成自己的Wrapper
    /// 
    /// </summary>
    /// 
    public class TezItemSlotWrapper : TezItemWrapper
    {
        public TezItemSlot mySlot { get; private set; }
        public override TezItem myItem
        {
            get { return mySlot.myItem; }
        }

        public Slot getSlot<Slot>() where Slot : TezItemSlot
        {
            return (Slot)mySlot;
        }

        public TezItemSlotWrapper(TezItemSlot slot) : base(slot.myItem)
        {
            mySlot = slot;
        }

        public override void close()
        {
            mySlot = null;
        }
    }


    public class TezItemSlotWrapper<Slot> : TezItemSlotWrapper where Slot : TezItemSlot
    {
        public TezItemSlotWrapper(Slot slot) : base(slot)
        {

        }

        public Slot getSlot()
        {
            return this.getSlot<Slot>();
        }
    }
}
