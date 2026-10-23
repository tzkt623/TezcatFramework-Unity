using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBonusModifierCalculateRule : byte
    {
        /// <summary>
        /// 在基础值上加
        /// </summary>
        Base_SumAdd = 0,
        /// <summary>
        /// 在基础值上以百分比相加
        /// </summary>
        Base_PercentAdd,

        /// <summary>
        /// 在Base加成后值上加
        /// </summary>
        Total_SumAdd,

        /// <summary>
        /// 在Base加成后值上以百分比相加
        /// </summary>
        Total_PercentAdd,
    }

    public class TezModifierHelper
    {
        //默认100倍缩放
        //也就是modifier=100时,实际加成是1倍

        static int mScale = 0;
        public static int Scale => mScale;

        static float mReciprocal;
        public static float reciprocal => mReciprocal;

        public static void setScale(int scale)
        {
            mScale = scale;
            mReciprocal = 1.0f / Scale;
        }
    }

    public interface ITezBonusModifier : ITezCloseable
    {
        //所有者
        object owner { get; set; }
        //值描述符
        ITezValueDescriptor valueDescriptor { get; set; }
        //是否需要移除
        //bool isNeedRemove { get; set; }
        //计算规则
        TezBonusModifierCalculateRule calculateRule { get; set; }
        //值
        //float value { get; set; }
        int value { get; set; }
        bool isClosed { get; }

        bool tryAddContainer(ITezBonusModifierContainer master, out TezBonusModifierEntry entry);
        bool tryRemoveContainer(ITezBonusModifierContainer master, out TezBonusModifierEntry entry);
        bool removeByContainerClear(ITezBonusModifierContainer master);
    }

    /// <summary>
    /// 
    /// Entry是为了解决Modifier和Container之间的生命周期问题
    /// 当Entry销毁自己时,Container需要在下一个更新周期中移除Modifier
    /// 此时Modififer可能已经被销毁了,所以Container不能直接访问Modifier
    /// 只能通过Entry来间接验证
    /// 
    /// Entry只能被Container销毁
    /// 因为Modifier销毁后,Container需要在下一个更新周期中移除Modifier
    /// Container销毁后,Entry需要通知Modifier立即移除自己,否则会造成内存泄漏
    /// 所以Entry的生命周期由Container来主动管理
    /// 
    /// Modifer只负责生成Entry
    /// 
    /// </summary>
    public class TezBonusModifierEntry
    {
        ITezBonusModifier mModifier = null;

        public ITezBonusModifier modifier => mModifier;
        public bool isNeedRemove => mModifier == null;

        public void setModifier(ITezBonusModifier modifier)
        {
            mModifier = modifier;
        }

        public void notifyByContainerIsCleared(ITezBonusModifierContainer container)
        {
            if (mModifier == null)
            {
                recycle(this);
                return;
            }

            if (mModifier.removeByContainerClear(container))
            {
                mModifier = null;
                recycle(this);
                return;
            }

            throw new System.Exception($"[TezBonusModifierEntry] removeByContainerClear failed, modifier: {mModifier}, container: {container}");
        }

        private static Stack<TezBonusModifierEntry> sCache = new Stack<TezBonusModifierEntry>();

        public static TezBonusModifierEntry peek()
        {
            if (sCache.Count == 0)
            {
                sCache.Push(new TezBonusModifierEntry());
            }

            return sCache.Peek();
        }

        public static TezBonusModifierEntry pop()
        {
            return sCache.Pop();
        }

        public static void recycle(TezBonusModifierEntry entry)
        {
            sCache.Push(entry);
        }
    }


    /// <summary>
    /// Modifier在移除时有两种情况
    /// 一种是Modifier被销毁了，这时候BonusModifierEntry需要通知MasterContainer移除自己
    /// 另一种是MasterContainer被销毁了，这时候BonusModifierEntry需要通知Modifier移除自己
    /// 
    /// 
    /// 
    /// </summary>
    public class TezBonusModifier : ITezBonusModifier
    {
        public object owner { get; set; } = null;
        public TezBonusModifierCalculateRule calculateRule { get; set; } = 0;
        public ITezValueDescriptor valueDescriptor { get; set; } = null;

        int mValue = 0;
        public int value
        {
            get { return mValue; }
            set
            {
                mValue = value;
                this.updateContainer();
            }
        }

        public bool isClosed => mEntryDict == null;

        Dictionary<ITezBonusModifierContainer, TezBonusModifierEntry> mEntryDict = new Dictionary<ITezBonusModifierContainer, TezBonusModifierEntry>();


        private void updateContainer()
        {
            foreach (var item in mEntryDict)
            {
                item.Key.setDirty();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="master"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool ITezBonusModifier.tryAddContainer(ITezBonusModifierContainer container, out TezBonusModifierEntry entry)
        {
            if (mEntryDict.TryAdd(container, TezBonusModifierEntry.peek()))
            {
                entry = TezBonusModifierEntry.pop();
                entry.setModifier(this);
                return true;
            }

            entry = null;
            return false;
        }

        bool ITezBonusModifier.tryRemoveContainer(ITezBonusModifierContainer container, out TezBonusModifierEntry entry)
        {
            if (mEntryDict.TryGetValue(container, out entry))
            {
                entry.setModifier(null);
                mEntryDict.Remove(container);
                return true;
            }

            entry = null;
            return false;
        }

        bool ITezBonusModifier.removeByContainerClear(ITezBonusModifierContainer container)
        {
            return mEntryDict.Remove(container);
        }

        public TezBonusModifier() { }
        public TezBonusModifier(ITezValueDescriptor valueDescriptor) { this.valueDescriptor = valueDescriptor; }

        public void close()
        {
            foreach (var item in mEntryDict)
            {
                item.Key.removeModifierByModifierClosed(this);
                item.Value.setModifier(null);
            }

            mEntryDict.Clear();
            mEntryDict = null;

            this.valueDescriptor = null;
            this.owner = null;
        }

        public override string ToString()
        {
            return $"[Modifier]{this.valueDescriptor.name}: {this.value}[{(TezBonusModifierCalculateRule)this.calculateRule}]";
        }
    }
}