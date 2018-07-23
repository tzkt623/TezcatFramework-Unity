using tezcat.Signal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezImageLabelButton : TezButton
    {
        public event TezEventCenter.Action<PointerEventData.InputButton> onClick;

        [SerializeField]
        Image m_Icon = null;
        [SerializeField]
        Sprite m_Normal = null;
        [SerializeField]
        Sprite m_Hover = null;
        [SerializeField]
        TezText m_Label = null;

        [SerializeField]
        Color m_PressColor;
        Color m_LabelColor;

        public string text
        {
            get { return m_Label.text; }
            set { m_Label.text = value; }
        }

        public TezText textDelegate
        {
            get { return m_Label; }
        }

        protected override void onInteractable(bool value)
        {

        }

        protected override void initWidget()
        {
            m_LabelColor = m_Label.color;
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public override void clear()
        {
            onClick = null;
        }

        protected override void onRefresh()
        {
            m_Label.dirty = true;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_Label.color = m_PressColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    m_Label.color = m_LabelColor;
                    onClick.Invoke(eventData.button);
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_Icon.sprite = m_Hover;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            m_Icon.sprite = m_Normal;
        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {
            onClick = null;
        }
    }
}