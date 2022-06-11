using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 并行节点
    /// 依次运行所有节点
    /// 直到没有节点返回Running
    /// 返回Success
    /// 
    /// 比如一个角色正在与一个敌人相互射击
    /// 这时你想要判断
    /// 是否需要翻滚
    /// 是否需要开枪
    /// 是否需要扔雷
    /// 是否需要加血
    /// 
    /// 这难道不是选择节点的功能吗
    /// 你难道还能同时做所有动作
    /// 
    /// 如果你真的需要这样的功能,请用Force节点,管你成不成功全部运行一遍,总有一款适合你
    /// 
    /// 所以真正并行在游戏行为中用的机会很少
    /// 你需要的是下面那种监察并行节点(TezBTObserveParallel)
    /// </summary>
    public class TezBTParallel : TezBTComposite_List
    {
        int m_SuccessCount = 0;
        int m_FailCount = 0;
        int m_RunningCount = 0;

        public override void init()
        {
            base.init();
            m_RunningCount = this.childrenCount;
        }

        public override void onReport(TezBTNode node, Result result)
        {
            m_Index++;
            switch (result)
            {
                case Result.Success:
                    m_SuccessCount++;
                    m_RunningCount--;
                    break;
                case Result.Fail:
                    m_FailCount++;
                    m_RunningCount--;
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }

            if (m_RunningCount == 0)
            {
                this.reset();
                if (m_SuccessCount == this.childrenCount)
                {
                    this.report(Result.Success);
                }
                else
                {
                    this.report(Result.Fail);
                }
            }
        }

        protected override void onExecute()
        {
            while(m_Index < this.childrenCount)
            {
                m_List[m_Index].execute();
            }

            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].execute();
            }
        }

        public override Result newExecute()
        {
            while (m_Index < this.childrenCount)
            {
                switch (m_List[m_Index++].newExecute())
                {
                    case Result.Success:
                        m_SuccessCount++;
                        m_RunningCount--;
                        break;
                    case Result.Fail:
                        m_FailCount++;
                        m_RunningCount--;
                        break;
                }
            }

            if (m_RunningCount == 0)
            {
                this.reset();
                return Result.Success;
            }

            return Result.Running;
        }

        public override void reset()
        {
            base.reset();
            m_RunningCount = this.childrenCount;
        }
    }
}
