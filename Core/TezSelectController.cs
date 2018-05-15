using tezcat.DataBase;
using tezcat.Event;

namespace tezcat.Core
{
    public enum TezSelectType
    {
        Object,
        Item
    }

    public interface ITezSelectable : ITezCore
    {
        TezSelectType selectType { get; }
    }

    public abstract class TezBasicSelector
    {
        public abstract TezSelectType selectType { get; }
        public abstract TezDatabase.GroupType groupType { get; }
        public abstract TezDatabase.CategoryType categoryType { get; }
    }

    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public class TezSelectObject : TezBasicSelector
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

        public sealed override TezSelectType selectType
        {
            get { return TezSelectType.Object; }
        }

        public TezSelectObject(TezObject obj)
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

        public sealed override TezSelectType selectType
        {
            get { return TezSelectType.Item; }
        }

        public sealed override TezDatabase.GroupType groupType
        {
            get { return m_Slot.myData.groupType; }
        }

        public sealed override TezDatabase.CategoryType categoryType
        {
            get { return m_Slot.myData.categoryType; }
        }

        public TezItemSelector(TezItemSlot wrapper)
        {
            m_Slot = wrapper;
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

        static TezBasicSelector m_Current;

        public static void select(TezBasicSelector selectable)
        {
            m_Current = selectable;
            onSelect.invoke(m_Current);
        }

        public static void cancelSelect()
        {
            onCancelSelect.invoke(m_Current);
            m_Current = null;
        }

        public static void objectToDo(TezEventBus.Action<TezSelectObject> action)
        {
            if (!object.ReferenceEquals(m_Current, null) && m_Current.selectType == TezSelectType.Object)
            {
                action((TezSelectObject)m_Current);
            }
        }

        public static void itemToDo(TezEventBus.Action<TezItemSelector> action)
        {
            if (!object.ReferenceEquals(m_Current, null) && m_Current.selectType == TezSelectType.Item)
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