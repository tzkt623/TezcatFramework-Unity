namespace tezcat.Framework.Game
{
    /// <summary>
    /// 强制顺序运行节点
    /// 不论子节点成功失败都会运行
    /// 直到所有节点运行完毕返回Success
    /// </summary>
    [TezBTRegister(name = "Force")]
    public class TezBTForce : TezBTCompositeList
    {
        public override Result imdExecute()
        {
            if (mList[mIndex].imdExecute() != Result.Running)
            {
                mList[mIndex].reset();
                mIndex++;
                if (mIndex == mList.Count)
                {
                    // this.reset();
                    return Result.Success;
                }
            }

            return Result.Running;
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.reportToParent(Result.Running);
                    break;
                default:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        this.reportToParent(Result.Success);
                    }
                    break;
            }
        }

        public override void execute()
        {
            mList[mIndex].execute();
        }
    }
}
