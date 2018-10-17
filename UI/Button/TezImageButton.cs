using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezImageButton : TezButton
    {
        [SerializeField]
        public Image imgae { get; private set; }

        public override Graphic graphicController
        {
            get { return imgae; }
        }

        protected override void initWidget()
        {
            this.imgae = this.GetComponent<Image>();
        }

        protected override void onRefresh()
        {

        }

        protected override void onPointerEnter(PointerEventData eventData)
        {
//             m_Image.DOColor(new Color(1, 1, 1, 60 / 255.0f), 1.2f)
//                 .SetLoops(-1, LoopType.Yoyo)
//                 .SetAutoKill(false);
        }

        protected override void onPointerExit(PointerEventData eventData)
        {
//             m_Image.DORewind(false);
//             m_Image.DOKill();
        }

        protected override void onPointerDown(PointerEventData eventData)
        {
//             m_Image.DORewind(false);
//             m_Image.DOKill();
        }

        protected override void onPointerUp(PointerEventData eventData)
        {

        }
    }
}