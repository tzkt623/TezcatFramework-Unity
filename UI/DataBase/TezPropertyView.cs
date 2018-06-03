using tezcat.Signal;
using UnityEngine;

namespace tezcat.UI
{
    public class TezPropertyView : TezToolWidget
    {
        [SerializeField]
        TezText m_PropertyName = null;
        [SerializeField]
        TezText m_PorpertyValue = null;

        public void set(string name, string value)
        {
            m_PropertyName.text = name;
            m_PorpertyValue.text = value;
        }

        public void set(TezEventBus.Function<string> name, TezEventBus.Function<string> value)
        {
            m_PropertyName.setGetter(name);
            m_PorpertyValue.setGetter(value);
        }

        public override void clear()
        {
            m_PropertyName = null;
            m_PorpertyValue = null;
        }

        protected override void onRefresh()
        {
            m_PropertyName.dirty = true;
            m_PorpertyValue.dirty = true;
        }

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {

        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }
    }
}