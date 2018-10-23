using tezcat.Core;

namespace tezcat.UI
{
    public abstract class TezPropertyEditor : TezToolWidget
    {
        protected TezValueWrapper m_Property;

        public void bind(TezValueWrapper value)
        {
            m_Property = value;
            this.refresh = RefreshPhase.System1;
        }

        public override void clear()
        {
            m_Property = null;
        }
    }
}
