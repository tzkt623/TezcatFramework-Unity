using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezModifier : ITezCloseable
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

        public override void close()
        {
            base.close();
            this.source = null;
            this.definition = null;
            onValueChanged = null;
        }
    }


#if false
    public interface ITezModifier
        : ITezCloseable
    {
        int operationID { get; }
        object source { get; }
        ITezModifierOperation operation { get; }
        ITezValueDescriptor descriptor { get; }      
    }

    public abstract class TezModifier : ITezModifier
    {
        public int operationID
        {
            get { return this.operation.toID; }
        }

        public object source { get; set; }
        public ITezModifierOperation operation { get; }
        public ITezValueDescriptor descriptor { get; }

        protected TezModifier(ITezValueDescriptor descriptor, ITezModifierOperation operation)
        {
            this.descriptor = descriptor;
            this.operation = operation;
        }

        public abstract void close();
    }
#endif
}