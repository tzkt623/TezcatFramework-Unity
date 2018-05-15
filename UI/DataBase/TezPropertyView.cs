using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPropertyView : TezWidget
    {
        [SerializeField]
        Text m_PropertyName = null;
        [SerializeField]
        Text m_PorpertyValue = null;

        public void setInfo(string name, string value)
        {
            m_PropertyName.text = name;
            m_PorpertyValue.text = value;
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
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