namespace tezcat.Framework.Game
{
    /// <summary>
    /// 顺序节点
    /// 
    /// <para>
    /// 依次执行每个节点
    /// 执行完一个才会进入下一个
    /// 成功运行完所有节点返回Success
    /// 否则返回Fail
    /// </para>
    /// 
    /// 比如
    /// 先走到门边-开门-进入屋子-坐下-拿起遥控器-打开电视机-看电视
    /// 
    /// </summary>
    [TezBTRegister(name = "Sequence")]
    public class TezBTSequence : TezBTCompositeList
    {
        protected override void onChildReport(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        this.setSuccess();
                    }
                    break;
                case Result.Fail:
                    this.reset();
                    this.setFail();
                    break;
            }
        }

        protected override void onExecute()
        {
            mList[mIndex].execute();
        }
    }
}