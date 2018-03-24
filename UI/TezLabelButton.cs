using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace tezcat
{
    [RequireComponent(typeof(Text))]
    public class TezLabelButton : TezButton
    {
        public event TezEventBus.Action<PointerEventData.InputButton> onClick;
        [SerializeField]
        Color m_PressColor;

        Text m_Label = null;

        Color m_LabelColor;

        Tweener m_Tweener = null;

        protected override void Awake()
        {
            base.Awake();
            m_Label = this.GetComponent<Text>();
        }

        protected override void Start()
        {
            base.Start();
            m_LabelColor = m_Label.color;
        }

        public override void clear()
        {

        }

        protected override void onInteractable(bool value)
        {
            if (value)
            {
                m_Label.color = m_LabelColor;
            }
            else
            {
                m_Label.color = Color.gray;
            }
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
            if (!this.interactable)
            {
                return;
            }

            m_Tweener = m_Label.DOColor(Colors.button_hover, 0.8f);
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