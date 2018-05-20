using tezcat.DataBase;
using tezcat.Event;

namespace tezcat.Core
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
        public abstract TezDatabase.GroupType groupType { get; }
        public abstract TezDatabase.CategoryType categoryType { get; }
    }

    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public class TezObjectSelector : TezBasicSelector
    {
        TezObject m_Object = null;

        public sealed override TezDatabase.GroupType groupType
        {
            get { return m_Object.groupType; }
        }

        public sealed override TezDatabase.CategoryType categoryType
        {
            get { return m_Object.categoryType; }
        }

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

        public sealed override TezDatabase.GroupType groupType
        {
            get { return m_Slot.myData.groupType; }
        }

        public sealed override TezDatabase.CategoryType categoryType
        {
            get { return m_Slot.myData.categoryType; }
        }

        public TezItemSelector(TezItemSlot slot)
        {
            m_Slot = slot;
        }

        public Item convertItem<Item>() where Item : TezItem
        {
            return (Item)m_Slot.myData;
        }

        public Slot convertSlot<Slot>() where Slot : TezItemSlot
        {
            return (Slot)m_Slot;
        }
    }


    public class TezSelectController
    {
        public static TezEvent<TezBasicSelector> onSelect { get; private set; } = new TezEvent<TezBasicSelector>();
        public static TezEvent<TezBasicSelector> onCancelSelect { get; private set; } = new TezEvent<TezBasicSelector>();

        static TezBasicSelector m_Current = null;

        public static void select(TezItemSlot slot)
        {
            m_Current = new TezItemSelector(slot);
            onSelect.invoke(m_Current);
        }

        public static void select(TezObject tobject)
        {
            m_Current = new TezObjectSelector(tobject);
            onSelect.invoke(m_Current);
        }

        public static void cancelSelect()
        {
            onCancelSelect.invoke(m_Current);
            m_Current = null;
        }

        public static void objectToDo(TezEventBus.Action<TezObjectSelector> action)
        {
            if (!object.ReferenceEquals(m_Current, null) && m_Current.selectorType == TezSelectorType.Object)
            {
                action((TezObjectSelector)m_Current);
            }
        }

        public static void itemToDo(TezEventBus.Action<TezItemSelector> action)
        {
            if (!object.ReferenceEquals(m_Current, null) && m_Current.selectorType == TezSelectorType.Item)
            {
                action((TezItemSelector)m_Current);
            }
        }

        public static void clear()
        {
            m_Current = null;
        }
    }
}