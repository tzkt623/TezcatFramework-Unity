using System.Collections.Generic;

namespace tezcat.Framework.AI
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
        List<TezBTNode> mRunningNodes = new List<TezBTNode>();
        int mSuccessCount = 0;


        public override void init()
        {
            base.init();
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                ///如果成功,就继续执行下一个(同一帧内),并且删除当前
                case Result.Success:
                    mRunningNodes.RemoveAt(mIndex--);
                    break;
                ///如果有一个执行失败,就立即返回
                case Result.Fail:
                    this.reset();
                    this.reportToParent(Result.Fail);
                    break;
                case Result.Running:
                    mIndex--;
                    this.reportToParent(Result.Running);
                    break;
                default:
                    break;
            }

            ///所有节点全部执行成功
            ///则返回成功
            if (mRunningNodes.Count == 0)
            {
                this.reset();
                this.reportToParent(Result.Success);
            }
        }

        public override void execute()
        {
            if (mRunningNodes.Count == 0)
            {
                mRunningNodes.AddRange(mList);
                mRunningNodes.Reverse();
            }

            mIndex = mRunningNodes.Count - 1;
            while ((mRunningNodes.Count > 0) && (mIndex >= 0))
            {
                mRunningNodes[mIndex].execute();
            }
        }

        public override Result imdExecute()
        {
            mIndex = 0;
            mSuccessCount = 0;
            while (mIndex < mList.Count)
            {
                switch (mList[mIndex].imdExecute())
                {
                    ///如果有一个执行失败,就立即返回
                    case Result.Fail:
//                        this.reset();
                        mList[mIndex].reset();
                        return Result.Fail;
                    ///不管是执行成功还是运行中
                    ///都要继续执行下一个
                    case Result.Success:
                        mList[mIndex].reset();
                        mSuccessCount++;
                        break;
                }

                mIndex++;
            }

            ///所有节点全部执行成功
            ///则返回成功
            if (mSuccessCount == mList.Count)
            {
//                this.reset();
                return Result.Success;
            }

            return Result.Running;
        }

        public override void reset()
        {
            base.reset();
            mSuccessCount = 0;
            mRunningNodes.Clear();
        }
    }
}
