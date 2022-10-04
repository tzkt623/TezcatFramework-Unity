using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 随机选择节点
    /// 随机运行直到某一个子节点返回Success 则返回Success
    /// 否则返回Fail
    /// </summary>
    public class TezBTRandomSelector : TezBTComposite_List
    {
        bool m_Random = false;

        /// <summary>
        /// 子节点向自己报告运行状态
        /// </summary>
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    ///如果有节点运行成功
                    ///像父级报告运行成功
                    this.reset();
                    this.report(Result.Success);
                    break;
                case Result.Fail:
                    ///如果有节点运行失败
                    ///测试下一个节点
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        this.report(Result.Fail);
                    }
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }
        }

        protected override void onExecute()
        {
            if (!m_Random)
            {
                m_Random = true;
                m_List.shuffle();
            }

            m_List[m_Index].execute();
            this.report(m_Result);
        }

        public override Result newExecute()
        {
            if (!m_Random)
            {
                m_Random = true;
                m_List.shuffle();
            }

            bool flag = true;
            while (flag)
            {
                switch (m_List[m_Index].newExecute())
                {
                    case Result.Success:
                        ///如果有节点运行成功
                        ///像父级报告运行成功
                        this.reset();
                        return Result.Success;
                    case Result.Fail:
                        ///如果有节点运行失败
                        ///测试下一个节点
                        m_Index++;
                        if (m_Index == m_List.Count)
                        {
                            this.reset();
                            return Result.Fail;
                        }
                        break;
                    case Result.Running:
                        flag = false;
                        break;
                }
            }

            return Result.Running;
        }

        public override void reset()
        {
            base.reset();
            m_Random = false;
        }

        public override void removeSelfFromTree()
        {
            m_List[m_Index].removeSelfFromTree();
        }
    }
}
