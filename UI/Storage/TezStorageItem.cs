using tezcat.Wrapper;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    [RequireComponent(typeof(Image))]
    public class TezStorageItem
        : TezWidget
        , ITezWrapperBinder<ITezItemWrapper>
    {
        Image m_Image = null;

        protected override void clear()
        {
            m_Image = null;
        }

        protected override void onRefresh()
        {

        }

        public void bind(ITezItemWrapper wrapper)
        {

        }

        protected override void preInit()
        {
            m_Image = this.GetComponent<Image>();
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

        public override void reset()
        {

        }

        protected override void onShow()
        {

        }

        protected override void onHide()
        {

        }
    }
}
