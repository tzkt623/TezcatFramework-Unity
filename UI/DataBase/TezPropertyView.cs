using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.UI
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

        public void set(TezEventExtension.Function<string> name, TezEventExtension.Function<string> value)
        {
            m_PropertyName.setGetter(name);
            m_PorpertyValue.setGetter(value);
        }

        public override void clear()
        {
            m_PropertyName = null;
            m_PorpertyValue = null;
        }

        protected override void onRefresh(RefreshPhase phase)
        {

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

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }
    }
}