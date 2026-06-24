using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBonusModifierCalculateRule : byte
    {
        Base_SumAdd = 0,
        Base_PercentAdd,
        Base_PercentMulti,
    }

    public interface ITezBonusModifier : ITezCloseable
    {
        //所有者
        object owner { get; set; }
        //值描述符
        ITezValueDescriptor valueDescriptor { get; set; }
        //是否需要移除
        bool isNeedRemove { get; set; }
        //计算规则
        byte calculateRule { get; set; }
        //值
        float value { get; set; }

        bool addMaster(ITezBonusModifierContainer master);
        bool removeMaster(ITezBonusModifierContainer master);
    }

    public class TezBonusModifier : ITezBonusModifier
    {
        public object owner { get; set; } = null;
        public byte calculateRule { get; set; } = 0;
        public ITezValueDescriptor valueDescriptor { get; set; } = null;
        public float value { get; set; }
        public bool isNeedRemove { get; set; } = false;

        HashSet<ITezBonusModifierContainer> mMaster = new HashSet<ITezBonusModifierContainer>();

        bool ITezBonusModifier.addMaster(ITezBonusModifierContainer master)
        {
            return mMaster.Add(master);
        }

        bool ITezBonusModifier.removeMaster(ITezBonusModifierContainer master)
        {
            return mMaster.Remove(master);
        }

        public TezBonusModifier()
        {

        }

        public TezBonusModifier(ITezValueDescriptor valueDescriptor)
        {
            this.valueDescriptor = valueDescriptor;
        }

        public void close()
        {
            foreach (var item in mMaster)
            {
                item.setDirty();
            }

            mMaster.Clear();
            mMaster = null;

            this.isNeedRemove = true;
            this.valueDescriptor = null;
            this.owner = null;
        }

        public override string ToString()
        {
            return $"[Modifier]{this.valueDescriptor.name}: {this.value}[{(TezBonusModifierCalculateRule)this.calculateRule}]";
        }
    }
}