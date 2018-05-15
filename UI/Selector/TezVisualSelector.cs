using tezcat.Core;
using tezcat.DataBase;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.UI
{
    [RequireComponent(typeof(Image))]
    public class TezVisualSelector : TezWidget
    {
        Image m_Icon = null;

        protected override void preInit()
        {
            m_Icon = this.GetComponent<Image>();

            TezSelectController.onSelect.add(onSelect);
            TezSelectController.onCancelSelect.add(onCancelSelect);

            this.gameObject.SetActive(false);
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

        private void onCancelSelect(TezBasicSelector selector)
        {
            m_Icon.sprite = null;
            this.gameObject.SetActive(false);
        }

        protected override void clear()
        {
            TezSelectController.onSelect.remove(onSelect);
            TezSelectController.onCancelSelect.remove(onCancelSelect);
        }

        protected override void onRefresh()
        {

        }

        private void onSelect(TezBasicSelector selector)
        {
            this.transform.position = Input.mousePosition;
            this.gameObject.SetActive(true);
            switch (selector.selectType)
            {
                case TezSelectType.Object:
                    break;
                case TezSelectType.Item:
                    m_Icon.sprite = TezTextureManager.getSprite(((TezItemSelector)selector).convertItem<TezItem>().asset.icon_0);
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            this.transform.position = Input.mousePosition;
        }
    }
}