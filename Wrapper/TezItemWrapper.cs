using tezcat.Core;
using tezcat.DataBase;

namespace tezcat.Wrapper
{
    /// <summary>
    /// 
    /// TezItem类的专用wrapper
    /// 
    /// 用于取得Item里的特定数据
    /// 例如 本地化文本 图片 模型等等
    /// 
    /// </summary>
    public abstract class TezItemWrapper : ITezItemWrapper
    {
        public string myName
        {
            get { return TezTranslator.translateName(this.myItem.NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(this.myItem.NID); }
        }

        public virtual TezDataBaseItem myItem { get; private set; }

        public TezItemWrapper(TezDataBaseItem item)
        {
            myItem = item;
        }

        public virtual void close()
        {
            myItem = null;
        }

        public Item getItem<Item>() where Item : TezDataBaseItem
        {
            return (Item)myItem;
        }

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

    public abstract class TezItemWrapper<Item> : TezItemWrapper where Item : TezDataBaseItem
    {
        public TezItemWrapper(int GUID) : base(null)
        {

        }

        public TezItemWrapper(Item item) : base(item)
        {

        }

        public Item getItem()
        {
            return this.getItem<Item>();
        }
    }

}