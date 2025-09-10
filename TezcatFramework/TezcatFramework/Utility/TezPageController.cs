using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 分页器
    /// </summary>
    public class TezPageController : ITezCloseable
    {
        TezEventExtension.Action<int, int> mOnPageChanged;
        TezEventExtension.Action mOnPageEmpty;

        /// <summary>
        /// 当前页码
        /// 不会低于1
        /// </summary>
        public int currentPage { get; private set; } = 1;

        /// <summary>
        /// 最大页码
        /// </summary>
        public int maxPage { get; private set; } = 0;

        /// <summary>
        /// 每一页的容量
        /// </summary>
        public int pageCapacity { get; set; } = 10;

        /// <summary>
        /// 每页开始的位置
        /// </summary>
        int mCurrentPageBegin;

        int mCount;
        /// <summary>
        /// 总数量
        /// 设置总数量以计算总页面
        /// </summary>
        public int count
        {
            get { return mCount; }
            set
            {
                mCount = value;
                this.maxPage = (int)Math.Ceiling(mCount / (float)this.pageCapacity);
            }
        }

        /// <summary>
        /// 当页面变化时
        /// on_page_changed(页面开始位置,页面容量)
        /// </summary>
        /// <param name="on_page_changed"></param>
        /// <param name="on_page_empty"></param>
        public void setListener(TezEventExtension.Action<int, int> on_page_changed, TezEventExtension.Action on_page_empty)
        {
            mOnPageChanged = on_page_changed;
            mOnPageEmpty = on_page_empty;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        public void pageUp()
        {
            if (mCount == 0)
            {
                mOnPageEmpty();
                return;
            }

            ///不会低于第一页
            if (this.currentPage == 1)
            {
                return;
            }

            this.currentPage -= 1;
            this.refresh();
        }

        /// <summary>
        /// 下一页
        /// </summary>
        public void pageDown()
        {
            if (mCount == 0)
            {
                mOnPageEmpty();
                return;
            }

            ///不会超过最大页数
            if (this.currentPage == maxPage)
            {
                return;
            }

            this.currentPage += 1;
            this.refresh();
        }

        /// <summary>
        /// 设定页码
        /// </summary>
        public void setPage(int page)
        {
            ///如果没有数量用于分页
            if (mCount == 0)
            {
                this.currentPage = 1;
                mOnPageEmpty();
                return;
            }

            if (page < 1)
            {
                this.currentPage = 1;
            }
            else if (page > maxPage)
            {
                currentPage = maxPage;
            }
            else
            {
                this.currentPage = page;
            }

            this.refresh();
        }

        private void refresh()
        {
            mCurrentPageBegin = (this.currentPage - 1) * pageCapacity;
            mOnPageChanged(mCurrentPageBegin, this.pageCapacity);
        }

        public void close()
        {
            mOnPageChanged = null;
            mOnPageEmpty = null;
        }
    }
}