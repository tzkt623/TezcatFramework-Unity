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

        protected override void Awake()
        {
            base.Awake();
            m_Image = this.GetComponent<Image>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_Image = null;
        }

        protected override void clear()
        {

        }

        protected override void onRefresh()
        {

        }

        public void bind(ITezItemWrapper wrapper)
        {

        }
    }
}
