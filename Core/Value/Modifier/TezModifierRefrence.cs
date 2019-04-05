using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public abstract class TezModifierRefrence
        : TezValueWrapper<float>
        , ITezModifier
    {
        public object source { get; set; }
        public TezModifierDefinition definition { get; protected set; }
        public event TezEventExtension.Action<ITezModifier, float> onValueChanged;

        ITezProperty m_Property = null;

        public sealed override ITezValueDescriptor descriptor
        {
            get { return m_Property.descriptor; }
            set { }
        }

        protected float m_Value = 0;
        public override float value
        {
            get
            {
                switch (m_Property.valueType)
                {
                    case TezValueType.Int:
                        m_Value = ((TezPropertyInt)m_Property).value;
                        break;
                    case TezValueType.Float:
                        m_Value = ((TezPropertyFloat)m_Property).value;
                        break;
                }
                return m_Value;
            }

            set
            {
                m_Value = value;
            }
        }

        protected TezModifierRefrence(ITezProperty property, TezModifierDefinition def) : base(null)
        {
            m_Property = property;
            m_Property.onValueChanged += onRefValueChanged;
            this.definition = def;
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
            onValueChanged?.Invoke(this, old);
        }

        public override void close()
        {
            base.close();
            m_Property.onValueChanged -= onRefValueChanged;
            m_Property = null;

            this.source = null;
            this.definition = null;
            onValueChanged = null;
        }
    }

}