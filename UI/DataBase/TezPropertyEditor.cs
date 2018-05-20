using tezcat.Utility;
namespace tezcat.UI
{
    public abstract class TezPropertyEditor : TezWidget
    {
        protected TezPropertyValue m_Property;

        public void bind(TezPropertyValue value)
        {
            m_Property = value;
            this.dirty = true;
        }

        public override void clear()
        {
            m_Property = null;
        }
    }
}
