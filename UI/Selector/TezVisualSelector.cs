using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class TezVisualSelector : TezUIWidget
    {
        Image m_Icon = null;

        public TezGameObject singleItem { get; private set; }

        protected override void preInit()
        {
            m_Icon = this.GetComponent<Image>();
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

        protected override void onHide()
        {
            this.singleItem = null;
            m_Icon.color = Color.gray;
        }

        public override void reset()
        {

        }

        public void onSelect(TezGameObject item)
        {
            this.singleItem = item;
        }

        protected override void onClose()
        {

        }

        public void Update()
        {
            this.transform.position = Input.mousePosition;
        }

        protected override void onRefresh(TezRefreshPhase phase)
        {
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    this.gameObject.SetActive(false);
                    break;
                case TezRefreshPhase.P_OnEnable:
                    this.transform.position = Input.mousePosition;
                    if(this.singleItem != null)
                    {
                        m_Icon.color = Color.green;
                    }
                    break;
            }
        }
    }
}