using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.BonusSystem
{
    public interface ITezBonusAgentEntry
    {
        TezBonusAgent bonusAgent { get; }
        void onRemoveBonusObject(ITezBonusCarrier bonusObject);
        void onAddBonusObject(ITezBonusCarrier bonusObject);
    }

    /// <summary>
    /// 加成代理
    /// 用于需要被加成的对象
    /// 处理各种加成事件
    /// </summary>
    public class TezBonusAgent
        : ITezBonusAgent
        , ITezCloseable
    {
        TezBonusPath mBonusPath = null;
        public TezBonusPath bonusPath => mBonusPath;

        TezEventExtension.Action<ITezBonusCarrier> mOnAddBonusObject = null;
        TezEventExtension.Action<ITezBonusCarrier> mOnRemoveBonusObject = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onAddBonusObject"></param>
        /// <param name="onRemoveBonusObject"></param>
        public void setListener(TezEventExtension.Action<ITezBonusCarrier> onAddBonusObject, TezEventExtension.Action<ITezBonusCarrier> onRemoveBonusObject)
        {
            mOnAddBonusObject = onAddBonusObject;
            mOnRemoveBonusObject = onRemoveBonusObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void setPath(TezBonusPath path)
        {
            mBonusPath = path;
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="obj"></param>
        public void addBonusObject(ITezBonusCarrier obj)
        {
            mOnAddBonusObject(obj);
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="obj"></param>
        public void removeBonusObject(ITezBonusCarrier obj)
        {
            mOnRemoveBonusObject(obj);
        }

        public void close()
        {
            mBonusPath = null;
            mOnAddBonusObject = null;
            mOnRemoveBonusObject = null;
        }
    }
}