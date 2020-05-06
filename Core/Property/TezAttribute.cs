using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezAttribute : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezAttribute> onValueChanged;

        void addBuffer(TezAttributeBuffer buffer);
        bool removeBuffer(TezAttributeBuffer buffer);
    }

    public abstract class TezAttribute<T>
        : TezValueWrapper<T>
        , ITezAttribute
    {
        public event TezEventExtension.Action<ITezAttribute> onValueChanged;
        public override TezWrapperType wrapperType => TezWrapperType.Attribute;

        List<TezAttributeBuffer> m_Buffers = new List<TezAttributeBuffer>();

        public override T value
        {
            get
            {
                return base.value;
            }
            set
            {
                base.value = value;
                this.onValueChanged?.Invoke(this);
            }
        }

        protected TezAttribute(ITezValueDescriptor name) : base(name)
        {

        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);
            onValueChanged = null;
            m_Buffers.Clear();
            m_Buffers = null;
        }

        public void addBuffer(TezAttributeBuffer buffer)
        {
            buffer.onAdd(this);
            m_Buffers.Add(buffer);
        }

        public bool removeBuffer(TezAttributeBuffer buffer)
        {
            if(m_Buffers.Remove(buffer))
            {
                buffer.onRemove(this);
                return true;
            }
            return false;
        }
    }

    public abstract class TezAttributeInt : TezAttribute<int>
    {
        protected TezAttributeInt(ITezValueDescriptor name) : base(name)
        {

        }
    }

    public abstract class TezAttributeFloat : TezAttribute<int>
    {
        protected TezAttributeFloat(ITezValueDescriptor name) : base(name)
        {

        }
    }
}