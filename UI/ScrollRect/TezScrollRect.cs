using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace tezcat
{
    public class TezScrollRect
        : ScrollRect
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        TezScrollRectListener m_Listener = new TezDefaultScrollRectListener();

        protected override void Start()
        {
            base.Start();
        }

        public void setListener(TezScrollRectListener listener)
        {
            if (listener == null)
            {
                m_Listener = new TezDefaultScrollRectListener();
            }
            else
            {
                m_Listener = listener;
            }

            m_Listener.setScrollRect(this, this.viewRect);

            var layout = this.content.GetComponent<LayoutGroup>();
            if (layout)
            {
                m_Listener.calculateLayout(layout);
            }
            else
            {
                throw new System.Exception();
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            m_Listener.onBeginScroll();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            m_Listener.onScroll(ref m_ContentBounds, eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            m_Listener.onScroll(ref m_ContentBounds, eventData);
        }

#if Test
        Vector2 getSize(RectTransform transform)
        {
            return transform.rect.size;
        }

        void onScrollArrivedTop(RectTransform[] array)
        {
            foreach (var item in array)
            {
                item.SetAsFirstSibling();
            }
        }

        void onScrollArrivedBottom(RectTransform[] array)
        {

        }

        void onScrollArrivedLeft(RectTransform[] array)
        {
            foreach (var item in array)
            {
                item.SetAsFirstSibling();
            }
        }

        void onScrollArrivedRight(RectTransform[] array)
        {

        }

        void calculatePositionOnDrag(PointerEventData eventData)
        {
            ///
            var view_pivot = this.viewRect.pivot;

            ///计算位置偏移
            m_Delta.y = this.vertical ? eventData.delta.y : 0;
            m_Delta.x = this.horizontal ? eventData.delta.x : 0;
            var my_anchored_position = this.content.anchoredPosition + m_Delta;

            ///并计算出当前真实锚点的标准值
            var content_pivot = this.content.pivot;
            var content_real_anchor = this.content.anchorMax - this.content.anchorMin;
            if (content_real_anchor == Vector2.zero)
            {
                content_real_anchor = this.content.anchorMax;
            }
            else
            {
                content_real_anchor = Vector2.Scale(content_real_anchor, content_pivot) + this.content.anchorMin;
            }

            ///计算当前锚点到rect左下角的大小
            var view_size = this.getSize(this.viewRect);
            m_RemainingSize = m_ContentBounds.size.toVector2() - view_size;
            var anchor_rect_size = Vector2.Scale(content_real_anchor, view_size);

            bool fix = false;
        #region Vertical
            if (this.vertical)
            {
                var factor_y = (view_size.y + m_RemainingSize.y) * content_pivot.y;
                if (my_anchored_position.y < (-m_RemainingSize.y - anchor_rect_size.y) + factor_y)
                {
                    ///滚动到顶
                    var array = m_Listener.onArrivedTop(m_Row * m_Col, m_TopRowID);
                    if (array.Length > 0)
                    {
                        this.onScrollArrivedTop(array);

                        m_TopRowID -= 1;
                        var rt = array[0].rect;
                        float y = rt.height + m_Spacing.y;
                        m_RemainingSize.y += y;
                        my_anchored_position.y += y * content_pivot.y;

                        fix = true;
                    }
                }
                else if (my_anchored_position.y > -anchor_rect_size.y + factor_y)
                {
                    ///滚动到底
                    var array = m_Listener.onArrivedBottom(m_Row * m_Col, m_BottomRowID);
                    if (array.Length > 0)
                    {
                        m_BottomRowID += 1;
                        this.onScrollArrivedBottom(array);
                        var rt = array[0].rect;

                        float y = rt.height + m_Spacing.y;
                        m_RemainingSize.y += y;
                        my_anchored_position.y += -y * (1 - content_pivot.y);

                        fix = true;
                    }
                }
                else
                {
                    bool remove_top = false, remove_bottom = false;
                    foreach (RectTransform item in this.content)
                    {
                        var view_local_position = item.localPosition + this.content.localPosition;
                        var height = item.rect.height;
                        if (view_local_position.y < -view_pivot.y * view_size.y - height)
                        {
                            if (m_Listener.onRemoveBottom(item) && !remove_bottom)
                            {
                                remove_bottom = true;

                                float y = height + m_Spacing.y;
                                m_RemainingSize.y -= y;
                                my_anchored_position.y += y * (1 - content_pivot.y);

                                fix = true;
                            }
                        }
                        else if (view_local_position.y > (1 - view_pivot.y) * view_size.y + height)
                        {
                            if (m_Listener.onRemoveTop(item) && !remove_top)
                            {
                                remove_top = true;

                                float y = height + m_Spacing.y;
                                m_RemainingSize.y -= y;
                                my_anchored_position.y += -y * content_pivot.y;

                                fix = true;
                            }
                        }
                    }
                }
            }
        #endregion

        #region Horizontal
            if (this.horizontal)
            {
                var factor_x = (view_size.x + m_RemainingSize.x) * content_pivot.x;
                if (my_anchored_position.x < (-m_RemainingSize.x - anchor_rect_size.x) + factor_x)
                {
                    ///滚动到右
                    var array = m_Listener.onArrivedRight(m_Row * m_Col, m_Col);
                    if (array.Length > 0)
                    {
                        this.onScrollArrivedRight(array);
                        var rt = array[0].rect;

                        float x = rt.width + m_Spacing.x;
                        m_RemainingSize.x += x;
                        my_anchored_position.x += x * content_pivot.x;

                        fix = true;
                    }
                }
                else if (my_anchored_position.x > -anchor_rect_size.x + factor_x)
                {
                    ///滚动到左
                    var array = m_Listener.onArrivedLeft(m_Row * m_Col, m_Col);
                    if (array.Length > 0)
                    {
                        this.onScrollArrivedLeft(array);
                        var rt = array[0].rect;

                        float x = rt.width + m_Spacing.x;
                        m_RemainingSize.x += x;
                        my_anchored_position.x += -x * (1 - content_pivot.x);

                        fix = true;
                    }
                }
                else
                {
                    bool remove_left = false, remove_right = false;
                    foreach (RectTransform item in this.content)
                    {
                        var view_local_position = item.localPosition + this.content.localPosition;
                        var width = item.rect.width;
                        if (view_local_position.x < -view_pivot.x * view_size.x - width)
                        {
                            if (m_Listener.onRemoveLeft(item) && !remove_left)
                            {
                                remove_left = true;
                                float x = width + m_Spacing.x;
                                m_RemainingSize.x -= x;
                                my_anchored_position.x += x * (1 - content_pivot.x);
                                fix = true;
                            }
                        }
                        else if (view_local_position.x > (1 - view_pivot.x) * view_size.x + width)
                        {
                            if (m_Listener.onRemoveRight(item) && !remove_right)
                            {
                                remove_right = true;
                                float x = width + m_Spacing.x;
                                m_RemainingSize.x -= x;
                                my_anchored_position.x += -x * content_pivot.x;
                                fix = true;
                            }
                        }
                    }
                }
            }
        #endregion

            if (fix)
            {
                this.content.anchoredPosition = my_anchored_position;
                ///为什么要调这个函数
                ///因为unity把一个关键的参数m_PointerStartLocalCursor写成了private
                this.OnBeginDrag(eventData);
            }
        }
#endif
        protected override void LateUpdate()
        {
            base.LateUpdate();
            m_Listener.update();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }

    }
}

