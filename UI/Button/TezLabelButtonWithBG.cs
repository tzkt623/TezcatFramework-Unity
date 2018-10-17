using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLabelButtonWithBG : TezLabelButton
    {
        [SerializeField]
        Image m_Background = null;
        [SerializeField]
        Text m_Label = null;

        public string text
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }

        public override void clear()
        {
            base.clear();
            m_Label = null;
        }
    }
}