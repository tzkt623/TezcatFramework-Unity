using System.Collections.Generic;
using tezcat.Framework.Database;
using UnityEngine;

namespace tezcat.Framework.UI
{
    /// <summary>
    /// 浮动Tip
    /// </summary>
    public class TezFloatingTip
        : TezUIWidget
        , ITezSinglePrefab
    {
        [SerializeField]
        protected RectTransform m_Content = null;
        [SerializeField]
        Vector2 m_BottomLeft = new Vector2(-8, -24);
        [SerializeField]
        Vector2 m_TopRight = new Vector2(32, 8);

        RectTransform m_RectTransform = null;
        Vector2 m_Pivot = new Vector2(0, 1);

        List<ITezBaseWidget> m_Widgets = new List<ITezBaseWidget>();

        protected override void initWidget()
        {
            m_RectTransform = this.GetComponent<RectTransform>();
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 添加一个控件
        /// </summary>
        public virtual void addWidget(ITezBaseWidget widget)
        {
            m_Widgets.Add(widget);
        }

        protected override void onHide()
        {
            foreach (var widget in m_Widgets)
            {
                widget.close();
            }
            m_Widgets.Clear();
        }

        protected override void onShow()
        {
            this.calculatePosition();
        }

        private void Update()
        {
            this.calculatePosition();
        }

        private void calculatePosition()
        {
            /*
             * (0, 1080)-------------(1920, 1080)
             * 
             * 
             * 
             * 
             * (0,    0)-------------(1920, 0)
             */
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