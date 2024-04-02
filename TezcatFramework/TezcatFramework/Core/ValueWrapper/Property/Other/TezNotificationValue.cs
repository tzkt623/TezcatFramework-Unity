using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezNotificationValue
    {
        event TezEventExtension.Action<ITezNotificationValue> onValueChanged;
    }

    public class TezNotificationValue<T>
        : TezValueWrapper<T>
        , ITezNotificationValue
    {
        public event TezEventExtension.Action<ITezNotificationValue> onValueChanged;

        public override TezWrapperType wrapperType => TezWrapperType.Notification;

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

        public TezNotificationValue(ITezValueDescriptor name) : base(name)
        {

        }
    }
}