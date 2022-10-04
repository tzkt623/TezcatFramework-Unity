using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 加成代理
    /// 用于需要被加成的对象
    /// 处理各种加成事件
    /// </summary>
    public class TezBonusAgent
        : ITezBonusAgent
        , ITezCloseable
    {
        TezBonusPath m_BonusPath = null;
        public TezBonusPath bonusPath => m_BonusPath;

        TezEventExtension.Action<ITezBonusObject> m_OnAddBonusObject = null;
        TezEventExtension.Action<ITezBonusObject> m_OnRemoveBonusObject = null;

        public void setListener(TezEventExtension.Action<ITezBonusObject> onAddBonusObject, TezEventExtension.Action<ITezBonusObject> onRemoveBonusObject)
        {
            m_OnAddBonusObject = onAddBonusObject;
            m_OnRemoveBonusObject = onRemoveBonusObject;
        }

        public void setPath(TezBonusPath path)
        {
            m_BonusPath = path;
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="obj"></param>
        public void addBonusObject(ITezBonusObject obj)
        {
            m_OnAddBonusObject(obj);
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="obj"></param>
        public void removeBonusObject(ITezBonusObject obj)
        {
            m_OnRemoveBonusObject(obj);
        }

        public void close()
        {
            m_BonusPath = null;
            m_OnAddBonusObject = null;
            m_OnRemoveBonusObject = null;
        }
    }
}