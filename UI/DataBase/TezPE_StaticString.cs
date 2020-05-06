using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.String;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezPE_StaticString : TezToolWidget
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

        protected override void onRefresh(TezRefreshPhase phase)
        {
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    break;
                case TezRefreshPhase.P_OnEnable:
                    break;
                default:
                    break;
            }
        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        protected override void onClose(bool self_close = true)
        {
            m_PropertyName = null;
        }

        private void set(string content)
        {
            m_String.replace(content);
        }

        public void bind(TezEventExtension.Function<string> function, TezStaticString str)
        {
            m_PropertyName.setGetter(function);
            m_String = str;
            m_Input.text = m_String;
        }
    }
}
