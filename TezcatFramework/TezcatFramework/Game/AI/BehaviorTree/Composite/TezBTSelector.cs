namespace tezcat.Framework.Game
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
        bool mFlag = true;

        protected override void onChildReport(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    mFlag = false;
                    this.reset();
                    this.setSuccess();
                    break;
                case Result.Fail:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        mFlag = false;
                        this.reset();
                        this.setFail();
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            mFlag = true;
            while (mFlag)
            {
                mList[mIndex].execute();
            }
        }
    }
}
