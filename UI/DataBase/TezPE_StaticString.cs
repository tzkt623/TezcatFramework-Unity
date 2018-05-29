using tezcat.String;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezPE_StaticString : TezWidget
    {
        [SerializeField]
        TezText m_PropertyName = null;
        [SerializeField]
        InputField m_Input = null;

        TezStaticString m_String = null;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
        }

        protected override void linkEvent()
        {
            m_Input.onEndEdit.AddListener(this.set);
        }

        protected override void unLinkEvent()
        {
            m_Input.onEndEdit.RemoveListener(this.set);
        }

        protected override void onRefresh()
        {
            m_PropertyName.dirty = true;
        }

        protected override void onHide()
        {

        }

        protected override void onShow()
        {

        }

        public override void reset()
        {

        }

        public override void clear()
        {
            m_PropertyName = null;
        }

        private void set(string content)
        {
            m_String.replace(content);
        }

        public void bind(TezEventBus.Function<string> function, TezStaticString str)
        {
            m_PropertyName.setGetter(function);
            m_String = str;
            m_Input.text = m_String;
        }
    }
}
