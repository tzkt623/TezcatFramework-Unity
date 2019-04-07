using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezModifier : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezModifier, float> onValueChanged;

        object source { get; set; }
        float value { get; }
        TezModifierDefinition definition { get; }
    }

    public abstract class TezModifier
        : TezValueWrapper<float>
        , ITezModifier
    {
        public object source { get; set; }
        public TezModifierDefinition definition { get; protected set; }
        /// <summary>
        /// <para>TezModifier --> Self</para> 
        /// <para>float -- > old value</para>
        /// </summary>
        public event TezEventExtension.Action<ITezModifier, float> onValueChanged;

        protected float m_Value;
        public override float value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (m_Value != value)
                {
                    var old = m_Value;
                    m_Value = value;
                    onValueChanged?.Invoke(this, old);
                }
            }
        }

        protected TezModifier(ITezValueDescriptor name, TezModifierDefinition def) : base(name)
        {
            this.definition = def;
        }

        protected void notifyValueChanged(ITezModifier modifier, float old_value)
        {
            onValueChanged?.Invoke(modifier, old_value);
        }

        public override void close()
        {
            base.close();
            this.source = null;
            this.definition = null;
            onValueChanged = null;
        }
    }
}