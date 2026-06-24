using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 随机选择节点
    /// 随机运行直到某一个子节点返回Success 则返回Success
    /// 否则返回Fail
    /// </summary>
    [TezBTRegister(name = "RandomSelector")]
    public class TezBTRandomSelector : TezBTSelector
    {
        bool mRandom = false;

        protected override void onExecute()
        {
            if (!mRandom)
            {
                mRandom = true;
                mList.shuffle();
            }

            base.onExecute();
        }

        public override void reset()
        {
            base.reset();
            mRandom = false;
        }
    }
}
