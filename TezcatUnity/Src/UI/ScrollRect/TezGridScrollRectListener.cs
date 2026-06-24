using tezcat.Framework.Extension;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat.Unity.UI
{
    public abstract class TezGridScrollRectListener : TezInfiniteScrollRectListener
    {
        GridLayoutGroup m_LayoutGroup = null;

        /// <summary>
        /// 可见的排数或者列数
        /// </summary>
        protected int viewCount
        {
            get { return m_Capacity[m_Axis]; }
        }

        /// <summary>
        /// 填充量
        /// </summary>
        protected int needCount
        {
            get { return m_Capacity[1 - m_Axis]; }
        }

        /// <summary>
        ///V方向
        ///既 m_Axis == 1
        ///那么x(Col)既是创建数量
        ///y(Row)既是最大可见数量
        ///
        ///H方向
        ///既 m_Axis == 0
        ///那么y(Row)既是创建数量
        ///x(Col)既是最大可见数量
        /// </summary>
        protected Vector2Int m_Capacity = Vector2Int.zero;


        /// <summary>
        /// 滚动起始时的本地坐标值
        /// </summary>
        protected Vector3 m_ContentStartLocalPosition = Vector3.zero;

        /// <summary>
        /// 总体偏移
        /// </summary>
        protected Vector2 m_TotalOffset = Vector2.zero;

        int m_OldTop = 0;
        int m_OldBottom = 0;

        protected override Vector2 getItemSize()
        {
            return m_LayoutGroup.cellSize;
        }

        protected override void onGridLayoutGroup(GridLayoutGroup group)
        {
            m_LayoutGroup = group;
            var real_view_size = m_ViewRect.rect.size - new Vector2(m_Padding.left + m_Padding.right, m_Padding.bottom + m_Padding.top);
            var rc = real_view_size.divide(this.getItemSize() + m_Spacing);
            m_Capacity = Vector2Int.CeilToInt(rc);
        }

        protected override void onHorizontalLayoutGroup(HorizontalLayoutGroup group)
        {
            throw new System.NotImplementedException("This Layout can not support gird scroll");
        }

        protected override void onVerticalLayoutGroup(VerticalLayoutGroup group)
        {
            throw new System.NotImplementedException("This Layout can not support gird scroll");
        }

        public override void onBeginScroll()
        {

        }

        public override void onScroll(ref Bounds content_bounds, PointerEventData eventData)
        {
            base.onScroll(ref content_bounds, eventData);
        }

        protected override void onScrollArriveTopOrRight()
        {
            if (this.checkArriveTopOrRight())
            {
                this.addSpaceToTopOrRight();
            }
        }

        protected override void onScrollArriveBottomOrLeft()
        {
            if (this.checkArriveBottmOrLeft())
            {
                this.addSpaceToBottomOrLeft();
            }
        }

        private bool checkArriveTopOrRight()
        {
            bool result = false;
            switch (this.axis)
            {
                case Axis.Horizontal: result = this.onScrollArriveRight(this.needCount); break;
                case Axis.Vertical: result = this.onScrollArriveTop(this.needCount); break;
            }

            return result;
        }

        private bool checkArriveBottmOrLeft()
        {
            bool result = false;
            switch (this.axis)
            {
                case Axis.Horizontal: result = this.onScrollArriveLeft(this.needCount); break;
                case Axis.Vertical: result = this.onScrollArriveBottom(this.needCount); break;
            }

            return result;
        }

        protected abstract bool onScrollArriveRight(int need_count);
        protected abstract bool onScrollArriveTop(int need_count);
        protected abstract bool onScrollArriveLeft(int need_count);
        protected abstract bool onScrollArriveBottom(int need_count);
    }

    /// <summary>
    /// 已知的BUG是有一排没有删掉导致显示ID重复
    /// </summary>
    public abstract class TezIDGridScrollRectListener : TezGridScrollRectListener
    {
        int m_BeginID = 0;
        int m_EndID = 0;

        bool calculateBegin()
        {
            bool result = true;
            m_BeginID -= 1;
            if (m_BeginID <= -1)
            {
                m_BeginID = 0;
                result = false;
            }
            m_EndID = m_BeginID + this.viewCount - 1;
            return result;
        }

        bool calculateEnd()
        {
            bool result = true;
            m_EndID += 1;
            if (m_EndID >= this.getMaxRowOrCol())
            {
                m_EndID = this.getMaxRowOrCol() - 1;
                result = false;
            }

            m_BeginID = m_EndID - this.viewCount + 1;
            return result;
        }

        protected override void init()
        {
            m_BeginID = 0;
            m_EndID = this.viewCount - 1;
        }

        private int getMaxRowOrCol()
        {
            return Mathf.CeilToInt(this.getCount() / (float)this.needCount);
        }

        protected abstract int getCount();

        protected sealed override bool onScrollArriveLeft(int need_count)
        {
            return false;
        }

        protected sealed override bool onScrollArriveRight(int need_count)
        {
            return false;
        }

        protected sealed override bool onScrollArriveTop(int need_count)
        {
            if (!this.calculateBegin())
            {
                return false;
            }

            var end = (m_BeginID * need_count) - 1;
            var begin = end + need_count;
            for (int i = begin; i > end; i--)
            {      
                this.addItemAsFirstSibling(this.onFillItemAsFirst(i));
            }

            return true;
        }

        protected sealed override bool onScrollArriveBottom(int need_count)
        {
            if (!this.calculateEnd())
            {
                return false;
            }

            var begin = m_EndID * need_count;
            var end = Mathf.Min(begin + need_count, this.getCount());
            for (int i = begin; i < end; i++)
            {
                this.addItemAsLastSibling(this.onFillItemAsLast(i));
            }

            return true;
        }

        protected abstract RectTransform onFillItemAsFirst(int index);
        protected abstract RectTransform onFillItemAsLast(int index);
    }
}