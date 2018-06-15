using tezcat.Core;
using tezcat.Signal;
using UnityEngine;

namespace tezcat.UI
{
    public class TezPageController : ITezCloseable
    {
        TezEventBus.Action<int, int> m_OnPageChanged;

        public int currentPage { get; private set; } = 1;
        public int maxPage { get; private set; } = 0;
        public int pageCapacity { get; set; } = 10;

        int m_CurrentPageBegin;
        int m_CurrentPageEnd;

        public void setListener(TezEventBus.Action<int, int> function)
        {
            m_OnPageChanged = function;
        }

        public void calculateMaxPage(int total_count)
        {
            this.maxPage = Mathf.CeilToInt(total_count / (float)pageCapacity);
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
            m_CurrentPageBegin = (currentPage - 1) * pageCapacity;
            m_CurrentPageEnd = m_CurrentPageBegin + pageCapacity;
            return m_CurrentPageBegin >= 0 && m_CurrentPageEnd > 0;
        }

        public void close()
        {
            m_OnPageChanged = null;
        }
    }
}