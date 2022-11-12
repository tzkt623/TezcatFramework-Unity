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
        TezBonusPath mBonusPath = null;
        public TezBonusPath bonusPath => mBonusPath;

        TezEventExtension.Action<ITezBonusObject> mOnAddBonusObject = null;
        TezEventExtension.Action<ITezBonusObject> mOnRemoveBonusObject = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onAddBonusObject"></param>
        /// <param name="onRemoveBonusObject"></param>
        public void setListener(TezEventExtension.Action<ITezBonusObject> onAddBonusObject, TezEventExtension.Action<ITezBonusObject> onRemoveBonusObject)
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
        public void addBonusObject(ITezBonusObject obj)
        {
            mOnAddBonusObject(obj);
        }

        /// <summary>
        /// 被动接收调用
        /// </summary>
        /// <param name="obj"></param>
        public void removeBonusObject(ITezBonusObject obj)
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