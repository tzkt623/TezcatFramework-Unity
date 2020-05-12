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
            this.gameObject.SetActive(false);
        }

        protected override void onHide()
        {
            this.singleItem = null;
            m_Icon.color = Color.gray;
        }

        protected override void onShow()
        {
            this.transform.position = Input.mousePosition;
            if (this.singleItem != null)
            {
                m_Icon.color = Color.green;
            }
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
    }
}