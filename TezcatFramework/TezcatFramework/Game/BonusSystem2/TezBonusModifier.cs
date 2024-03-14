using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBonusModifierType : byte
    {
        Base_SumAdd = 0,
        Base_PercentAdd,
        Base_PercentMulti,
    }

    public abstract class TezBonusModifier : ITezCloseable
    {
        public object owner;
        public byte modifyType;
        public TezBonusToken bonusToken;

        public abstract float value { get; set; }

        public virtual void close()
        {
            this.bonusToken = null;
            this.owner = null;
        }
    }

    public class TezBonusValueModifier : TezBonusModifier
    {
        float mValue;
        public override float value
        {
            get => mValue;
            set => mValue = value;
        }
    }

    public interface ITezBonusReadModifierEntry
    {
        float value { get; }
    }

    public class TezBonusReadModifier : TezBonusModifier
    {
        public ITezBonusReadModifierEntry entry = null;

        public override float value
        {
            get { return entry.value; }
            set { throw new Exception("Read Only BRO!!"); }
        }

        public void bind(ITezBonusReadModifierEntry entry)
        {
            this.entry = entry;
        }

        public override void close()
        {
            base.close();
            this.entry = null;
        }
    }
}

