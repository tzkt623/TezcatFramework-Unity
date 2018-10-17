using tezcat.DataBase;
using tezcat.Event;
using tezcat.Extension;

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

        public Item convertItem<Item>() where Item : TezDataBaseItem
        {
            return (Item)m_Slot.myItem;
        }

        public Slot convertSlot<Slot>() where Slot : TezItemSlot
        {
            return (Slot)m_Slot;
        }
    }


    public class TezSelectController
    {
        public static TezAction<TezBasicSelector> onSelect { get; private set; } = new TezAction<TezBasicSelector>();
        public static TezAction<TezBasicSelector> onCancelSelect { get; private set; } = new TezAction<TezBasicSelector>();

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

        public static void objectToDo(TezEventExtension.Action<TezObjectSelector> action)
        {
            if (!object.ReferenceEquals(m_Current, null) && m_Current.selectorType == TezSelectorType.Object)
            {
                action((TezObjectSelector)m_Current);
            }
        }

        public static void itemToDo(TezEventExtension.Action<TezItemSelector> action)
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