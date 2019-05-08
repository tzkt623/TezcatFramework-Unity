using UnityEngine;
using System.Collections;
using tezcat.Framework.Extension;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public interface IBuffer
    {
        object source { get; set; }
    }

    public interface IPropertyBuffer : IBuffer
    {

        void onAdd(ITezCurrentProperty property);
        void onRemove(ITezCurrentProperty property);
    }

    public interface ITezCurrentProperty : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezCurrentProperty> onValueChanged;

        void addBuffer(IBuffer buffer);
        bool removeBuffer(IBuffer buffer);
    }

    public abstract class TezCurrentProperty<T>
        : TezValueWrapper<T>
        , ITezCurrentProperty
    {
        public event TezEventExtension.Action<ITezCurrentProperty> onValueChanged;

        List<IBuffer> m_Buffers = new List<IBuffer>();

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

        protected TezCurrentProperty(ITezValueDescriptor name) : base(name)
        {

        }

        public override void close()
        {
            base.close();
            onValueChanged = null;
            m_Buffers.Clear();
            m_Buffers = null;
        }

        public void addBuffer(IBuffer buffer)
        {
            m_Buffers.Add(buffer);
        }

        public bool removeBuffer(IBuffer buffer)
        {
            return m_Buffers.Remove(buffer);
        }
    }

    public abstract class TezCPropertyInt : TezCurrentProperty<int>
    {
        protected TezCPropertyInt(ITezValueDescriptor name) : base(name)
        {

        }
    }

    public abstract class TezCPropertyFloat : TezCurrentProperty<int>
    {
        protected TezCPropertyFloat(ITezValueDescriptor name) : base(name)
        {

        }
    }
}