using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
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
        int value { get; }
        bool isClosed { get; }

        void setValue(float value);
        void setValue(int value);

        bool tryAddContainer(ITezBonusModifierContainer master, out TezBonusModifierEntry entry);
        bool tryRemoveContainer(ITezBonusModifierContainer master, out TezBonusModifierEntry entry);
        bool removeByContainerClear(ITezBonusModifierContainer master);
        bool tryRemoveEntry(TezBonusModifierEntry tezBonusModifierEntry);
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
        }

        public void setValue(float value)
        {
            var oldValue = mValue;
            mValue = (int)(value * TezBonusModifierConfig.scale);
            this.updateContainer(oldValue);
        }

        public void setValue(int value)
        {
            var oldValue = mValue;
            mValue = value * TezBonusModifierConfig.scale;
            this.updateContainer(oldValue);
        }

        public bool isClosed => mEntryDict == null;

        Dictionary<ITezBonusModifierContainer, TezBonusModifierEntry> mEntryDict = new Dictionary<ITezBonusModifierContainer, TezBonusModifierEntry>();

        private void updateContainer(int oldValue)
        {
            foreach (var item in mEntryDict)
            {
                item.Key.setDirty();
                item.Key.fastUpdate(oldValue, item.Value);
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
                entry.initModifier(this);
                return true;
            }

            entry = null;
            return false;
        }

        bool ITezBonusModifier.tryRemoveContainer(ITezBonusModifierContainer container, out TezBonusModifierEntry entry)
        {
            if (mEntryDict.TryGetValue(container, out entry))
            {
                entry.initModifier(null);
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
                item.Value.closeByModifierClosed();
            }

            mEntryDict.Clear();
            mEntryDict = null;

            this.valueDescriptor = null;
            this.owner = null;
        }

        public override string ToString()
        {
            return $"[Modifier]{this.valueDescriptor.name}: {mValue * TezBonusModifierConfig.reciprocal}[{this.calculateRule}] RealValue:{this.value}";
        }

        bool ITezBonusModifier.tryRemoveEntry(TezBonusModifierEntry entry)
        {
            return mEntryDict.Remove(entry.targetValue);
        }
    }
}