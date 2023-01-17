using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryFilter : ITezCloseable
    {
        public abstract class BaseFilter : ITezCloseable
        {
            protected string mName = null;
            public string name => mName;
            public int index { get; }

            public abstract bool calculate(TezItemableObject gameObject);
            public abstract void setFunction(TezEventExtension.Function<bool, TezItemableObject> function);
            public abstract int getCount(TezInventoryFilter manager);
            public abstract TezInventoryDataSlot get(TezInventoryFilter manager, int index);

            public virtual void close()
            {
                mName = null;
            }

            public BaseFilter(string name, int index)
            {
                mName = name;
                this.index = index;
            }

            /// <summary>
            /// 将当前ItemSlot
            /// 转化为Filter中的Slot
            /// </summary>
            public abstract TezInventoryDataSlot convertToDataSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot);
            public abstract TezInventoryDataSlot createSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot);
        }

        public class Filter_Null : BaseFilter
        {
            public override int getCount(TezInventoryFilter manager) => manager.mInventoryRef.get().count;
            public override TezInventoryDataSlot get(TezInventoryFilter manager, int index)
            {
                return manager.mInventoryRef.get()[index];
            }

            public Filter_Null(string name) : base(name, 0)
            {
            }

            public override bool calculate(TezItemableObject gameObject)
            {
                return true;
            }

            public override TezInventoryDataSlot convertToDataSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot)
            {
                return itemSlot;
            }

            public override TezInventoryDataSlot createSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot)
            {
                return itemSlot;
            }

            public override void setFunction(TezEventExtension.Function<bool, TezItemableObject> function)
            {

            }
        }

        public class Filter_Custom : BaseFilter
        {
            TezEventExtension.Function<bool, TezItemableObject> mFunction = null;

            public Filter_Custom(string name, int index) : base(name, index)
            {
            }

            public override int getCount(TezInventoryFilter manager)
            {
                return manager.mSlotList.Count;
            }

            public override TezInventoryDataSlot get(TezInventoryFilter manager, int index)
            {
                return manager.mSlotList[index];
            }

            public override bool calculate(TezItemableObject gameObject)
            {
                return mFunction(gameObject);
            }

            public override void close()
            {
                base.close();
                mFunction = null;
            }

            public override TezInventoryDataSlot convertToDataSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot)
            {
                manager.mSlotDict.TryGetValue(itemSlot.index, out var slot);
                return slot;
            }

            public override TezInventoryDataSlot createSlot(TezInventoryFilter manager, TezInventoryItemSlot itemSlot)
            {
                return manager.createOrGetFilterSlot(itemSlot);
            }

            public override void setFunction(TezEventExtension.Function<bool, TezItemableObject> function)
            {
                mFunction = function;
            }
        }

        #region Tool
        public const string DefaultFilter = "DefaultFilter";
        static List<BaseFilter> sFilters = new List<BaseFilter>()
        {
            new Filter_Null(DefaultFilter)
        };


        public static int createFilter(string filterName, TezEventExtension.Function<bool, TezItemableObject> function)
        {
            var result = sFilters.Find((BaseFilter filter) =>
            {
                return filter.name == filterName;
            });

            if (result == null)
            {
                result = new Filter_Custom(filterName, sFilters.Count);
                result.setFunction(function);
                sFilters.Add(result);
                return result.index;
            }

            return -1;
        }
        #endregion

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



        Dictionary<int, TezInventoryFilterSlot> mSlotDict = new Dictionary<int, TezInventoryFilterSlot>();
        List<TezInventoryFilterSlot> mSlotList = new List<TezInventoryFilterSlot>();

        BaseFilter mCurrentFilter = null;
        TezFlagableRef<TezInventory> mInventoryRef = null;

        public int count
        {
            get
            {
                return mCurrentFilter.getCount(this);
            }
        }

        public TezInventoryDataSlot this[int index]
        {
            get
            {
                return mCurrentFilter.get(this, index);
            }
        }

        public TezInventoryFilter()
        {
            mCurrentFilter = sFilters[0];
        }

        public void setInventory(TezInventory inventory)
        {
            if (mInventoryRef != null && mInventoryRef.tryGet(out var old_inventory))
            {
                old_inventory.onItemAdded -= this.onItemAdded;
                old_inventory.onItemRemoved -= this.onItemRemoved;
                mInventoryRef.close();
            }

            this.resetSlots();

            mInventoryRef = new TezFlagableRef<TezInventory>(inventory);
            inventory.onItemAdded += onItemAdded;
            inventory.onItemRemoved += onItemRemoved;
        }

        private void resetSlots()
        {
            for (int i = 0; i < mSlotList.Count; i++)
            {
                mSlotList[i].close();
            }
            mSlotList.Clear();
            mSlotDict.Clear();
        }

        public void changeFilter(int index = 0)
        {
            if (mCurrentFilter.index != index)
            {
                mCurrentFilter = sFilters[index];

                this.resetSlots();

                if (mInventoryRef.tryGet(out var inventory))
                {
                    for (int i = 0; i < inventory.count; i++)
                    {
                        var slot = inventory[i];
                        if (mCurrentFilter.calculate(slot.item))
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
            if (mCurrentFilter.name != filterName)
            {
                var result = sFilters.Find((BaseFilter filter) =>
                {
                    return filter.name == filterName;
                });

                mCurrentFilter = result;

                this.resetSlots();

                if (mInventoryRef.tryGet(out var inventory))
                {
                    for (int i = 0; i < inventory.count; i++)
                    {
                        var slot = inventory[i];
                        if (mCurrentFilter.calculate(slot.item))
                        {
                            this.createOrGetFilterSlot(slot);
                        }
                    }
                }

                onFilterChanged?.Invoke(this);
            }
        }

        private void onItemRemoved(TezInventoryItemSlot itemSlot)
        {
            //删除时无需检测过滤条件
            //只需要检测是否被当前过滤条件记录即可
            var data_slot = mCurrentFilter.convertToDataSlot(this, itemSlot);
            if (data_slot != null)
            {
                onItemChanged?.Invoke(data_slot);
                if (data_slot.category == TezInventoryDataSlot.Category.Filter)
                {
                    if (itemSlot.item == null)
                    {
                        mSlotDict.Remove(itemSlot.index);
                        ((TezInventoryFilterSlot)data_slot).bindItemSlot(null);
                    }
                }
            }
        }

        private void onItemAdded(TezInventoryItemSlot itemSlot)
        {
            //添加物品时需要计算过滤范围
            //如果不符合条件
            //就不显示
            if (mCurrentFilter.calculate(itemSlot.item))
            {
                var data_slot = mCurrentFilter.createSlot(this, itemSlot);
                if (data_slot != null)
                {
                    onItemChanged?.Invoke(data_slot);
                }
            }
        }

        private TezInventoryFilterSlot createOrGetFilterSlot(TezInventoryItemSlot itemSlot)
        {
            if (!mSlotDict.TryGetValue(itemSlot.index, out var slot))
            {
                foreach (var filter_slot in mSlotList)
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
                    slot.index = mSlotList.Count;
                    mSlotList.Add(slot);
                }
                mSlotDict.Add(itemSlot.index, slot);

                slot.bindItemSlot(itemSlot);
            }

            return slot;
        }

        public bool store(TezItemableObject gameObject)
        {
            if (mInventoryRef.tryGet(out var inventory))
            {
                inventory.store(gameObject);
                return true;
            }

            return false;
        }

        public bool store(TezItemableObject gameObject, int count)
        {
            if (mInventoryRef.tryGet(out var inventory))
            {
                inventory.store(gameObject, count);
                return true;
            }

            return false;
        }

        public bool take(TezItemableObject gameObject)
        {
            if (mInventoryRef.tryGet(out var inventory))
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

        public TezItemableObject take(int index)
        {
            if (mInventoryRef.tryGet(out var inventory))
            {
                return inventory.take(index);
            }

            return null;
        }

        public bool take(TezItemableObject gameObject, int count)
        {
            if (mInventoryRef.tryGet(out var inventory))
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

        public TezItemableObject take(int index, int count)
        {
            if (mInventoryRef.tryGet(out var inventory))
            {
                return inventory.take(index, count);
            }

            return null;
        }

        public void close()
        {
            if (mInventoryRef.tryGet(out var inventory))
            {
                inventory.onItemAdded -= this.onItemAdded;
                inventory.onItemRemoved -= this.onItemRemoved;
            }
            mInventoryRef.close();

            this.resetSlots();

            mCurrentFilter = null;
            mInventoryRef = null;
            mSlotDict = null;
            mSlotList = null;

            onItemChanged = null;
            onFilterChanged = null;
        }
    }
}