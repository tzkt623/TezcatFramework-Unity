using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.UI
{
    public abstract class TezButton
        : TezWidget
        , ITezFocusableWidget
        , ITezClickable
    {
        protected Graphic m_ControlGraphic = null;

        protected virtual Graphic getGraphic()
        {
            return m_ControlGraphic;
        }

        protected override void preInit()
        {
            m_ControlGraphic = this.gameObject.GetComponent<Graphic>();
        }

        protected override void initWidget()
        {

        }

        protected override void linkEvent()
        {

        }

        protected override void onHide()
        {

        }

        protected override void onRefresh()
        {

        }

        protected override void onShow()
        {

        }

        protected override void unLinkEvent()
        {

        }

        public override void reset()
        {

        }

        public override void clear()
        {
            m_ControlGraphic = null;
        }

        protected override void onInteractable(bool value)
        {
            m_ControlGraphic.raycastTarget = value;
        }



        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
    }
}