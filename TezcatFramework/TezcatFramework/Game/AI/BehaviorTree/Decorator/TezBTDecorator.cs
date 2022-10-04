using tezcat.Framework.Database;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 修饰节点
    /// 用来做辅助功能的节点
    /// </summary>
    public abstract class TezBTDecorator : TezBTNode
    {
        public override Category category => Category.Decorator;

        protected TezBTNode m_Child = null;

        public override void execute()
        {
            m_Child.execute();
        }

        public virtual void setChild(TezBTNode node)
        {
            m_Child = node;
        }

        public override void removeSelfFromTree()
        {
            m_Child.removeSelfFromTree();
        }

        public override void reset()
        {
            m_Child.reset();
        }

        public override void close()
        {
            base.close();
            m_Child = null;
        }

        public override void loadConfig(TezReader reader)
        {
            reader.beginObject("Node");
            m_Child = TezBehaviorTree.create(reader.readString("CID"));
            m_Child.loadConfig(reader);
            reader.endObject("Node");
        }
    }

    public class TezBTInverter : TezBTDecorator
    {
        public override void init()
        {

        }

        public override Result newExecute()
        {
            switch (m_Child.newExecute())
            {
                case Result.Success:
                    return Result.Fail;
                case Result.Fail:
                    return Result.Success;
            }

            return Result.Running;
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.report(Result.Running);
                    break;
                default:
                    this.report(1 - result);
                    break;
            }
        }
    }

    public class TezBTSucceeder : TezBTDecorator
    {
        public override void init()
        {

        }

        public override Result newExecute()
        {
            switch (m_Child.newExecute())
            {
                case Result.Success:
                case Result.Fail:
                    return Result.Success;
            }

            return Result.Running;
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.report(Result.Running);
                    break;
                default:
                    this.report(Result.Success);
                    break;
            }
        }
    }

    public class TezBTFailure : TezBTDecorator
    {
        public override void init()
        {

        }

        public override Result newExecute()
        {
            switch (m_Child.newExecute())
            {
                case Result.Success:
                case Result.Fail:
                    return Result.Fail;

            }

            return Result.Running;
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    this.report(Result.Running);
                    break;
                default:
                    this.report(Result.Fail);
                    break;
            }
        }
    }
}