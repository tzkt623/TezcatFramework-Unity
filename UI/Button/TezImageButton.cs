using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public class TezImageButton : TezButton
    {
        [SerializeField]
        Image m_Image = null;
        public Image imgae
        {
            get { return m_Image; }
        }

        public override Graphic graphicController
        {
            get { return m_Image; }
        }
    }
}