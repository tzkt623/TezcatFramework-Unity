using tezcat.Framework.Core;
using tezcat.Framework.DataBase;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Wrapper
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
            get { return TezService.get<TezTranslator>().translateName(this.myItem.NID); }
        }

        public string myDescription
        {
            get { return TezService.get<TezTranslator>().translateDescription(this.myItem.NID); }
        }

        public virtual TezDataBaseItem myItem { get; private set; }

        TezDataBaseItem ITezItemWrapper.myItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        TezEntity ITezComponent.entity { get { throw new System.NotImplementedException(); } }

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

        void ITezComponent.onAdd(TezEntity entity)
        {
            throw new System.NotImplementedException();
        }

        void ITezComponent.onRemove(TezEntity entity)
        {
            throw new System.NotImplementedException();
        }

        void ITezComponent.onOtherComponentAdded(ITezComponent component, int com_id)
        {
            throw new System.NotImplementedException();
        }

        void ITezComponent.onOtherComponentRemoved(ITezComponent component, int com_id)
        {
            throw new System.NotImplementedException();
        }

        void ITezCloseable.close()
        {
            throw new System.NotImplementedException();
        }

        void ITezWrapper.retain()
        {
            throw new System.NotImplementedException();
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