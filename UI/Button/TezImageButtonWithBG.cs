using UnityEngine;
using UnityEngine.UI;


namespace tezcat.Framework.UI
{
    public class TezImageButtonWithBG : TezImageButton
    {
        [SerializeField]
        Image m_Background = null;
        public Image background
        {
            get
            {
                return m_Background;
            }
        }

        protected override void initWidget()
        {
            base.initWidget();
        }
    }
}