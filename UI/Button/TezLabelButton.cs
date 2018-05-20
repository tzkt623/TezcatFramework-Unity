using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    [RequireComponent(typeof(TezText))]
    public class TezLabelButton : TezButton
    {
        public event TezEventBus.Action<PointerEventData.InputButton> onClick;
        [SerializeField]
        Color m_PressColor;
        Color m_LabelColor;

        TezText m_Label = null;
        Tweener m_Tweener = null;

        protected override void onInteractable(bool value)
        {
            m_Label.interactable = value;
        }

        protected override void onRefresh()
        {
            m_Label.dirty = true;
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

            m_Tweener = m_Label.handler.DOColor(ShipProject.Colors.button_hover, 0.8f);
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
            m_Label.handler.text = text;
        }

        public void setGetFunction(TezEventBus.Function<string> function)
        {
            m_Label.setGetFunction(function);
        }

        protected override void preInit()
        {
            base.preInit();
            m_Label = this.GetComponent<TezText>();
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

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }

        public override void reset()
        {

        }

        public override void clear()
        {
            base.clear();
            m_Label = null;
            m_Tweener = null;
        }
    }
}