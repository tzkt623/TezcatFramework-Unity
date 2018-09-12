using UnityEngine;
using UnityEngine.EventSystems;

namespace tezcat.UI
{
    public abstract class TezInfiniteScrollRectListener : TezScrollRectListener
    {
        bool m_Dirty = false;

        /// <summary>
        /// 鼠标拖动帧偏移或滚轮值
        /// </summary>
        protected Vector2 m_Delta = Vector2.zero;

        /// <summary>
        /// ContentSize - ViewSzie的剩余大小
        /// </summary>
        protected Vector2 m_RemainingSize = Vector2.zero;

        /// <summary>
        /// 铆钉的标准点
        /// </summary>
        protected Vector2 m_MyAnchoredPosition = Vector2.zero;

        /// <summary>
        /// view pivot
        /// </summary>
        protected Vector2 m_ViewPiovt = Vector2.zero;

        /// <summary>
        /// view的大小
        /// </summary>
        protected Vector2 m_ViewSize = Vector2.zero;

        /// <summary>
        /// content的pivot
        /// </summary>
        protected Vector2 m_ContentPivot = Vector2.zero;

        /// <summary>
        /// content的大小
        /// </summary>
        protected Vector2 m_AnchorRectSize = Vector2.zero;

        /// <summary>
        /// content当前LocalPosition的offset
        /// </summary>
        protected Vector2 m_LocalPositionOffset = Vector2.zero;


        protected abstract Vector2 getItemSize();

        /// <summary>
        /// 在顶部或者右部增加空间
        /// </summary>
        protected void addSpaceToTopOrRight()
        {
            var offset = (this.getItemSize()[m_Axis] + m_Spacing[m_Axis]) * m_ContentPivot[m_Axis];
            m_MyAnchoredPosition[m_Axis] += offset;
            m_Dirty = true;
        }

        /// <summary>
        /// 在底部或者左部增加空间
        /// </summary>
        protected void addSpaceToBottomOrLeft()
        {
            var offset = -(this.getItemSize()[m_Axis] + m_Spacing[m_Axis]) * (1 - m_ContentPivot[m_Axis]);
            m_MyAnchoredPosition[m_Axis] += offset;
            m_Dirty = true;
        }

        /// <summary>
        /// 在顶部或者右部减少空间
        /// </summary>
        protected void removeSpaceFromTopOrRight()
        {
            m_MyAnchoredPosition[m_Axis] += -(this.getItemSize()[m_Axis] + m_Spacing[m_Axis]) * m_ContentPivot[m_Axis];
            m_Dirty = true;
        }

        /// <summary>
        /// 在底部或者左部减少空间
        /// </summary>
        protected void removeSpaceFromBottomOrLeft()
        {
            m_MyAnchoredPosition[m_Axis] += (this.getItemSize()[m_Axis] + m_Spacing[m_Axis]) * (1 - m_ContentPivot[m_Axis]);
            m_Dirty = true;
        }

        /// <summary>
        /// 当滚动达到Top Or Right时,调用此函数
        /// </summary>
        protected abstract void onScrollArriveTopOrRight();

        /// <summary>
        /// 当滚动达到Bottom Or Left时,调用此函数
        /// </summary>
        protected abstract void onScrollArriveBottomOrLeft();

        /// <summary>
        /// 如果找到一个在View范围之外的Item,则调用此函数
        /// </summary>
        /// <param name="item">范围外的Item</param>
        /// <returns>True则会删除此元素所占的空间 False表示不处理此元素所占的空间</returns>
        protected abstract bool onFindOutOfTopOrRightItem(RectTransform item);

        /// <summary>
        /// 如果找到一个在View范围之外的Item,则调用此函数
        /// </summary>
        /// <param name="item">范围外的Item</param>
        /// <returns>True则会删除此元素所占的空间 False表示不处理此元素所占的空间</returns>
        protected abstract bool onFindOutOfBottomOrLeftItem(RectTransform item);

        public override void onScroll(ref Bounds content_bounds, PointerEventData eventData)
        {
            ///
            m_ViewPiovt = m_ViewRect.pivot;

            ///计算位置偏移
            m_Delta.y = m_ScrollRect.vertical ? eventData.delta.y : 0;
            m_Delta.x = m_ScrollRect.horizontal ? eventData.delta.x : 0;
            m_MyAnchoredPosition = m_Content.anchoredPosition + m_Delta;

            ///并计算出当前真实锚点的标准值
            m_ContentPivot = m_Content.pivot;
            var content_real_anchor = m_Content.anchorMax - m_Content.anchorMin;
            if (content_real_anchor == Vector2.zero)
            {
                content_real_anchor = m_Content.anchorMax;
            }
            else
            {
                content_real_anchor = Vector2.Scale(content_real_anchor, m_ContentPivot) + this.m_Content.anchorMin;
            }

            ///计算当前锚点到rect左下角的大小
            m_ViewSize = m_ViewRect.rect.size;
            m_AnchorRectSize = Vector2.Scale(content_real_anchor, m_ViewSize);

            ///
            m_RemainingSize = m_Content.rect.size - m_ViewSize;

            #region Vertical
            if (m_ScrollRect.vertical)
            {
                this.axis = Axis.Vertical;
                this.calculatePosition();
            }
            #endregion

            #region Horizontal
            if (m_ScrollRect.horizontal)
            {
                this.axis = Axis.Horizontal;
                this.calculatePosition();
            }
            #endregion

            if (m_Dirty)
            {
                this.m_Content.anchoredPosition = (m_MyAnchoredPosition - m_Delta);
                ///为什么要调这个函数
                ///因为unity把一个关键的参数m_PointerStartLocalCursor写成了private
                m_Dirty = false;
            }

            m_ScrollRect.OnBeginDrag(eventData);
        }

        /// <summary>
        /// 计算content的位置
        /// </summary>
        /// <returns></returns>
        void calculatePosition()
        {
            var factor = (m_ViewSize[m_Axis] + m_RemainingSize[m_Axis]) * m_ContentPivot[m_Axis];
            if (m_MyAnchoredPosition[m_Axis] < (-m_RemainingSize[m_Axis] - m_AnchorRectSize[m_Axis]) + factor)
            {
                this.onScrollArriveTopOrRight();
            }
            else if (m_MyAnchoredPosition[m_Axis] > -m_AnchorRectSize[m_Axis] + factor)
            {
                this.onScrollArriveBottomOrLeft();
            }
            else
            {
                this.findOutOfRangeItem();
            }
        }

        void findOutOfRangeItem()
        {
            bool removed_bl = false, removed_tr = false;
            foreach (RectTransform item in this.m_Content)
            {
                var in_view_local_position = item.localPosition + this.m_Content.localPosition;
                var size = this.getItemSize()[m_Axis];
                if (in_view_local_position[m_Axis] < -(m_ViewPiovt[m_Axis] * m_ViewSize[m_Axis] + size))
                {
                    if (this.onFindOutOfBottomOrLeftItem(item) && !removed_bl)
                    {
                        removed_bl = true;
                        this.removeSpaceFromBottomOrLeft();
                    }
                }
                else if (in_view_local_position[m_Axis] > (1 - m_ViewPiovt[m_Axis]) * m_ViewSize[m_Axis] + size)
                {
                    if (this.onFindOutOfTopOrRightItem(item) && !removed_tr)
                    {
                        removed_tr = true;
                        this.removeSpaceFromTopOrRight();
                    }
                }
            }
        }
    }
}