using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Framework.UI
{
    public abstract class TezScrollRectListener
    {
        public enum Axis : int
        {
            Horizontal = 0,
            Vertical
        }

        /// <summary>
        /// 滑动轴向
        /// </summary>
        protected int m_Axis = 0;
        protected Axis axis
        {
            get { return (Axis)m_Axis; }
            set { m_Axis = (int)value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected RectOffset m_Padding = null;

        /// <summary>
        /// 
        /// </summary>
        protected Vector2 m_Spacing = Vector2.zero;

        /// <summary>
        /// 可视区域
        /// </summary>
        protected RectTransform m_ViewRect = null;
        /// <summary>
        /// 内容区域
        /// </summary>
        protected RectTransform m_Content = null;

        /// <summary>
        /// 整体
        /// </summary>
        protected TezScrollRect m_ScrollRect = null;


        public void setScrollRect(TezScrollRect sr, RectTransform view_rect)
        {
            m_ScrollRect = sr;
            this.m_ViewRect = view_rect;
            this.m_Content = m_ScrollRect.content;
        }

        public abstract void onBeginScroll();
        public abstract void onScroll(ref Bounds content_bounds, PointerEventData eventData);

        protected void addItemAsFirstSibling(RectTransform item)
        {
            item.SetParent(this.m_Content);
            item.SetAsFirstSibling();
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;
        }

        protected void addItemAsLastSibling(RectTransform item)
        {
            item.SetParent(this.m_Content);
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;
        }

        protected void insertItemAtSibling(RectTransform item, int sibling)
        {
            item.SetParent(this.m_Content);
            item.SetSiblingIndex(sibling);
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;
        }

        public void calculateLayout(LayoutGroup layout)
        {
            m_Padding = layout.padding != null ? layout.padding : new RectOffset();
            if (layout.GetType() == typeof(HorizontalLayoutGroup))
            {
                m_Spacing.x = ((HorizontalLayoutGroup)layout).spacing;
                m_Spacing.y = 0;
                this.onHorizontalLayoutGroup((HorizontalLayoutGroup)layout);
            }
            else if (layout.GetType() == typeof(VerticalLayoutGroup))
            {
                m_Spacing.x = 0;
                m_Spacing.y = ((VerticalLayoutGroup)layout).spacing;
                this.onVerticalLayoutGroup((VerticalLayoutGroup)layout);
            }
            else if (layout.GetType() == typeof(GridLayoutGroup))
            {
                m_Spacing = ((GridLayoutGroup)layout).spacing;
                this.onGridLayoutGroup((GridLayoutGroup)layout);
            }

            this.init();
        }

        protected abstract void init();

        protected abstract void onHorizontalLayoutGroup(HorizontalLayoutGroup group);
        protected abstract void onVerticalLayoutGroup(VerticalLayoutGroup group);
        protected abstract void onGridLayoutGroup(GridLayoutGroup group);
    }

    public class TezDefaultScrollRectListener : TezScrollRectListener
    {
        protected override void init()
        {

        }

        public override void onBeginScroll()
        {

        }

        public override void onScroll(ref Bounds content_bounds, PointerEventData eventData)
        {

        }

        protected override void onGridLayoutGroup(GridLayoutGroup group)
        {

        }

        protected override void onHorizontalLayoutGroup(HorizontalLayoutGroup group)
        {

        }

        protected override void onVerticalLayoutGroup(VerticalLayoutGroup group)
        {

        }
    }
}