using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public enum TezSelectorType
    {
        Object,
        Item
    }

    public interface ITezSelectable : ITezCore
    {
        TezSelectorType selectorType { get; }
    }

    /// <summary>
    /// 基础选择器
    /// 
    /// </summary>
    public abstract class TezBasicSelector
    {
        public abstract TezSelectorType selectorType { get; }
    }

    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public class TezObjectSelector : TezBasicSelector
    {
        TezObject m_Object = null;

        public sealed override TezSelectorType selectorType
        {
            get { return TezSelectorType.Object; }
        }

        public TezObjectSelector(TezObject obj)
        {
            m_Object = obj;
        }
    }

    /// <summary>
    /// 
    /// 在此框架下
    /// 当你选择一个Item时
    /// 这个Item必然处于一个的管理器中
    /// 所以必然会选择到一个Slot
    /// 
    /// </summary>
    public class TezItemSelector : TezBasicSelector
    {
        TezItemSlot m_Slot = null;

        public sealed override TezSelectorType selectorType
        {
            get { return TezSelectorType.Item; }
        }

        public TezItemSelector(TezItemSlot slot)
        {
            m_Slot = slot;
        }

        public Item convertItem<Item>() where Item : TezDatabaseItem
        {
            return (Item)m_Slot.myItem;
        }

        public Slot convertSlot<Slot>() where Slot : TezItemSlot
        {
            return (Slot)m_Slot;
        }
    }
}