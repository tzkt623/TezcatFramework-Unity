using System;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public class TezNewTitle
        : TezWidget
        , ITezClickable
    {
        TezWindow m_Window = null;

        protected override void preInit()
        {

        }

        protected override void initWidget()
        {
            m_Window = this.transform.parent.GetComponent<TezWindow>();
            if (m_Window == null)
            {
                throw new ArgumentNullException("ParenWidget Not Found");
            }
        }

        protected override void linkEvent()
        {

        }

        protected override void unLinkEvent()
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

        public override void clear()
        {

        }

        public override void reset()
        {

        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            m_Window.setAsTopWindow();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}