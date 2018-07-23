using DG.Tweening;
using tezcat.Signal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public class TezImageButton : TezButton
    {
        public event TezEventCenter.Action<PointerEventData.InputButton> onClick;

        [SerializeField]
        Image m_Image;
        [SerializeField]
        Color m_PressColor;
        Color m_OrgColor;

        protected override void onInteractable(bool value)
        {
            if(value)
            {
                m_Image.color = m_OrgColor;
            }
            else
            {
                m_Image.color = Color.gray;
            }
        }

        protected override void onRefresh()
        {

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_Image.DOColor(new Color(1, 1, 1, 60 / 255.0f), 1.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_Image.DORewind(false);
            m_Image.DOKill();

            m_Image.color = m_PressColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            m_Image.color = m_OrgColor;
            onClick?.Invoke(eventData.button);
        }

        protected override void initWidget()
        {
            m_OrgColor = m_Image.color;
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }
    }
}