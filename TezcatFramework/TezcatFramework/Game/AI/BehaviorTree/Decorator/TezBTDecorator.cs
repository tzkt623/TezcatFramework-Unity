using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 修饰节点
    /// 用来做辅助功能的节点
    /// </summary>
    public abstract class TezBTDecorator : TezBTNode
    {
        public override Category category => Category.Decorator;

        protected TezBTNode mChild = null;

        public override void execute()
        {
            mChild.execute();
        }

        public virtual void setChild(TezBTNode node)
        {
            mChild = node;
        }

        public override void reset()
        {
            mChild.reset();
        }

        public override void close()
        {
            base.close();
            mChild = null;
        }

        public override void loadConfig(TezReader reader)
        {
            reader.beginObject("Node");
            mChild = TezBehaviorTree.create(reader.readString("CID"));
            mChild.loadConfig(reader);
            reader.endObject("Node");
        }
    }

    [TezBTRegister(name = "Inverter")]
    public class TezBTInverter : TezBTDecorator
    {
        public override void init()
        {

        }

        public override Result imdExecute()
        {
            switch (mChild.imdExecute())
            {
                case Result.Fail:
                    return Result.Success;
                case Result.Success:
                    return Result.Fail;
            }

            return Result.Running;
        }

        public override void execute()
        {
            mChild.execute();
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.reportToParent(Result.Running);
                    break;
                default:
                    this.reportToParent(1 - result);
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

        public override Result imdExecute()
        {
            if (mChild.imdExecute() != Result.Running)
            {
                return Result.Success;
            }

            return Result.Running;
        }

        public override void execute()
        {
            mChild.execute();
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.reportToParent(Result.Running);
                    break;
                default:
                    this.reportToParent(Result.Success);
                    break;
            }
        }
    }

    [TezBTRegister(name = "Failure")]
    public class TezBTFailure : TezBTDecorator
    {
        public override void init()
        {

        }

        public override Result imdExecute()
        {
            if (mChild.imdExecute() != Result.Running)
            {
                return Result.Fail;
            }

            return Result.Running;
        }

        public override void execute()
        {
            mChild.execute();
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.reportToParent(Result.Running);
                    break;
                default:
                    this.reportToParent(Result.Fail);
                    break;
            }
        }
    }
}