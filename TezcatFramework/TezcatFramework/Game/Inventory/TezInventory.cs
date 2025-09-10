using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 基础物品信息
    /// </summary>
    public abstract class TezInventoryBaseItemInfo : ITezCloseable
    {
        public abstract bool stacked { get; }
        public virtual int count { get; set; }
        public TezProtoObject item { get; set; } = null;

        public void close()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            this.item = null;
        }
    }

    /// <summary>
    /// 唯一物品信息
    /// </summary>
    public class TezInventoryUniqueItemInfo
        : TezInventoryBaseItemInfo
        , ITezInventoryViewSlotData
        , ITezInventoryFilterData
    {
        public override bool stacked => false;
        public int viewIndex { get; set; }
        public int filterIndex { get; set; }
        public override int count
        {
            get { return 1; }
            set { }
        }
        public TezInventory inventory { get; set; }

        protected override void onClose()
        {
            base.onClose();
            this.inventory = null;
        }
    }

    /// <summary>
    /// 可堆叠物品信息
    /// </summary>
    public class TezInventoryStackedItemInfo
        : TezInventoryBaseItemInfo
    {
        public override bool stacked => true;

        public List<TezInventoryStackedItemData> list { get; set; } = new List<TezInventoryStackedItemData>();
    }

    /// <summary>
    /// 可堆叠物品分格数据
    /// </summary>
    public class TezInventoryStackedItemData
        : ITezInventoryViewSlotData
        , ITezInventoryFilterData
        , ITezCloseable
    {
        TezInventoryStackedItemInfo mInfo;
        public TezInventory inventory { get; set; }

        public int count { get; set; }
        public int viewIndex { get; set; }
        public int filterIndex { get; set; }

        public TezProtoObject item => mInfo.item;

        public TezInventoryStackedItemData(TezInventoryStackedItemInfo info)
        {
            mInfo = info;
            mInfo.list.Add(this);
        }

        public void close()
        {
            mInfo = null;
            this.inventory = null;
        }
    }

    /// <summary>
    /// 
    /// 背包系统
    /// 
    /// 1.通过唯一ID来区分物品
    /// 2.是否可以堆叠物品
    /// 
    /// 
    /// 
    /// 功能设计
    /// 
    /// 1.可堆叠物品
    /// 2.不可堆叠物品
    /// 3.过滤器
    /// 
    /// 物品栏中每种物品只占一个格子
    /// 可堆叠的也只堆叠到一个格子中
    /// 
    /// 只在显示层才会按照可堆叠个数进行分割
    /// 
    /// 
    /// Data层
    /// 用于储存item的概览数据
    /// 一个List用于统计不可堆叠的信息
    /// 一个Dict用于统计可堆叠的物品信息
    /// 每当添加一个item进来时,向view发送消息进行更新
    /// 
    /// View层
    /// 用于保存物品在inventory中的排列顺序
    /// 
    /// </summary>
    public class TezInventory
        : ITezCloseable
    {
        protected List<TezInventoryUniqueItemInfo> mUniqueItemList = new List<TezInventoryUniqueItemInfo>();
        protected Dictionary<ulong, TezInventoryStackedItemInfo> mStackedItemDict = new Dictionary<ulong, TezInventoryStackedItemInfo>();

        protected TezInventoryBaseView mView = null;
        public TezInventoryBaseView view => mView;

        TezLifeHolder mLifeMonitor = new TezLifeHolder();
        public TezLifeHolder lifeMonitor => mLifeMonitor;

        public TezInventory()
        {
            mLifeMonitor.create(this);
        }

        public void setView(TezInventoryBaseView view)
        {
            mView = view;
            mView.setInventory(this, mUniqueItemList, mStackedItemDict);
        }

        public void store(TezProtoObject item, int count = 1)
        {
            TezProtoItemInfo item_info = item.itemInfo;
            var stack_count = item_info.stackCount;

            //可堆叠物品
            if (stack_count > 1)
            {
                if (!mStackedItemDict.TryGetValue(item_info.itemID.ID, out var stack_info))
                {
                    stack_info = new TezInventoryStackedItemInfo()
                    {
                        item = item,
                        count = count
                    };

                    mStackedItemDict.Add(item_info.itemID.ID, stack_info);
                }
                else
                {
                    stack_info.count += count;
                }

                var slot_list = stack_info.list;
                int index = 0;
                while (true)
                {
                    if (index < slot_list.Count)
                    {
                        var slot = slot_list[index];
                        if (slot.count < stack_count)
                        {
                            int remain = stack_count - slot.count;
                            if (remain >= count)
                            {
                                slot.count += count;
                                mView.updateViewSlotData(slot.viewIndex);
                                break;
                            }
                            else
                            {
                                slot.count += remain;
                                count -= remain;
                                mView.updateViewSlotData(slot.viewIndex);

                                if (count == 0)
                                {
                                    break;
                                }
                            }
                        }

                        index++;
                    }
                    else
                    {
                        var data = new TezInventoryStackedItemData(stack_info);
                        mView.addViewSlotData(data);
                    }
                }
            }
            //不可堆叠物品
            else
            {
                var info = new TezInventoryUniqueItemInfo()
                {
                    item = item
                };
                mUniqueItemList.Add(info);
                mView.addViewSlotData(info);
            }
        }

        public void take(TezInventoryBaseItemInfo info, int count)
        {
            if (info.stacked)
            {
                info.count -= count;
                if (info.count == 0)
                {
                    mStackedItemDict.Remove(info.item.itemInfo.itemID.ID);
                    info.close();
                }
            }
            else
            {
                var uinfo = (TezInventoryUniqueItemInfo)info;
                mUniqueItemList.Remove(uinfo);
                uinfo.close();
            }
        }

        public void take(TezProtoObject item, int count = -1)
        {
            if (item.itemInfo.stackCount > 1)
            {
                if (mStackedItemDict.TryGetValue(item.itemInfo.itemID.ID, out var stack_info) && stack_info.item == item)
                {
                    stack_info.count -= count;

                    var list = stack_info.list;
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var info = list[i];
                        if (info.count >= count)
                        {
                            info.count -= count;

                            if (info.count == 0)
                            {
                                mView.removeViewSlotData(info);
                                info.close();
                                list.RemoveAt(i);
                            }
                            else
                            {
                                mView.updateViewSlotData(info.viewIndex);
                            }

                            break;
                        }
                        else
                        {
                            count -= info.count;
                            info.count = 0;
                            mView.removeViewSlotData(info);
                            info.close();
                            list.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < mUniqueItemList.Count; i++)
                {
                    if (mUniqueItemList[i].item == item)
                    {
                        var info = mUniqueItemList[i];
                        mUniqueItemList.RemoveAt(i);
                        mView.removeViewSlotData(info);
                        info.close();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 从包裹里面查询是否有某个特定物品
        /// 返回-1表示没有
        /// </summary>
        public TezInventoryBaseItemInfo find(TezProtoObject item)
        {
            if (item.itemInfo.stackCount > 1)
            {
                if (mStackedItemDict.TryGetValue(item.itemInfo.itemID.ID, out var result))
                {
                    return result;
                }
            }
            else
            {
                return mUniqueItemList.Find((TezInventoryUniqueItemInfo info) =>
                {
                    return info.item == item;
                });
            }

            return null;
        }

        /// <summary>
        /// 从包裹里面查询是否有某个特定类型的物品
        /// </summary>
        public TezInventoryBaseItemInfo find(ushort TID)
        {
            TezInventoryBaseItemInfo result = mUniqueItemList.Find((TezInventoryUniqueItemInfo info) =>
            {
                return info.item.itemInfo.itemID.TID == TID;
            });

            if (result != null)
            {
                return result;
            }

            foreach (var pair in mStackedItemDict)
            {
                if (pair.Value.item.itemInfo.itemID.TID == TID)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public void close()
        {
            mLifeMonitor.close();

            for (int i = 0; i < mUniqueItemList.Count; i++)
            {
                mUniqueItemList[i].close();
            }
            mUniqueItemList.Clear();

            foreach (var pair in mStackedItemDict)
            {
                pair.Value.close();
            }
            mStackedItemDict.Clear();

            mUniqueItemList = null;
            mStackedItemDict = null;
            mLifeMonitor = null;
            mView = null;
        }
    }
}