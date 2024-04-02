namespace tezcat.Framework.Game
{
#if false
    public class TezBTObserveParallel : TezBTComposite_List
    {
        int m_SuccessCount = 0;
        int mLastNode = 0;
        bool m_Running = true;


        public override void init()
        {
            base.init();
            mLastNode = mList.Count - 1;
        }

        public override void addNode(TezBTNode node)
        {
            ///
            if (node.GetType() == this.GetType())
            {
                throw new System.Exception();
            }

            base.addNode(node);
        }

        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    m_SuccessCount++;
                    if (m_SuccessCount == this.childrenCount)
                    {
                        this.reset();
                        this.reportToParent(Result.Success);
                    }
                    break;
                case Result.Fail:
                    m_Running = false;
                    this.reset();
                    this.reportToParent(Result.Fail);
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            while (mIndex < this.childrenCount)
            {
                mList[mIndex].execute();
                if (m_Running)
                {
                    break;
                }
            }
        }

        public override Result newExecute()
        {
            while (mIndex < this.childrenCount)
            {
                switch (mList[mIndex++].newExecute())
                {
                    case Result.Success:
                        if (mIndex == this.childrenCount)
                        {
                            this.reset();
                            return Result.Success;
                        }
                        break;
                    case Result.Fail:
                        this.reset();
                        return Result.Fail;
                }
            }


            this.reset();
            return Result.Running;
        }

        public override void reset()
        {
            base.reset();
            m_SuccessCount = 0;
            m_Running = true;
        }
    }
#endif
}
