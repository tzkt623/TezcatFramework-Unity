using tezcat.Framework.Core;

namespace tezcat.Framework.UI
{
    public abstract class TezPropertyEditor : TezToolWidget
    {
        protected TezValueWrapper m_Property;

        public void bind(TezValueWrapper value)
        {
            m_Property = value;
            this.refreshPhase = TezRefreshPhase.P_Custom1;
        }

        protected override void onClose(bool self_close = true)
        {
            m_Property = null;
        }
    }
}
