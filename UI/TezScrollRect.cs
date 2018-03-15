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
        Queue<RectTransform> m_FreeTransform = new Queue<RectTransform>();

        bool m_Entered = false;
        Camera m_Camera = null;
        Vector2 m_CurrentPos = Vector2.zero;

        enum LayoutType
        {
            H, V, G
        }
        LayoutType m_LayoutType = LayoutType.H;
        HorizontalLayoutGroup m_HLayout = null;
        VerticalLayoutGroup m_VLayout = null;
        GridLayoutGroup m_GLayout = null;

        RectOffset m_Padding = null;
        Vector2 m_Spacing = Vector2.zero;
        Vector2 m_RemainingSize = Vector2.zero;
        Vector2 m_Delta = Vector2.zero;

        int m_Index = 0;

        void calculateIndex()
        {

        }

        protected override void Start()
        {
            base.Start();

            var layout = this.content.GetComponent<LayoutGroup>();
            if (layout)
            {
                if (layout.GetType() == typeof(HorizontalLayoutGroup))
                {
                    m_HLayout = (HorizontalLayoutGroup)layout;
                    m_LayoutType = LayoutType.H;
                    m_Spacing.x = m_HLayout.spacing;
                }
                else if (layout.GetType() == typeof(VerticalLayoutGroup))
                {
                    m_VLayout = (VerticalLayoutGroup)layout;
                    m_LayoutType = LayoutType.V;
                    m_Spacing.y = m_VLayout.spacing;
                }
                else if (layout.GetType() == typeof(GridLayoutGroup))
                {
                    m_GLayout = (GridLayoutGroup)layout;
                    m_LayoutType = LayoutType.G;
                    m_Spacing = m_GLayout.spacing;
                }

                m_Padding = layout.padding != null ? layout.padding : new RectOffset();
            }
            else
            {
                throw new System.Exception();
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            this.calculatePositionOnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }

        public override void OnScroll(PointerEventData data)
        {
            base.OnScroll(data);
        }

        Vector2 getSize(RectTransform transform)
        {
            return transform.rect.size;
        }

        RectTransform createItem(RectTransform parent)
        {
            if(m_FreeTransform.Count > 0)
            {
                var rect = m_FreeTransform.Dequeue();
                rect.SetParent(parent);
                rect.gameObject.SetActive(true);
                return rect;
            }

            var go = new GameObject();
            go.AddComponent<Image>().color = Random.ColorHSV();
            go.transform.SetParent(parent, false);
            go.name = m_Index++.ToString();
            return (RectTransform)go.transform;
        }

        List<RectTransform> onScrollArrivedBottom()
        {
            var rect = this.createItem(this.content);
            return new List<RectTransform>() { rect };
        }

        List<RectTransform> onScrollArrivedTop()
        {
            var rect = this.createItem(this.content);
            rect.SetAsFirstSibling();
            return new List<RectTransform>() { rect };
        }

        List<RectTransform> onScrollArrivedLeft()
        {
            var rect = this.createItem(this.content);
            rect.SetAsFirstSibling();
            return new List<RectTransform>() { rect };
        }

        List<RectTransform> onScrollArrivedRight()
        {
            var rect = this.createItem(this.content);
            return new List<RectTransform>() { rect };
        }

        void calculatePositionOnDrag(PointerEventData eventData)
        {
            bool fix = false;

            ///初始化剩余大小
            if (m_RemainingSize == Vector2.zero)
            {
                var content_size = this.getSize(this.content);
                if (content_size.x != 0 && content_size.y != 0)
                {
                    m_RemainingSize = content_size - this.getSize(this.viewRect);
                }
            }

            ///
            var view_pivot = this.viewRect.pivot;

            ///计算位置偏移
            m_Delta.y = this.vertical ? eventData.delta.y : 0;
            m_Delta.x = this.horizontal ? eventData.delta.x : 0;
            var my_anchored_position = this.content.anchoredPosition + m_Delta;

            ///并计算出当前真实锚点的标准值
            var content_pivot = this.content.pivot;
            var content_anchor = this.content.anchorMax - this.content.anchorMin;
            if (content_anchor == Vector2.zero)
            {
                content_anchor = this.content.anchorMax;
            }
            else
            {
                content_anchor = Vector2.Scale(content_anchor, content_pivot) + this.content.anchorMin;
            }

            ///计算当前锚点到rect左下角的大小
            var view_size = this.getSize(this.viewRect);
            var anchor_rect_size = Vector2.Scale(content_anchor, view_size);

            if (this.vertical)
            {
                var factor_y = (view_size.y + m_RemainingSize.y) * content_pivot.y;
                if (my_anchored_position.y < (-m_RemainingSize.y - anchor_rect_size.y) + factor_y)
                {
                    Debug.Log("到顶");
                    var list = this.onScrollArrivedTop();
                    if (list.Count > 0)
                    {
                        var rt = list[0].rect;

                        float y = rt.height + m_Spacing.y;
                        m_RemainingSize.y += y;
                        my_anchored_position.y += y * content_pivot.y;

                        fix = true;
                    }
                }
                else if (my_anchored_position.y > -anchor_rect_size.y + factor_y)
                {
                    Debug.Log("到底");
                    var list = this.onScrollArrivedBottom();
                    if (list.Count > 0)
                    {
                        var rt = list[0].rect;

                        float y = rt.height + m_Spacing.y;
                        m_RemainingSize.y += y;
                        my_anchored_position.y += -y * (1 - content_pivot.y);

                        fix = true;
                    }
                }
            }

            if (this.horizontal)
            {
                var factor_x = (view_size.x + m_RemainingSize.x) * content_pivot.x;
                if (my_anchored_position.x < (-m_RemainingSize.x - anchor_rect_size.x) + factor_x)
                {
                    Debug.Log("到右");
                    var list = this.onScrollArrivedRight();
                    if (list.Count > 0)
                    {
                        var rt = list[0].rect;

                        float x = rt.width + m_Spacing.x;
                        m_RemainingSize.x += x;
                        my_anchored_position.x += x * content_pivot.x;

                        fix = true;
                    }
                }
                else if (my_anchored_position.x > -anchor_rect_size.x + factor_x)
                {
                    Debug.Log("到左");
                    var list = this.onScrollArrivedLeft();
                    if (list.Count > 0)
                    {
                        var rt = list[0].rect;

                        float x = rt.width + m_Spacing.x;
                        m_RemainingSize.x += x;
                        my_anchored_position.x += -x * (1 - content_pivot.x);

                        fix = true;
                    }
                }
                else
                {
                    foreach (RectTransform item in this.content)
                    {
                        var view_local_position = item.localPosition + this.content.localPosition;
                        var width = item.rect.width;
                        if (view_local_position.x < -view_pivot.x * view_size.x - width)
                        {
                            m_FreeTransform.Enqueue(item);
                            item.gameObject.SetActive(false);
                            item.SetParent(null);
                            float x = width + m_Spacing.x;
                            m_RemainingSize.x -= x;
                            my_anchored_position.x += x * (1 - content_pivot.x);
                            fix = true;
                        }
                        else if(view_local_position.x > (1 - view_pivot.x) * view_size.x + width)
                        {
                            m_FreeTransform.Enqueue(item);
                            item.gameObject.SetActive(false);
                            item.SetParent(null);
                            float x = width + m_Spacing.x;
                            m_RemainingSize.x -= x;
                            my_anchored_position.x += -x * content_pivot.x;
                            fix = true;
                        }
                    }
                }
            }

            if (fix)
            {
                this.content.anchoredPosition = my_anchored_position;
                ///为什么要调这个函数
                ///因为unity把一个关键的参数m_PointerStartLocalCursor写成了private
                this.OnBeginDrag(eventData);


            }
        }

        private Bounds getBounds(RectTransform transform, RectTransform relative_rect)
        {
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            Matrix4x4 worldToLocalMatrix = relative_rect.worldToLocalMatrix;
            Bounds result = TezScrollRect.internalGetBounds(corners, ref worldToLocalMatrix);
            return result;
        }

        internal static Bounds internalGetBounds(Vector3[] corners, ref Matrix4x4 world_to_local_matrix)
        {
            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < 4; i++)
            {
                Vector3 lhs = world_to_local_matrix.MultiplyPoint3x4(corners[i]);
                vector = Vector3.Min(lhs, vector);
                vector2 = Vector3.Max(lhs, vector2);
            }
            Bounds result = new Bounds(vector, Vector3.zero);
            result.Encapsulate(vector2);
            return result;
        }

        Vector2 getItemSize(RectTransform transform)
        {
            return transform.rect.size;
        }

        Bounds getItemBounds(RectTransform transform)
        {
            return new Bounds(transform.rect.center, transform.rect.size);
        }

        RectTransform getNewItem()
        {
            if (m_FreeTransform.Count > 0)
            {
                return m_FreeTransform.Dequeue();
            }

            return null;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_Entered = true;
            m_Camera = eventData.enterEventCamera;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_Entered = false;
            m_Camera = null;
        }

    }
}

