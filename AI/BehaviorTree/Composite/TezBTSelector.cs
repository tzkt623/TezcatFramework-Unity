using System;
using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 选择节点
    /// 按照行为重要性优先级排序
    /// 
    /// 运行直到某一个子节点返回Success 则返回Success
    /// 否则返回Fail
    /// 
    /// 选择节点特殊性
    /// 需要将所有子节点加入tree进行选择
    /// 在选择成功后中断
    /// 并且删除所有加入的节点
    /// 
    /// 
    /// 进入并行
    /// 检测条件1-1
    /// 如果成功,加入tree
    /// 进入选择节点
    /// 加入条件2-1
    /// 进入选择节点
    /// 加入条件3-1
    /// 进入顺序节点
    /// 加入节点1
    /// 加入条件3-3
    /// 加入条件2-3
    /// 
    /// </summary>
    public class TezBTSelector : TezBTComposite_List
    {
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

        public override Result newExecute()
        {
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

        public override void removeSelfFromTree()
        {
            m_List[m_Index].removeSelfFromTree();
        }
    }
}
