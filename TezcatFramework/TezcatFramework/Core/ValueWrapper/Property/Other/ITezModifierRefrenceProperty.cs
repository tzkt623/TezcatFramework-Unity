using tezcat.Framework.BonusSystem;

namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        /// <summary>
        /// 只能由MR来释放Ref
        /// 其他地方不能释放
        /// </summary>
        bool allowCloseRef { get; }
        TezAgentValueModifier modifierRefrence { get; }
        void createRefrence(TezBonusPath definition, TezValueModifierConfig modifierConfig, object source);
    }
}