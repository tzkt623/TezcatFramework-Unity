using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat
{
    public class TezLabelButton : TezButton
    {
        public event TezEventBus.Action onClick;

        public override bool interactable
        {
            get { return base.interactable; }

            set
            {
                base.interactable = value;
                if(value)
                {
                    m_Label.color = m_LabelColor;
                }
                else
                {
                    m_Label.color = Color.gray;
                }
            }
        }

        [SerializeField]
        Image m_BG = null;
        [SerializeField]
        Text m_Label = null;
        [SerializeField]
        Color m_PressColor;

        Color m_LabelColor;

        Tweener m_Tweener = null;

        protected override void Start()
        {
            base.Start();
            m_LabelColor = m_Label.color;
        }

        public override void clear()
        {

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!this.interactable)
            {
                return;
            }

            m_Label.color = m_PressColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!this.interactable)
            {
                return;
            }

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    m_Label.color = m_LabelColor;
                    onClick?.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.interactable)
            {
                return;
            }

            m_Tweener = m_BG.DOColor(Colors.button_hover, 0.8f);
            m_Tweener.SetAutoKill(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!this.interactable)
            {
                return;
            }

            m_Tweener.Rewind();
            m_Tweener.Kill();
        }

        public void setText(string text)
        {
            m_Label.text = text;
        }
    }
}