using tezcat.Framework.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Unity.UI
{
    public class TezIconLabel : TezUIWidget
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

        protected override void onClose(bool self_close)
        {
            m_Icon = null;
            m_Label = null;
        }

        public void setGetFunction(TezEventExtension.Function<string> function)
        {
            m_Label.setGetter(function);
        }

        protected override void preInit()
        {

        }

        protected override void initWidget()
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

