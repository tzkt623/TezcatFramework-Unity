using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezNotificationValue
    {
        event TezEventExtension.Action<ITezNotificationValue> evtValueChanged;
    }

    public class TezNotificationValue<T>
        : TezValueWrapper<T>
        , ITezNotificationValue
    {
        public event TezEventExtension.Action<ITezNotificationValue> evtValueChanged;

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
                this.evtValueChanged?.Invoke(this);
            }
        }

        public TezNotificationValue(ITezValueDescriptor name) : base(name)
        {

        }
    }
}