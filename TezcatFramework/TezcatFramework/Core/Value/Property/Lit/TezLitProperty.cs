using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Game;

namespace tezcat.Framework.Core
{
    public interface ITezLitProperty : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezLitProperty> onValueChanged;
        void manualUpdate();
    }

    public interface ITezLitProperty<T> : ITezLitProperty
    {
        T value { get; set; }
        T innerValue { set; }
    }

    /// <summary>
    /// 轻量级的属性
    /// 没有modifier功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TezLitProperty<T>
        : TezValueWrapper<T>
        , ITezLitProperty<T>
    {
        public event TezEventExtension.Action<ITezLitProperty> onValueChanged;
        public override TezWrapperType wrapperType => TezWrapperType.LitProperty;

        T mValue;
        /// <summary>
        /// 设置此数值将会通知更新
        /// </summary>
        public override T value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                this.onValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// 内部数据
        /// 设置此数据将不会通知更新
        /// 需要手动调用update
        /// </summary>
        public T innerValue
        {
            set { mValue = value; }
        }


        public TezLitProperty(ITezValueDescriptor descriptor) : base(descriptor)
        {

        }

        public TezLitProperty() : base() { }

        public override void close()
        {
            this.descriptor = null;
            mValue = default;
            onValueChanged = null;
        }

        /// <summary>
        /// 手动通知数据更新
        /// </summary>
        public void manualUpdate()
        {
            this.onValueChanged?.Invoke(this);
        }
    }

    public class TezLitPropertyInt : TezLitProperty<int>
    {
        public TezLitPropertyInt() : base() { }

        public TezLitPropertyInt(ITezValueDescriptor name) : base(name) { }
    }

    public class TezLitPropertyFloat : TezLitProperty<float>
    {
        public TezLitPropertyFloat() : base() { }

        public TezLitPropertyFloat(ITezValueDescriptor name) : base(name) { }
    }
}