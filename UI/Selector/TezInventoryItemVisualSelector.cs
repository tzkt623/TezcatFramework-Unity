using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Game.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class TezInventoryItemVisualSelector
        : TezUIWidget
        , ITezInventoryVisualSelector
    {
        Image m_Icon = null;

        public TezGameObject singleItem { get; private set; }

        protected override void preInit()
        {
            m_Icon = this.GetComponent<Image>();
        }

        protected override void onHide()
        {
            this.singleItem = null;
            m_Icon.color = Color.gray;
        }

        public void onSelect(TezGameObject item)
        {
            this.singleItem = item;
            this.open();
        }

        public void onComplete()
        {
            this.hide();
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
                    if (this.singleItem != null)
                    {
                        m_Icon.color = Color.green;
                    }
                    break;
            }
        }
    }
}