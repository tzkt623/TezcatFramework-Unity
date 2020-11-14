using tezcat.Framework.Core;

namespace tezcat.Framework.UI
{
    public abstract class TezPropertyEditor : TezToolWidget
    {
        protected TezValueWrapper m_Property;

        public void bind(TezValueWrapper value)
        {
            m_Property = value;
            this.refreshPhase = TezRefreshPhase.Refresh;
        }

        protected override void onClose(bool self_close)
        {
            m_Property = null;
        }
    }
}
