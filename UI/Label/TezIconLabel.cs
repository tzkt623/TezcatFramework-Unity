using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezIconLabel : TezWidget
    {
        [SerializeField]
        Image m_Icon = null;
        [SerializeField]
        TezText m_Label = null;

        public Sprite icon
        {
            get { return m_Icon.sprite; }
            set { m_Icon.sprite = value; }
        }

        public string text
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }

        protected override void onRefresh()
        {
            m_Label.dirty = true;
        }

        protected override void clear()
        {
            m_Icon = null;
            m_Label = null;
        }

        public void setGetFunction(TezEventBus.Function<string> function)
        {
            m_Label.setGetFunction(function);
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

