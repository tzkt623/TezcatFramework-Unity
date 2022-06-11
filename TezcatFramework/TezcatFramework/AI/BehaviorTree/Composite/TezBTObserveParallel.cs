namespace tezcat.Framework.AI
{
    /// <summary>
    /// 
    /// 监察并行节点
    /// 此节点下不可能存在监察并行节点(功能重复)和顺序节点(无意义功能)
    /// 
    /// <para>
    /// 依次运行所有节点
    /// 直到最后一个节点之前的某个节点返回fail
    /// 整个节点返回fail
    /// 否则返回最后一个节点的状态
    /// </para>
    /// 
    /// <para>
    /// 最后一个节点之前的节点
    /// 作为一种条件节点存在
    /// 用于检测最后一个节点是否应该被执行
    /// </para>
    /// 
    /// <para>
    /// 与sequence的区别是
    /// sequence是每帧执行一个node直到返回success后再执行下一个node
    /// 此节点是在一帧之内完成所有node的检测
    /// 然后决定是否执行最后一个node的操作
    /// </para>
    /// 
    /// <para>
    /// 比如一个角色正在执行拿枪冲锋的命令
    /// 这时你需要检测自身HP,自身弹药,战场环境等等前置条件
    /// 如果满足,才会继续冲锋
    /// 否则立刻返回fail执行其他动作
    /// </para>
    /// 
    /// </summary>
    public class TezBTObserveParallel : TezBTComposite_List
    {
        int m_SuccessCount = 0;
        int m_LastNode = 0;
        bool m_Running = true;


        public override void init()
        {
            base.init();
            m_LastNode = m_List.Count - 1;
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
                        this.report(Result.Success);
                    }
                    break;
                case Result.Fail:
                    m_Running = false;
                    this.reset();
                    this.report(Result.Fail);
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            while (m_Index < this.childrenCount)
            {
                m_List[m_Index].execute();
                if (m_Running)
                {
                    break;
                }
            }
        }

        public override Result newExecute()
        {
            while (m_Index < this.childrenCount)
            {
                switch (m_List[m_Index++].newExecute())
                {
                    case Result.Success:
                        if (m_Index == this.childrenCount)
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

        public override void removeSelfFromTree()
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].removeSelfFromTree();
            }
        }

        public override void reset()
        {
            base.reset();
            m_SuccessCount = 0;
            m_Running = true;
        }
    }
}
