using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat
{
    public class TezLabelButton
        : TezUIObjectMB
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerDownHandler
        , IPointerUpHandler
    {
        public GameEvent.Event onClick;

        [SerializeField]
        Image m_BG = null;
        [SerializeField]
        Text m_Label = null;
        [SerializeField]
        Color m_PressColor;

        Color m_LabelColor;

        Tweener m_Tweener = null;


        protected override void onAwake()
        {

        }

        protected override void onStart()
        {
            m_LabelColor = m_Label.color;

        }

        protected override void onDelete()
        {

        }

        protected override void onDisable()
        {

        }

        protected override void onEnable()
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            m_Label.color = m_PressColor;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            m_Label.color = m_LabelColor;
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if(onClick != null)
                    {
                        onClick();
                    }
                    break;
                case PointerEventData.InputButton.Right:
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_Tweener = m_BG.DOColor(Colors.button_hover, 0.8f);
            m_Tweener.SetAutoKill(false);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_Tweener.Rewind();
            m_Tweener.Kill();
        }

        public void setText(string text)
        {
            m_Label.text = text;
        }
    }
}