﻿using UnityEngine;

namespace tezcat.Framework.Core
{
    public class TezValueModifierCache : TezValueModifierBaseCache
    {
        protected float m_SumBase = 0;
        protected float m_SumTotal = 0;
        protected float m_MultBase = 0;
        protected float m_MultTotal = 0;

        public override float calculate(TezPropertyFloat property)
        {
            float result = property.baseValue + m_SumBase + property.baseValue * m_MultBase;
            result = result + m_SumTotal + result * m_MultTotal;
            return result;
        }

        public override int calculate(TezPropertyInt property)
        {
            float result = property.baseValue + m_SumBase + property.baseValue * m_MultBase;
            result = result + m_SumTotal + result * m_MultTotal;
            return Mathf.RoundToInt(result);
        }

        protected override void onModifierAdded(ITezValueModifier modifier)
        {
            var def = (TezValueModifierDefinition)modifier.definition;
            switch (def.assemble)
            {
                case TezValueModifierDefinition.Assemble.SumBase:
                    m_SumBase += modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.SumTotal:
                    m_SumTotal += modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultBase:
                    m_MultBase += modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultTotal:
                    m_MultTotal += modifier.value;
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierRemoved(ITezValueModifier modifier)
        {
            var def = (TezValueModifierDefinition)modifier.definition;
            switch (def.assemble)
            {
                case TezValueModifierDefinition.Assemble.SumBase:
                    m_SumBase -= modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.SumTotal:
                    m_SumTotal -= modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultBase:
                    m_MultBase -= modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultTotal:
                    m_MultTotal -= modifier.value;
                    break;
                default:
                    break;
            }
        }

        protected override void onModifierChanged(ITezValueModifier modifier, float old_value)
        {
            var def = (TezValueModifierDefinition)modifier.definition;
            switch (def.assemble)
            {
                case TezValueModifierDefinition.Assemble.SumBase:
                    m_SumBase = m_SumBase - old_value + modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.SumTotal:
                    m_SumTotal = m_SumTotal - old_value + modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultBase:
                    m_MultBase = m_MultBase - old_value + modifier.value;
                    break;
                case TezValueModifierDefinition.Assemble.MultTotal:
                    m_MultTotal = m_MultTotal - old_value + modifier.value;
                    break;
                default:
                    break;
            }
        }
    }
}