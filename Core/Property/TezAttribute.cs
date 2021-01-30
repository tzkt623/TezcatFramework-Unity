using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Game;

namespace tezcat.Framework.Core
{
    public interface ITezAttribute : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezAttribute> onValueChanged;
    }

    public abstract class TezAttribute<T>
        : TezValueWrapper<T>
        , ITezAttribute
    {
        public event TezEventExtension.Action<ITezAttribute> onValueChanged;
        public override TezWrapperType wrapperType => TezWrapperType.Attribute;

        T m_Value;
        /// <summary>
        /// 设置此数值将会通知更新
        /// </summary>
        public override T value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                this.update();
            }
        }

        /// <summary>
        /// 内部数据
        /// 设置此数据将不会通知更新
        /// 需要手动调用update
        /// </summary>
        public T innerValue
        {
            set { m_Value = value; }
        }


        protected TezAttribute(ITezValueDescriptor name) : base(name)
        {

        }

        protected TezAttribute() : base() { }

        public override void close()
        {
            this.descriptor = null;
            m_Value = default;
            onValueChanged = null;
        }

        /// <summary>
        /// 通知数据更新
        /// </summary>
        public void update()
        {
            this.onValueChanged?.Invoke(this);
        }
    }

    public abstract class TezAttributeInt : TezAttribute<int>
    {
        protected TezAttributeInt(ITezValueDescriptor name) : base(name)
        {

        }
    }

    public abstract class TezAttributeFloat : TezAttribute<float>
    {
        protected TezAttributeFloat(ITezValueDescriptor name) : base(name)
        {

        }
    }
}