namespace tezcat.Framework.AI
{
    /// <summary>
    /// 选择节点
    /// 
    /// <para>
    /// 运行直到某一个子节点返回Success
    /// 则返回Success
    /// 并且不再执行后续的节点
    /// 否则返回Fail
    /// </para>
    /// 
    /// <para>
    /// 也就是在一帧之内从子节点当中选一个可执行的出来
    /// </para>
    /// 
    /// 比如
    /// 要么攻击,要么防御,要么逃跑,要么投降
    /// 
    /// </summary>
    [TezBTRegister(name = "Selector")]
    public class TezBTSelector : TezBTCompositeList
    {
        TezBTNode mRunning = null;

        public override Result imdExecute()
        {
            if (mRunning != null)
            {
                switch (mRunning.imdExecute())
                {
                    case Result.Success:
                        ///如果有节点运行成功,立即中断并返回
                        //                        this.reset();
                        mRunning.reset();
                        return Result.Success;
                    case Result.Fail:
                        ///如果有节点运行失败
                        ///测试下一个节点
                        ///如果测试完了都没有成功,就返回失败
                        mRunning.reset();
                        mRunning = null;
                        mIndex++;
                        if (mIndex == mList.Count)
                        {
                            //                            this.reset();
                            return Result.Fail;
                        }
                        break;
                }

                return Result.Running;
            }
            else
            {
                while (mRunning == null)
                {
                    switch (mList[mIndex].imdExecute())
                    {
                        case Result.Success:
                            ///如果有节点运行成功,立即中断并返回
                            //                            this.reset();
                            mList[mIndex].reset();
                            return Result.Success;
                        case Result.Fail:
                            ///如果有节点运行失败
                            ///测试下一个节点
                            ///如果测试完了都没有成功,就返回失败
                            mList[mIndex].reset();
                            mIndex++;
                            if (mIndex == mList.Count)
                            {
                                return Result.Fail;
                            }
                            break;
                        case Result.Running:
                            ///如果是running,就啥也不管
                            mRunning = mList[mIndex];
                            break;
                    }
                }

                return Result.Running;
            }
        }

        /// <summary>
        /// 子节点向自己报告运行状态
        /// </summary>
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    ///如果有节点运行成功,立即中断并返回
                    this.reset();
                    this.reportToParent(Result.Success);
                    break;
                case Result.Fail:
                    ///如果有节点运行失败
                    ///测试下一个节点
                    ///如果测试完了都没有成功,就返回失败
                    mRunning = null;
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        this.reportToParent(Result.Fail);
                    }
                    break;
                case Result.Running:
                    ///如果是running,就啥也不管
                    mRunning = node;
                    this.reportToParent(Result.Running);
                    break;
                default:
                    break;
            }
        }

        public override void execute()
        {
            if (mRunning != null)
            {
                mRunning.execute();
            }
            else
            {
                while (mRunning == null)
                {
                    mList[mIndex].execute();
                }
            }
        }

        public override void reset()
        {
            base.reset();
            mRunning = null;
        }
    }
}
