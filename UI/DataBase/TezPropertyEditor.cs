using tezcat.Framework.Core;

namespace tezcat.Framework.UI
{
    public abstract class TezPropertyEditor : TezToolWidget
    {
        protected TezValueWrapper m_Property;

        public void bind(TezValueWrapper value)
        {
            m_Property = value;
            this.refresh = RefreshPhase.Custom1;
        }

        public override void clear()
        {
            m_Property = null;
        }
    }
}
