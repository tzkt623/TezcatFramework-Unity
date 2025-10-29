using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezValue : ITezValueWrapper
    {
        void manualUpdate();
    }

    public interface ITezValue<T> : ITezValue
    {
        event TezEventExtension.Action<T> evtValueChanged;
        T value { get; set; }
        T innerValue { set; }
    }

    /// <summary>
    /// 轻量级的属性
    /// 没有modifier功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TezValue<T>
        : TezValueWrapper<T>
        , ITezValue<T>
    {
        public event TezEventExtension.Action<T> evtValueChanged;
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
                this.evtValueChanged?.Invoke(mValue);
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

        public string valueToString()
        {
            return mValue.ToString();
        }

        public TezValue(ITezValueDescriptor descriptor) : base(descriptor)
        {

        }

        public TezValue() : base() { }

        protected override void onClose()
        {
            base.onClose();
            evtValueChanged = null;
        }

        /// <summary>
        /// 手动通知数据更新
        /// </summary>
        public void manualUpdate()
        {
            this.evtValueChanged?.Invoke(mValue);
        }
    }

    public class TezValueInt : TezValue<int>
    {
        public TezValueInt() : base() { }

        public TezValueInt(ITezValueDescriptor name) : base(name) { }
    }

    public class TezValueFloat : TezValue<float>
    {
        public TezValueFloat() : base() { }

        public TezValueFloat(ITezValueDescriptor name) : base(name) { }
    }


    public class TezValueArray : TezValueWrapperArray<ITezValue>
    {

    }
}