namespace tezcat.Framework.AI
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
    public class TezBTSequence : TezBTComposite_List
    {
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                ///如果成功,就运行下一个节点(不同帧)
                ///直到所有节点运行完毕,才返回成功
                case Result.Success:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        this.reportToParent(Result.Success);
                    }
                    break;
                ///如果失败,就立即返回
                case Result.Fail:
                    this.reset();
                    this.reportToParent(Result.Fail);
                    break;
                ///啥也不做
                case Result.Running:
                    this.reportToParent(Result.Running);
                    break;
                default:
                    break;
            }
        }

        public override void execute()
        {
            mList[mIndex].execute();
        }

        public override Result imdExecute()
        {
            switch (mList[mIndex].imdExecute())
            {
                case Result.Success:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.reset();
                        return Result.Success;
                    }
                    break;
                case Result.Fail:
                    this.reset();
                    return Result.Fail;
            }

            return Result.Running;
        }
    }
}