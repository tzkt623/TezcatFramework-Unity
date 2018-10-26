﻿using tezcat.Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace tezcat.Framework.UI
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

        protected override void onOpenAndRefresh()
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
            TezSelectController.onSelect.remove(onSelect);
            TezSelectController.onCancelSelect.remove(onCancelSelect);
        }

        protected override void refreshAfterInit()
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
                case RefreshPhase.System1:
                    break;
                case RefreshPhase.System2:
                    break;
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