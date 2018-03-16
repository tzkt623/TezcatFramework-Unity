using UnityEngine;

namespace tezcat
{
    public abstract class TezScrollRectListener
    {
        protected TezScrollRect m_ScrollRect = null;
        public void setScrollRect(TezScrollRect sr)
        {
            m_ScrollRect = sr;
        }

        public abstract RectTransform[] onArrivedTop(int need_count, int top_row_id);
        public abstract RectTransform[] onArrivedBottom(int need_count, int bottom_row_id);
        public abstract RectTransform[] onArrivedLeft(int need_count, int left_col_id);
        public abstract RectTransform[] onArrivedRight(int need_count, int right_col_id);


        public abstract bool onRemoveLeft(RectTransform removed_item);
        public abstract bool onRemoveRight(RectTransform removed_item);
        public abstract bool onRemoveTop(RectTransform removed_item);
        public abstract bool onRemoveBottom(RectTransform removed_item);
    }
}