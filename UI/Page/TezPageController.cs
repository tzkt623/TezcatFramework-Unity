using UnityEngine;

namespace tezcat.UI
{
    public class TezPageController
    {
        TezEventBus.Action<int, int> m_OnPageChanged;

        public int currentPage { get; private set; } = 1;
        public int maxPage { get; private set; } = 0;
        public int countPerPage { get; set; } = 10;

        int m_CurrentPageBegin;
        int m_CurrentPageEnd;

        public void calculateMaxPage(int total_count)
        {
            this.maxPage = Mathf.CeilToInt(total_count / (float)countPerPage);
        }

        public void setListener(TezEventBus.Action<int, int> function)
        {
            m_OnPageChanged = function;
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
                return;
            }

            if(page <= 1)
            {
                this.currentPage = 1;
            }
            else if(page >= maxPage)
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
            this.m_CurrentPageBegin = (this.currentPage - 1) * countPerPage;
            this.m_CurrentPageEnd = this.m_CurrentPageBegin + countPerPage;
            return this.m_CurrentPageBegin >= 0 && this.m_CurrentPageEnd > 0;
        }
    }
}