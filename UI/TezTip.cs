using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using UnityEngine;

namespace tezcat.Framework.UI
{
    /*
     * (0, 1080)-------------(1920, 1080)
     * 
     * 
     * 
     * 
     * (0,    0)-------------(1920, 0)
     */
    public class TezTip
        : TezUIWidget
        , ITezSinglePrefab
    {
        RectTransform m_RectTransform = null;
        [SerializeField]
        protected RectTransform m_Content = null;
        [SerializeField]
        Vector2 m_BottomLeft = new Vector2(-8, -24);
        [SerializeField]
        Vector2 m_TopRight = new Vector2(32, 8);

        Vector2 m_Pivot = new Vector2(0, 1);
        List<ITezWidget> m_Widgets = new List<ITezWidget>();

        protected override void initWidget()
        {
            base.initWidget();
            m_RectTransform = this.GetComponent<RectTransform>();
        }

        public void addWidget(ITezWidget widget)
        {
            m_Widgets.Add(widget);
        }

        protected override void onHide()
        {
            base.onHide();
            foreach (var widget in m_Widgets)
            {
                widget.close();
            }
            m_Widgets.Clear();
        }

        protected override void onRefresh(TezRefreshPhase phase)
        {
            switch (phase)
            {
                case TezRefreshPhase.P_OnInit:
                    this.gameObject.SetActive(false);
                    break;
                case TezRefreshPhase.P_OnEnable:
                    this.calculatePosition();
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            this.calculatePosition();
        }

        private void calculatePosition()
        {
            var position = Input.mousePosition;

            var rect = m_RectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            if (position.x + width >= Screen.width)
            {
                m_Pivot.x = 1;
                position.x += m_BottomLeft.x;
            }
            else
            {
                m_Pivot.x = 0;
                position.x += m_TopRight.x;
            }

            if (position.y - height <= 0)
            {
                m_Pivot.y = 0;
                position.y += m_TopRight.y;
            }
            else
            {
                m_Pivot.y = 1;
                position.y += m_BottomLeft.y;
            }

            m_RectTransform.pivot = m_Pivot;
            m_RectTransform.position = position;
        }
    }
}