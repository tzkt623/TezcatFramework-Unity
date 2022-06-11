using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 物品虚拟选择接口
    /// </summary>
    public interface ITezInventoryVisualSelector
    {
        void onSelect(TezComData game_object);
        void onComplete();
    }

    /// <summary>
    /// 物品选择器
    /// </summary>
    public class TezInventorySelector
    {
        public bool isActive
        {
            get { return this.sourceSlot != null; }
        }

        public TezInventory sourceInventory { get; private set; }
        public TezInventory targetInventory { get; set; }

        public TezInventoryItemSlot sourceSlot { get; private set; }

        ITezInventoryVisualSelector m_VisualSelector = null;

        public void setVisualSelector(ITezInventoryVisualSelector visualSelector)
        {
            m_VisualSelector = visualSelector;
        }

        public void setSlot(TezInventoryItemSlot slot)
        {
            this.sourceSlot = slot;
            this.sourceInventory = slot.inventory;
            m_VisualSelector.onSelect(slot.item);
        }

        public void putToTarget(int count = 1)
        {
            this.targetInventory.store(sourceSlot.item, count);
            this.sourceInventory.take(sourceSlot.index, count);
        }

        public void removeFromSource(int count = 1)
        {
            this.sourceInventory.take(sourceSlot.index, count);
        }

        public void complete()
        {
            m_VisualSelector.onComplete();
            this.sourceInventory = null;
            this.targetInventory = null;
            this.sourceSlot = null;
        }

        public virtual void close()
        {
            this.sourceInventory = null;
            this.targetInventory = null;

            this.sourceSlot = null;
        }
    }

    public static class TezInventoryManager
    {
        public enum State
        {
            BeginAdd,
            ContinueAdd,
            EndAdd,
            ItemSwitch,
        }

        public static event TezEventExtension.Action<State> eventState;
        public static event TezEventExtension.Action eventEndPick;

        static TezInventory m_SelectedInventory = null;
        static List<TezInventoryItemSlot> m_SelectedItemList = new List<TezInventoryItemSlot>();

        public static void selectSlot(TezInventory inventory, TezInventoryItemSlot itemSlot)
        {
            if (itemSlot.picked)
            {
                return;
            }

            ///没有被选择的Inventory 或者是一样的 就是Pick功能
            if (m_SelectedInventory == null)
            {
                itemSlot.picked = true;
                m_SelectedInventory = inventory;
                m_SelectedItemList.Add(itemSlot);
                eventState?.Invoke(State.BeginAdd);
            }
            else
            {
                if (m_SelectedInventory == inventory)
                {
                    itemSlot.picked = true;
                    m_SelectedItemList.Add(itemSlot);
                    eventState?.Invoke(State.ContinueAdd);
                }
                else
                {
                    ///只选择了一个
                    if (m_SelectedItemList.Count == 1)
                    {
                        ///如果选择的槽位有item了
                        ///就是交换功能
                        if (itemSlot.item != null)
                        {
                            var count = m_SelectedItemList[0].count;
                            var temp = m_SelectedItemList[0].take();

                            m_SelectedItemList[0].item = itemSlot.item;
                            m_SelectedItemList[0].count = itemSlot.count;

                            itemSlot.item = temp;
                            itemSlot.count = count;

                            eventState?.Invoke(State.ItemSwitch);
                        }
                        ///否则就是存储功能
                        else
                        {
                            var count = m_SelectedItemList[0].count;
                            var item = m_SelectedItemList[0].take();
                            itemSlot.store(item, count);

                            eventState?.Invoke(State.ContinueAdd);
                        }
                    }
                    ///大量释放就是存储功能
                    else
                    {
                        for (int i = 0; i < m_SelectedItemList.Count; i++)
                        {
                            var count = m_SelectedItemList[i].count;
                            var item = m_SelectedItemList[i].take();
                            inventory.store(item, count);
                        }

                        eventState?.Invoke(State.ContinueAdd);
                    }
                }
            }
        }

        public static void clearSelect()
        {
            m_SelectedInventory = null;
            for (int i = 0; i < m_SelectedItemList.Count; i++)
            {
                m_SelectedItemList[i].picked = false;
            }
            m_SelectedItemList.Clear();
            eventState?.Invoke(State.EndAdd);
        }
    }
}