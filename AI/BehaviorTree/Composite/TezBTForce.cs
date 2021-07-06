using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// 强制顺序运行节点
    /// 不论子节点成功失败都会运行
    /// 直到所有节点运行完毕返回Success
    /// </summary>
    public class TezBTForce : TezBTComposite_List
    {
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Running:
                    break;
                default:
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        this.report(Result.Success);
                    }
                    break;
            }
        }

        public override Result newExecute()
        {
            switch (m_List[m_Index].newExecute())
            {
                case Result.Fail:
                case Result.Success:
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        return Result.Success;
                    }
                    break;
            }

            return Result.Running;
        }

        public override void removeSelfFromTree()
        {
            m_List[m_Index].removeSelfFromTree();
        }
    }
}
