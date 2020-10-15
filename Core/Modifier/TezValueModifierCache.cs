using UnityEngine;

namespace tezcat.Framework.Core
{
    public class TezIntValueModifierCache : TezValueModifierBaseCache<int>
    {
        protected float m_SumBase = 0;
        protected float m_SumTotal = 0;
        protected float m_MultBase = 0;
        protected float m_MultTotal = 0;

        public override int calculate(ITezProperty<int> property)
        {
            float result = property.baseValue + m_SumBase + property.baseValue * m_MultBase;
            result = result + m_SumTotal + result * m_MultTotal;
            return Mathf.RoundToInt(result);
        }

        protected override void onModifierAdded(ITezValueModifier modifier)
        {
            var config = modifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierConfig.Assemble.SumBase:
                    m_SumBase += modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.SumTotal:
                    m_SumTotal += modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultBase:
                    m_MultBase += modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultTotal:
                    m_MultTotal += modifier.value;
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierRemoved(ITezValueModifier modifier)
        {
            var config = modifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierConfig.Assemble.SumBase:
                    m_SumBase -= modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.SumTotal:
                    m_SumTotal -= modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultBase:
                    m_MultBase -= modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultTotal:
                    m_MultTotal -= modifier.value;
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierChanged(ITezValueModifier modifier, float old_value)
        {
            var config = modifier.modifierConfig;
            switch (config.assemble)
            {
                case TezValueModifierConfig.Assemble.SumBase:
                    m_SumBase = m_SumBase - old_value + modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.SumTotal:
                    m_SumTotal = m_SumTotal - old_value + modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultBase:
                    m_MultBase = m_MultBase - old_value + modifier.value;
                    break;
                case TezValueModifierConfig.Assemble.MultTotal:
                    m_MultTotal = m_MultTotal - old_value + modifier.value;
                    break;
                default:
                    break;
            }
        }
    }
}