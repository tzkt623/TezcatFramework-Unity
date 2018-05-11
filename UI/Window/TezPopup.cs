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

        protected override bool onClose()
        {
            this.window.closePopup(this);
            return base.onClose();
        }

        protected override void clear()
        {
            this.window = null;
        }
    }
}