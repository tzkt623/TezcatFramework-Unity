using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    /// <summary>
    /// ˳��ڵ�
    /// ˳���������нڵ� ���нڵ�Success �򷵻�Success
    /// ���򷵻�Fail
    /// </summary>
    public class TezBTSequence : TezBTComposite_List
    {
        /// <summary>
        /// �ӽڵ����Լ���������״̬
        /// </summary>
        public override void onReport(TezBTNode node, Result result)
        {
            switch (result)
            {
                case Result.Success:
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        this.report(Result.Success);
                    }
                    break;
                case Result.Fail:
                    this.reset();
                    this.report(Result.Fail);
                    break;
                case Result.Running:
                    break;
                default:
                    break;
            }
        }

        public override void removeSelfFromTree()
        {
            m_List[m_Index].removeSelfFromTree();
        }

        protected override void onExecute()
        {
            m_List[m_Index].execute();
        }

        public override Result newExecute()
        {
            switch (m_List[m_Index].newExecute())
            {
                case Result.Success:
                    m_Index++;
                    if (m_Index == m_List.Count)
                    {
                        this.reset();
                        return Result.Success;
                    }
                    break;
                case Result.Fail:
                    this.reset();
                    return Result.Fail;
            }

            return Result.Running;
        }
    } 
}
