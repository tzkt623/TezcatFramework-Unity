using tezcat.Framework.BonusSystem;

namespace tezcat.Framework.Core
{
    public abstract class TezFunctionModifier : ITezFunctionModifer
    {
        public TezModifierType modifierType { get; } = TezModifierType.Function;
        public TezBonusPath bonusPath { get; set; } = null;
        public object source { get; set; }

        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }
}