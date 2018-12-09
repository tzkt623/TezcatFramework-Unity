using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class TezVisualSelector : TezUIWidget
    {
        Image m_Icon = null;

        protected override void preInit()
        {
            m_Icon = this.GetComponent<Image>();

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

        public override void clear()
        {

        }

        private void onSelect(TezBasicSelector selector)
        {
            this.transform.position = Input.mousePosition;
            this.gameObject.SetActive(true);
            switch (selector.selectorType)
            {
                case TezSelectorType.Object:
                    break;
                case TezSelectorType.Item:
                    m_Icon.sprite = null;
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            this.transform.position = Input.mousePosition;
        }

        protected override void onRefresh(RefreshPhase phase)
        {
            switch (phase)
            {
                case RefreshPhase.Custom1:
                    break;
                case RefreshPhase.Custom2:
                    break;
                case RefreshPhase.Custom3:
                    break;
                case RefreshPhase.Custom4:
                    break;
                case RefreshPhase.Custom5:
                    break;
                default:
                    break;
            }
        }
    }
}