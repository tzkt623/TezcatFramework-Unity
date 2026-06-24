using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 并行节点
    /// <para>
    /// 同时运行所有节点
    /// 只要其中一个节点Fail就立即中断所有,并且返回Fail
    /// 只有全部成功执行才返回Success
    /// 只要有节点在运行并且没有Fail就返回Running
    /// </para>
    /// 
    /// <para>
    /// 比如
    /// 移动+瞄准+射击
    /// </para>
    /// 
    /// <para>
    /// 这里有个逻辑优先级需要注意
    /// 你必须要先瞄准再射击
    /// 所以节点顺序很重要
    /// </para>
    /// 
    /// <para>
    /// 当然如果你的AI逻辑是相互独立的
    /// 那怎么排序都没有问题
    /// </para>
    /// </summary>
    [TezBTRegister(name = "Parallel")]
    public class TezBTParallel : TezBTCompositeList
    {
        List<TezBTNode> mRunningList = null;
        bool mRunning = true;

        public override void init()
        {
            base.init();
            mRunningList = new List<TezBTNode>(mList);
        }

        protected override void onChildReport(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    mRunningList.RemoveAt(mIndex);
                    mIndex--;
                    if (mRunningList.Count == 0)
                    {
                        mRunning = false;
                        this.setSuccess();
                    }
                    break;
                case Result.Fail:
                    mRunning = false;
                    this.setFail();
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            mRunning = true;
            mIndex = 0;
            while (mRunning && (mIndex < mRunningList.Count))
            {
                mRunningList[mIndex].execute();
                mIndex++;
            }
        }

        public override void reset()
        {
            base.reset();
            mRunningList.Clear();
            mRunningList.AddRange(mList);
        }

        protected override void onClose()
        {
            base.onClose();
            mRunningList.Clear();
            mRunningList = null;
        }
    }
}