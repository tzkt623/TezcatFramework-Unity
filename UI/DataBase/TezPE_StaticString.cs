using tezcat.Framework.Extension;
using tezcat.Framework.Utility;
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

//        TezStaticString<> m_String = null;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
            m_Input.onEndEdit.AddListener(this.set);
        }

        protected override void onHide()
        {
            m_Input.onEndEdit.RemoveListener(this.set);
        }

        public override void reset()
        {

        }

        protected override void onClose(bool self_close)
        {
            m_PropertyName = null;
        }

        private void set(string content)
        {
//            m_String.replace(content);
        }

//         public void bind(TezEventExtension.Function<string> function, TezStaticString str)
//         {
//             m_PropertyName.setGetter(function);
//             m_String = str;
//             m_Input.text = m_String;
//         }
    }
}
