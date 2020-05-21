using System;

namespace tezcat.Framework.AI
{
    public abstract class TezBTActionNode : TezBTNode
    {
        bool m_Enter = false;
        bool m_Init = false;

        public sealed override Result execute(ITezBTContext context)
        {
            if (!m_Enter)
            {
                m_Enter = true;
                this.enter();
            }

            var result = update(context);

            switch (result)
            {
                case Result.Running:
                    break;
                default:
                    this.exit();
                    break;
            }

            return result;
        }

        protected abstract Result update(ITezBTContext context);

        protected override void enter()
        {

        }

        protected override void exit()
        {
            m_Enter = false;
        }

        public override void close(bool self_close = true)
        {

        }
    }

}
