﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public enum TezBonusModifierType : byte
    {
        Base_SumAdd = 0,
        Base_PercentAdd,
        Base_PercentMulti,
    }

    public interface ITezBonusModifier : ITezCloseable
    {
        //event TezEventExtension.Action<ITezBonusModifier> onDelete;

        object owner { get; set; }
        byte modifyType { get; set; }
        //TezBonusToken bonusToken { get; set; }
        ITezValueDescriptor valueDescriptor { get; set; }
        float value { get; set; }
    }

    public class TezBonusModifier : ITezBonusModifier
    {
        //public event TezEventExtension.Action<ITezBonusModifier> onDelete;

        public object owner { get; set; }
        public byte modifyType { get; set; }
        //public TezBonusToken bonusToken { get; set; }
        public ITezValueDescriptor valueDescriptor { get; set; }
        public float value { get; set; }

        public TezBonusModifier()
        {

        }

        public TezBonusModifier(ITezValueDescriptor valueDescriptor)
        {
            this.valueDescriptor = valueDescriptor;
        }

//         public TezBonusModifier(TezBonusToken bonusToken)
//         {
//             this.bonusToken = bonusToken;
//         }

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            //onDelete?.Invoke(this);
            //this.bonusToken = null;
            this.valueDescriptor = null;
            this.owner = null;
            //onDelete = null;
        }

        public override string ToString()
        {
            return $"[Modifier]{this.valueDescriptor.name}: {this.value}[{(TezBonusModifierType)this.modifyType}]";
        }
    }

#if false
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

        protected override void onClose()
        {
            base.onClose();
            this.entry = null;
        }
    }
#endif
}