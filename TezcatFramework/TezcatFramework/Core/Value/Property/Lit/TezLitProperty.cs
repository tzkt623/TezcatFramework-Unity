using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Game;

namespace tezcat.Framework.Core
{

    /// <summary>
    /// 轻量级的属性
    /// 没有modifier功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TezLitProperty<T>
        : TezValueWrapper<T>
        , ITezLitProperty
    {
        public event TezEventExtension.Action<ITezLitProperty> onValueChanged;
        public override TezWrapperType wrapperType => TezWrapperType.LitProperty;

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


        protected TezLitProperty(ITezValueDescriptor name) : base(name)
        {

        }

        protected TezLitProperty() : base() { }

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

    public abstract class TezLitPropertyInt : TezLitProperty<int>
    {
        protected TezLitPropertyInt(ITezValueDescriptor name) : base(name)
        {

        }
    }

    public abstract class TezLitPropertyFloat : TezLitProperty<float>
    {
        protected TezLitPropertyFloat(ITezValueDescriptor name) : base(name)
        {

        }
    }
}