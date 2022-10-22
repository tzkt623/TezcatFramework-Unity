using tezcat.Framework.Extension;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 随机选择节点
    /// 随机运行直到某一个子节点返回Success 则返回Success
    /// 否则返回Fail
    /// </summary>
    [TezBTRegister(name = "RandomSelector")]
    public class TezBTRandomSelector : TezBTCompositeList
    {
        bool mRandom = false;

        /// <summary>
        /// 子节点向自己报告运行状态
        /// </summary>
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    ///如果有节点运行成功
                    ///像父级报告运行成功
                    this.reset();
                    this.reportToParent(Result.Success);
                    break;
                case Result.Fail:
                    ///如果有节点运行失败
                    ///测试下一个节点
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        this.reportToParent(Result.Fail);
                    }
                    break;
                case Result.Running:
                    ///啥也不报告
                    this.reportToParent(Result.Running);
                    break;
                default:
                    break;
            }
        }

        public override void execute()
        {
            if (!mRandom)
            {
                mRandom = true;
                mList.shuffle();
            }

            mList[mIndex].execute();
        }

        public override Result imdExecute()
        {
            if (!mRandom)
            {
                mRandom = true;
                mList.shuffle();
            }

            switch (mList[mIndex].imdExecute())
            {
                case Result.Success:
                    ///如果有节点运行成功
                    ///像父级报告运行成功
//                    this.reset();
                    mList[mIndex].reset();
                    return Result.Success;
                case Result.Fail:
                    ///如果有节点运行失败
                    ///测试下一个节点
                    mList[mIndex].reset();
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
//                        this.reset();
                        return Result.Fail;
                    }
                    break;
            }

            return Result.Running;
        }


        public override void reset()
        {
            base.reset();
            mRandom = false;
        }
    }
}
