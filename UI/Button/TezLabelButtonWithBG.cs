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

        protected override void onClose()
        {
            base.onClose();
            m_Background = null;
        }
    }
}