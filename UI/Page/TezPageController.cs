using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.UI
{
    public class TezPageController : ITezCloseable
    {
        TezEventExtension.Action<int, int> m_OnPageChanged;
        TezEventExtension.Action m_OnPageEmpty;

        public int currentPage { get; private set; } = 1;
        public int maxPage { get; private set; } = 0;

        /// <summary>
        /// 每一页的容量
        /// </summary>
        public int pageCapacity { get; set; } = 10;

        int m_CurrentPageBegin;
        int m_CurrentPageEnd;
        int m_TotalCount;

        /// <summary>
        /// 页面变化时的通知
        /// <para>begin 当前页面Item开始位置</para> 
        /// <para>end 当前页面Item结束位置的后一位</para>
        /// </summary>
        public void setListener(TezEventExtension.Action<int, int> on_page_changed, TezEventExtension.Action on_page_empty)
        {
            m_OnPageChanged = on_page_changed;
            m_OnPageEmpty = on_page_empty;
        }

        /// <summary>
        /// 计算最大页数
        /// </summary>
        public void calculateMaxPage(int total_count)
        {
            m_TotalCount = total_count;
            this.maxPage = Mathf.CeilToInt(m_TotalCount / (float)pageCapacity);
        }

        public int isInPage(int index)
        {
            if(m_CurrentPageEnd > index)
            {
                return index - m_CurrentPageBegin;
            }

            return -1;
        }

        /// <summary>
        /// 不会低于第一页
        /// </summary>
        public void pageUp()
        {
            if (currentPage <= 1)
            {
                return;
            }

            this.currentPage -= 1;

            if (this.calculateCurrentPage())
            {
                m_OnPageChanged(this.m_CurrentPageBegin, this.m_CurrentPageEnd);
            }
            else
            {
                m_OnPageEmpty();
            }
        }

        /// <summary>
        /// 不会超过最大页数
        /// </summary>
        public void pageDown()
        {
            if (currentPage >= maxPage)
            {
                return;
            }

            this.currentPage += 1;

            if (this.calculateCurrentPage())
            {
                m_OnPageChanged(this.m_CurrentPageBegin, this.m_CurrentPageEnd);
            }
            else
            {
                m_OnPageEmpty();
            }
        }

        public void setPage(int page)
        {
            if(this.maxPage == 0)
            {
                this.currentPage = 0;
                m_OnPageEmpty();
                return;
            }

            if(page < 1)
            {
                this.currentPage = 1;
            }
            else if(page > maxPage)
            {
                currentPage = maxPage;
            }
            else
            {
                this.currentPage = page;
            }

            if(this.calculateCurrentPage())
            {
                m_OnPageChanged(this.m_CurrentPageBegin, this.m_CurrentPageEnd);
            }
            else
            {
                m_OnPageEmpty();
            }
        }

        private bool calculateCurrentPage()
        {
            m_CurrentPageBegin = (currentPage - 1) * pageCapacity;
            m_CurrentPageEnd = Mathf.Min(m_CurrentPageBegin + pageCapacity, m_TotalCount);
            return m_CurrentPageBegin >= 0 && m_CurrentPageEnd > 0;
        }

        public void close()
        {
            m_OnPageChanged = null;
        }
    }
}