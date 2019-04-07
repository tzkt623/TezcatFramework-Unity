namespace tezcat.Framework.Core
{
    /// <summary>
    /// 引用型的Modifier
    /// 其属性值来源于另一个Property
    /// 其Value值不能被直接Set
    /// </summary>
    public abstract class TezModifierRefrence : TezModifier
    {
        ITezProperty m_Property = null;

        public override float value
        {
            get
            {
                return m_Value;
            }

            set
            {
                ///引用型Modifier不能被直接Set
            }
        }

        protected TezModifierRefrence(ITezProperty property, ITezValueDescriptor descriptor, TezModifierDefinition def) : base(descriptor, def)
        {
            m_Property = property;
            m_Property.onValueChanged += onRefValueChanged;
            switch (m_Property.valueType)
            {
                case TezValueType.Int:
                    m_Value = ((TezPropertyInt)m_Property).value;
                    break;
                case TezValueType.Float:
                    m_Value = ((TezPropertyFloat)m_Property).value;
                    break;
            }
        }

        private void onRefValueChanged(ITezProperty property)
        {
            var old = m_Value;
            switch (m_Property.valueType)
            {
                case TezValueType.Int:
                    m_Value = ((TezPropertyInt)m_Property).value;
                    break;
                case TezValueType.Float:
                    m_Value = ((TezPropertyFloat)m_Property).value;
                    break;
            }
            this.notifyValueChanged(this, old);
        }

        public override void close()
        {
            base.close();
            m_Property.onValueChanged -= onRefValueChanged;
            m_Property = null;
        }
    }

}