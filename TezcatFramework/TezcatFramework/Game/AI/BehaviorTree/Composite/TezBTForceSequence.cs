namespace tezcat.Framework.Game
{
    /// <summary>
    /// 强制顺序运行节点
    /// 不论子节点成功失败都会运行
    /// 直到所有节点运行完毕返回Success
    /// </summary>
    [TezBTRegister(name = "ForceSequence")]
    public class TezBTForceSequence : TezBTSequence
    {
        protected override void onChildReport(Result result)
        {
            switch (result)
            {
                case Result.Success:
                case Result.Fail:
                    mIndex++;
                    if (mIndex == mList.Count)
                    {
                        this.setSuccess();
                    }
                    break;
            }
        }
    }
}
