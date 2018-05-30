using UnityEngine;

namespace tezcat.UI
{
    [RequireComponent(typeof(Canvas))]
    public class TezOverlay : TezLayer
    {
        Canvas m_Canvas = null;

        protected override void preInit()
        {
            overlay = this;
            this.name = "Layer_Overlay";
        }

        protected override void initWidget()
        {
            m_Canvas = this.GetComponent<Canvas>();
        }
    }
}

