using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezValueModifier
        : TezValueWrapper<float>
        , ITezValueModifier
    {
        public object source { get; set; }
        public TezModifierDefinition definition { get; protected set; }
        public TezModifierType modifierType { get; } = TezModifierType.Value;

        /// <summary>
        /// <para>TezModifier --> Self</para> 
        /// <para>float --> old value</para>
        /// </summary>
        public event TezEventExtension.Action<ITezValueModifier, float> onValueChanged;

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

        protected TezValueModifier(ITezValueDescriptor name, TezValueModifierDefinition def) : base(name)
        {
            this.definition = def;
        }

        protected void notifyValueChanged(ITezValueModifier modifier, float old_value)
        {
            onValueChanged?.Invoke(modifier, old_value);
        }

        public override string ToString()
        {
            return string.Format("[{0}]\nSrc : {1}\nDef:\n======\n{2}\n======"
                , this.name
                , this.source.GetType().Name
                , definition.ToString());
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);
            this.definition.close(self_close);

            this.source = null;
            this.definition = null;
            onValueChanged = null;
        }
    }
}