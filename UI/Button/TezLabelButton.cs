using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezLabelButton : TezButton
    {
        [SerializeField]
        Text m_Label = null;

        public override Graphic graphicController
        {
            get { return m_Label; }
        }

        public override void clear()
        {
            base.clear();
            m_Label = null;
        }

        public void setText(string text)
        {
            m_Label.text = text;
        }

        protected override void onPointerDown(PointerEventData eventData)
        {
        }

        protected override void onPointerUp(PointerEventData eventData)
        {
        }

        protected override void onPointerEnter(PointerEventData eventData)
        {
        }

        protected override void onPointerExit(PointerEventData eventData)
        {
        }
    }
}