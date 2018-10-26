using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLabelButton : TezButton
    {
        [SerializeField]
        Text m_Label = null;
        public Text label
        {
            get { return m_Label; }
        }

        public override Graphic graphicController
        {
            get { return m_Label; }
        }

        public override void clear()
        {
            base.clear();
            m_Label = null;
        }
    }
}