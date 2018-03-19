using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat
{
    public abstract class TezGridScrollRectListener : TezInfiniteScrollRectListener
    {
        GridLayoutGroup m_LayoutGroup = null;

        int m_ColCount = 0;
        int m_RowCount = 0;

        /// <summary>
        /// 滚动起始时的本地坐标值
        /// </summary>
        protected Vector3 m_ContentStartLocalPosition = Vector3.zero;

        /// <summary>
        /// 总体偏移
        /// </summary>
        protected Vector2 m_TotalOffset = Vector2.zero;

        protected override Vector2 getItemSize()
        {
            return m_LayoutGroup.cellSize;
        }

        protected override void onGridLayoutGroup(GridLayoutGroup group)
        {
            m_LayoutGroup = group;
            var real_view_size = m_ViewRect.rect.size - new Vector2(m_Padding.left + m_Padding.right, m_Padding.bottom + m_Padding.top);
            var rc = real_view_size.divide(this.getItemSize() + m_Spacing);
            m_ColCount = Mathf.CeilToInt(rc.x);
            m_RowCount = Mathf.CeilToInt(rc.y);
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
            m_ContentStartLocalPosition = m_Content.localPosition;
        }

        public override void calculatePositionOnDrag(ref Bounds content_bounds, PointerEventData eventData)
        {
            ///计算当前的偏移量
            m_LocalPositionOffset = (m_Content.localPosition - m_ContentStartLocalPosition).toVector2();
//            Debug.Log(m_LocalPositionOffset);

            base.calculatePositionOnDrag(ref content_bounds, eventData);
        }

        protected override void onScrollArriveTopOrRight()
        {
            m_TotalOffset[m_Axis] += m_LocalPositionOffset[m_Axis];
            if (m_TotalOffset[m_Axis] < 0)
            {
                m_TotalOffset[m_Axis] = 0;
            }


            if (this.checkArriveTopOrRight())
            {
                this.updateOnArriveTopOrRightPosition();
            }
            else
            {
                m_TotalOffset[m_Axis] -= m_LocalPositionOffset[m_Axis];
            }
        }

        protected override void onScrollArriveBottomOrLeft()
        {
            m_TotalOffset[m_Axis] += m_LocalPositionOffset[m_Axis];
            if (m_TotalOffset[m_Axis] < 0)
            {
                m_TotalOffset[m_Axis] = 0;
            }

            if (this.checkArriveBottmOrLeft())
            {
                this.updateOnArriveBottomOrLeftPosition();
            }
            else
            {
                m_TotalOffset[m_Axis] -= m_LocalPositionOffset[m_Axis];
            }
        }

        private bool checkArriveTopOrRight()
        {
            var factor = m_TotalOffset[m_Axis] / this.getItemSize()[m_Axis];
            Debug.Log(factor);
            var vid = Mathf.CeilToInt(factor) - 1;

            switch (this.axis)
            {
                case Axis.Horizontal: return this.onScrollArriveRight(vid, m_RowCount);
                case Axis.Vertical: return this.onScrollArriveTop(vid, m_ColCount);
            }

            return false;
        }

        private bool checkArriveBottmOrLeft()
        {
            var factor = (m_TotalOffset[m_Axis] + m_ViewSize[m_Axis]) / this.getItemSize()[m_Axis];
            var vid = Mathf.CeilToInt(factor);

            switch (this.axis)
            {
                case Axis.Horizontal: return this.onScrollArriveLeft(vid, m_RowCount);
                case Axis.Vertical: return this.onScrollArriveBottom(vid, m_ColCount);
            }

            return false;
        }



        protected abstract bool onScrollArriveRight(int right_col_vid, int need_count);
        protected abstract bool onScrollArriveTop(int top_row_vid, int need_count);
        protected abstract bool onScrollArriveLeft(int left_col_vid, int need_count);
        protected abstract bool onScrollArriveBottom(int bottom_row_vid, int need_count);
    }

    public class TezGridScrollRectTeszListener : TezGridScrollRectListener
    {
        Text m_Prefab = null;
        Text prefab
        {
            get
            {
                if(m_Prefab == null)
                {
                    m_Prefab = GamePrefabManager.get<Text>("Text");
                }

                return m_Prefab;
            }
        }

        protected override bool onFindOutOfBottomOrLeftItem(RectTransform item)
        {
            Object.Destroy(item.gameObject);
            return true;
        }

        protected override bool onFindOutOfTopOrRightItem(RectTransform item)
        {
            Object.Destroy(item.gameObject);
            return true;
        }

        RectTransform create(int id, int self_id)
        {
            var go = Object.Instantiate(this.prefab);
            go.text = id + "-" + self_id;
            return (RectTransform)go.transform;
        }

        protected override bool onScrollArriveLeft(int left_col_vid, int need_count)
        {
            for (int i = need_count - 1; i >= 0; i--)
            {
                this.addItemAsFirstSibling(this.create(left_col_vid, i));
            }

            return true;
        }

        protected override bool onScrollArriveRight(int right_col_vid, int need_count)
        {
            for (int i = 0; i < need_count; i++)
            {
                this.addItemAsLastSibling(this.create(right_col_vid, i));
            }

            return true;
        }

        protected override bool onScrollArriveTop(int top_row_vid, int need_count)
        {
            if(top_row_vid < 0)
            {
                return false;
            }

            for (int i = need_count - 1; i >= 0; i--)
            {
                this.addItemAsFirstSibling(this.create(top_row_vid, i));
            }

            return true;
        }

        protected override bool onScrollArriveBottom(int bottom_row_vid, int need_count)
        {
            if (bottom_row_vid > 9)
            {
                return false;
            }

            for (int i = 0; i < need_count; i++)
            {
                this.addItemAsLastSibling(this.create(bottom_row_vid, i));
            }

            return true;
        }
    }
}