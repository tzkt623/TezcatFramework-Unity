using tezcat.Core;
using tezcat.DataBase;
using tezcat.Utility;

namespace tezcat.Wrapper
{
    public interface ITezItemWrapper : ITezWrapper
    {
        TezItem getItem();

        void showTip();
        void hideTip();
    }

    public abstract class TezItemWrapper : ITezItemWrapper
    {
        public string myName
        {
            get { return TezTranslater.translateName(this.getItem().NID); }
        }

        public string myDescription
        {
            get { return TezTranslater.translateDescription(this.getItem().NID); }
        }

        public abstract void clear();

        public abstract TezItem getItem();
        public abstract void showTip();
        public abstract void hideTip();

        public static bool operator true(TezItemWrapper wrapper)
        {
            return !object.ReferenceEquals(wrapper, null);
        }

        public static bool operator false(TezItemWrapper wrapper)
        {
            return object.ReferenceEquals(wrapper, null);
        }

        public static bool operator !(TezItemWrapper wrapper)
        {
            return object.ReferenceEquals(wrapper, null);
        }
    }

    /// <summary>
    /// 
    /// TezItem类的专用wrapper
    /// 
    /// 用于取得Item里的特定数据
    /// 例如 本地化文本 图片 模型等等
    /// 
    /// </summary>
    public abstract class TezItemWrapper<Item> : TezItemWrapper where Item : TezItem
    {
        public Item myItem { get; protected set; }

        public TezItemWrapper(int GUID)
        {
            myItem = TezDatabase.getItem<Item>(GUID);
        }

        public TezItemWrapper(Item item)
        {
            myItem = item;
        }

        public sealed override TezItem getItem()
        {
            return myItem;
        }

        public override void clear()
        {
            myItem = null;
        }
    }

    /// <summary>
    /// 
    /// TezSlot专用Wrapper
    /// 
    /// 用于取得Slot中Item的特定数据
    /// 例如 本地化文本 图片 模型等等
    /// 
    /// </summary>
    public abstract class TezItemSlotWrapper<Slot> : TezItemWrapper where Slot : TezItemSlot
    {
        public Slot mySlot { get; protected set; }

        public TezItemSlotWrapper(Slot slot)
        {
            mySlot = slot;
        }

        public sealed override TezItem getItem()
        {
            return mySlot.item;
        }

        public override void clear()
        {
            mySlot = null;
        }
    }
}