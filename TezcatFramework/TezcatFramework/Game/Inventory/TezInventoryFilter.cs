using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using System.Collections.Generic;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryFilter : ITezCloseable
    {
        public abstract class BaseFilter : ITezCloseable
        {
            public abstract int count { get; }
            public abstract TezInventoryDataSlot this[int index] { get; }

            public int index { get; }
            public TezInventoryFilter manager { get; set; }
            public string name { get; set; }
            public abstract bool calculate(ITezInventoryItem gameObject);
            public abstract void setFunction(TezEventExtension.Function<bool, ITezInventoryItem> function);
            public virtual void close()
            {
                this.name = null;
                this.manager = null;
            }

            public BaseFilter(int index)
            {
                this.index = index;
            }

            /// <summary>
            /// 将当前ItemSlot
            /// 转化为Filter中的Slot
            /// </summary>
            public abstract TezInventoryDataSlot convertToDataSlot(TezInventoryItemSlot itemSlot);
            public abstract TezInventoryDataSlot createSlot(TezInventoryItemSlot itemSlot);
        }

        public class Filter_Null : BaseFilter
        {
            public override int count => this.manager.m_Inventory.get().count;

            public override TezInventoryDataSlot this[int index]
            {
                get
                {
                    return this.manager.m_Inventory.get()[index];
                }
            }

            public Filter_Null() : base(0)
            {
                this.name = DefaultFilter;
            }

            public override bool calculate(ITezInventoryItem gameObject)
            {
                return true;
            }

            public override TezInventoryDataSlot convertToDataSlot(TezInventoryItemSlot itemSlot)
            {
                return itemSlot;
            }

            public override TezInventoryDataSlot createSlot(TezInventoryItemSlot itemSlot)
            {
                return itemSlot;
            }

            public override void setFunction(TezEventExtension.Function<bool, ITezInventoryItem> function)
            {

            }
        }

        public class Filter_Custom : BaseFilter
        {
            TezEventExtension.Function<bool, ITezInventoryItem> m_Function = null;

            public Filter_Custom(int index) : base(index)
            {
            }

            public override TezInventoryDataSlot this[int index]
            {
                get { return this.manager.m_SlotList[index]; }
            }

            public override int count => this.manager.m_SlotList.Count;

            public override bool calculate(ITezInventoryItem gameObject)
            {
                return m_Function(gameObject);
            }

            public override void close()
            {
                base.close();
                m_Function = null;
            }

            public override TezInventoryDataSlot convertToDataSlot(TezInventoryItemSlot itemSlot)
            {
                this.manager.m_SlotDic.TryGetValue(itemSlot.index, out var slot);
                return slot;
            }

            public override TezInventoryDataSlot createSlot(TezInventoryItemSlot itemSlot)
            {
                return this.manager.createOrGetFilterSlot(itemSlot);
            }

            public override void setFunction(TezEventExtension.Function<bool, ITezInventoryItem> function)
            {
                m_Function = function;
            }
        }

        public const string DefaultFilter = "DefaultFilter";

        /// <summary>
        /// 当前Filter规则发生变化
        /// 通知整个物品栏刷新
        /// </summary>
        public event TezEventExtension.Action<TezInventoryFilter> onFilterChanged;

        /// <summary>
        /// 当前符合Filter规则的槽位发生变化
        /// 通知此槽位刷新
        /// </summary>
        public event TezEventExtension.Action<TezInventoryDataSlot> onItemChanged;



        Dictionary<int, TezInventoryFilterSlot> m_SlotDic = new Dictionary<int, TezInventoryFilterSlot>();
        List<TezInventoryFilterSlot> m_SlotList = new List<TezInventoryFilterSlot>();

        BaseFilter m_CurrentFilter = null;
        List<BaseFilter> m_Filters = new List<BaseFilter>();
        TezWeakRef<TezInventory> m_Inventory = null;

        public int count
        {
            get
            {
                return m_CurrentFilter.count;
            }
        }

        public TezInventoryDataSlot this[int index]
        {
            get
            {
                return m_CurrentFilter[index];
            }
        }

        public TezInventoryFilter()
        {
            m_CurrentFilter = new Filter_Null()
            {
                name = DefaultFilter,
                manager = this
            };
            m_Filters.Add(m_CurrentFilter);
        }

        public void setInventory(TezInventory inventory)
        {
            if (m_Inventory != null && m_Inventory.tryGet(out var old_inventory))
            {
                old_inventory.onItemAdded -= this.onItemAdded;
                old_inventory.onItemRemoved -= this.onItemRemoved;
                m_Inventory.close();
            }

            this.resetSlots();

            m_Inventory = inventory;
            inventory.onItemAdded += onItemAdded;
            inventory.onItemRemoved += onItemRemoved;
        }

        private void resetSlots()
        {
            for (int i = 0; i < m_SlotList.Count; i++)
            {
                m_SlotList[i].close();
            }
            m_SlotList.Clear();
            m_SlotDic.Clear();
        }

        public void changeFilter(int index = 0)
        {
            if (m_CurrentFilter.index != index)
            {
                m_CurrentFilter = m_Filters[index];

                this.resetSlots();

                if (m_Inventory.tryGet(out var inventory))
                {
                    for (int i = 0; i < inventory.count; i++)
                    {
                        var slot = inventory[i];
                        if (m_CurrentFilter.calculate(slot.item))
                        {
                            this.createOrGetFilterSlot(slot);
                        }
                    }
                }

                onFilterChanged?.Invoke(this);
            }
        }

        public void changeFilter(string filterName = DefaultFilter)
        {
            if (m_CurrentFilter.name != filterName)
            {
                var result = m_Filters.Find((BaseFilter filter) =>
                {
                    return filter.name == filterName;
                });

                m_CurrentFilter = result;

                this.resetSlots();

                if (m_Inventory.tryGet(out var inventory))
                {
                    for (int i = 0; i < inventory.count; i++)
                    {
                        var slot = inventory[i];
                        if (m_CurrentFilter.calculate(slot.item))
                        {
                            this.createOrGetFilterSlot(slot);
                        }
                    }
                }

                onFilterChanged?.Invoke(this);
            }
        }

        public int createFilter(string filterName, TezEventExtension.Function<bool, ITezInventoryItem> function)
        {
            var result = m_Filters.Find((BaseFilter filter) =>
            {
                return filter.name == filterName;
            });

            if (result == null)
            {
                result = new Filter_Custom(m_Filters.Count)
                {
                    name = filterName,
                    manager = this
                };
                result.setFunction(function);
                m_Filters.Add(result);
                return result.index;
            }

            return -1;
        }

        private void onItemRemoved(TezInventoryItemSlot itemSlot)
        {
            ///删除时无需检测过滤条件
            ///只需要检测是否被当前过滤条件记录即可
            var data_slot = m_CurrentFilter.convertToDataSlot(itemSlot);
            if (data_slot != null)
            {
                onItemChanged?.Invoke(data_slot);
                if (data_slot.category == TezInventoryDataSlot.Category.Filter)
                {
                    if (itemSlot.item == null)
                    {
                        m_SlotDic.Remove(itemSlot.index);
                        ((TezInventoryFilterSlot)data_slot).bindItemSlot(null);
                    }
                }
            }
        }

        private void onItemAdded(TezInventoryItemSlot itemSlot)
        {
            ///添加物品时需要计算过滤范围
            ///如果不符合条件
            ///就不显示
            if (m_CurrentFilter.calculate(itemSlot.item))
            {
                var data_slot = m_CurrentFilter.createSlot(itemSlot);
                if (data_slot != null)
                {
                    onItemChanged?.Invoke(data_slot);
                }
            }
        }

        private TezInventoryFilterSlot createOrGetFilterSlot(TezInventoryItemSlot itemSlot)
        {
            if (!m_SlotDic.TryGetValue(itemSlot.index, out var slot))
            {
                foreach (var filter_slot in m_SlotList)
                {
                    if (filter_slot.itemSlot == null)
                    {
                        slot = filter_slot;
                        break;
                    }
                }

                if (slot == null)
                {
                    slot = TezInventoryFilterSlot.create();
                    slot.index = m_SlotList.Count;
                    m_SlotList.Add(slot);
                }
                m_SlotDic.Add(itemSlot.index, slot);

                slot.bindItemSlot(itemSlot);
            }

            return slot;
        }

        public bool store(ITezInventoryItem gameObject)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                inventory.store(gameObject);
                return true;
            }

            return false;
        }

        public bool store(ITezInventoryItem gameObject, int count)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                inventory.store(gameObject, count);
                return true;
            }

            return false;
        }

        public bool take(ITezInventoryItem gameObject)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                var index = inventory.find(gameObject);
                if (index > 0)
                {
                    inventory.take(index);
                    return true;
                }
            }

            return false;
        }

        public ITezInventoryItem take(int index)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                return inventory.take(index);
            }

            return null;
        }

        public bool take(ITezInventoryItem gameObject, int count)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                var index = inventory.find(gameObject);
                if (index > 0)
                {
                    inventory.take(index, count);
                    return true;
                }
            }

            return false;
        }

        public ITezInventoryItem take(int index, int count)
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                return inventory.take(index, count);
            }

            return null;
        }

        public void close()
        {
            if (m_Inventory.tryGet(out var inventory))
            {
                inventory.onItemAdded -= this.onItemAdded;
                inventory.onItemRemoved -= this.onItemRemoved;
            }
            m_Inventory.close();

            for (int i = 0; i < m_Filters.Count; i++)
            {
                m_Filters[i].close();
            }
            m_Filters.Clear();

            this.resetSlots();

            m_CurrentFilter = null;
            m_Filters = null;
            m_Inventory = null;
            m_SlotDic = null;
            m_SlotList = null;

            onItemChanged = null;
            onFilterChanged = null;
        }
    }
}