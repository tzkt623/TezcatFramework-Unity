using UnityEngine;

namespace tezcat.Unity.UI
{
    [RequireComponent(typeof(Canvas))]
    public class TezOverlay : TezLayer
    {
        Canvas m_Canvas = null;

        protected override void preInit()
        {
            overlay = this;
        }

        protected override void initWidget()
        {
            m_Canvas = this.GetComponent<Canvas>();
        }

        protected override void sort()
        {
            this.transform.SetAsLastSibling();
            this.name = "Layer_Overlay";
        }
    }
}

