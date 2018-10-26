using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.UI
{
    public class TezPageController : ITezCloseable
    {
        TezEventExtension.Action<int, int> m_OnPageChanged;

        public int currentPage { get; private set; } = 1;
        public int maxPage { get; private set; } = 0;
        public int pageCapacity { get; set; } = 10;

        int m_CurrentPageBegin;
        int m_CurrentPageEnd;
        int m_TotalCount;

        /// <summary>
        /// 页面变化时的通知
        /// begin 当前页面Item开始位置
        /// end 当前页面Item结束位置的后一位
        /// </summary>
        /// <param name="function"></param>
        public void setListener(TezEventExtension.Action<int, int> function)
        {
            m_OnPageChanged = function;
        }

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
        }

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
        }

        public void setPage(int page)
        {
            if(this.maxPage == 0)
            {
                this.currentPage = 0;
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