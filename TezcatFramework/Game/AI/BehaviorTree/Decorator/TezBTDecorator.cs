using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 修饰节点
    /// 用来做辅助功能的节点
    /// </summary>
    public abstract class TezBTDecorator 
        : TezBTNode
        , ITezBTParentNode
    {
        public override Category category => Category.Decorator;
        protected TezBTNode mChild = null;

        protected override void onExecute()
        {
            mChild.execute();
        }

        public override void reset()
        {
            mChild.reset();
        }

        protected override void onClose()
        {
            base.onClose();
            mChild = null;
        }

        public override void loadConfig(TezSaveController.Reader reader)
        {
            reader.enterObject(TokenNode);
            mChild = TezBehaviorTree.create(reader.readString(TokenCID));
            mChild.loadConfig(reader);
            reader.exitObject(TokenNode);
        }

        public virtual void addChild(TezBTNode node)
        {
            mChild = node;
        }

        public abstract void childReport(Result result);
    }

    [TezBTRegister(name = "Inverter")]
    public class TezBTInverter : TezBTDecorator
    {
        public override void init()
        {

        }

        public override void childReport(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    this.setFail();
                    break;
                case Result.Fail:
                    this.setSuccess();
                    break;
                default:
                    break;
            }
        }
    }

    [TezBTRegister(name = "Succeeder")]
    public class TezBTSucceeder : TezBTDecorator
    {
        public override void init()
        {
        }

        public override void childReport(Result result)
        {
            if(result == Result.Running)
            {
                return;
            }

            this.setSuccess();
        }
    }

    [TezBTRegister(name = "Failure")]
    public class TezBTFailure : TezBTDecorator
    {
        public override void init()
        {

        }

        public override void childReport(Result result)
        {
            if (result == Result.Running)
            {
                return;
            }

            this.setFail();
        }
    }
}