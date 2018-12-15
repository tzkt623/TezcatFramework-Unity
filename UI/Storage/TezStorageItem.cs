using tezcat.Framework.Core;
using tezcat.Framework.Wrapper;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class TezStorageItem
        : TezToolWidget
        , ITezWrapperBinder<ITezItemWrapper>
    {
        Image m_Image = null;

        public override void clear()
        {
            m_Image = null;
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

        protected override void onHide()
        {

        }

        protected override void onRefresh(TezRefreshPhase phase)
        {

        }
    }
}
