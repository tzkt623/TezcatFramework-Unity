using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezPopup
        : TezWidget
        , ITezFocusableWidget
    {
        public int popupID { get; set; }
        public TezWindow window { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.window.setFocusWidget(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.window.setFocusWidget(null);
        }

        protected override void onRefresh()
        {

        }

        public override bool checkForClose()
        {
            this.window.closePopup(this);
            return base.checkForClose();
        }

        public override void clear()
        {
            this.window = null;
        }

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {

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