using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezLabelButtonWithBG : TezLabelButton
    {
        [SerializeField]
        protected Image m_Background = null;

        public Image getBG()
        {
            return m_Background;
        }

        public override void clear()
        {
            base.clear();
            m_Background = null;
        }
    }
}