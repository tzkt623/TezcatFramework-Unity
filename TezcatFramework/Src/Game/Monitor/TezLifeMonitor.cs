using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public class TezLifeData
    {
        public bool valid = false;
        public object holdObject = null;
    }

    public interface ITezLifeDataHolder
    {
        TezLifeData metaData { get; }
    }

    public abstract class TezLifeDataHolder
        : ITezCloseable
        , ITezLifeDataHolder
    {
        protected TezLifeData mMetaData = null;

        TezLifeData ITezLifeDataHolder.metaData => mMetaData;
        public bool isValied => mMetaData.valid;
        public object holdObject => mMetaData.holdObject;

        public bool tryGetObject(out object result)
        {
            if (mMetaData.valid)
            {
                result = mMetaData.holdObject;
                return true;
            }

            result = null;
            return false;
        }

        public bool tryGetObject<T>(out T result) where T : class
        {
            if (mMetaData.valid)
            {
                result = (T)mMetaData.holdObject;
                return true;
            }

            result = null;
            return false;
        }

        public object getObject()
        {
            return mMetaData.holdObject;
        }

        public T getObject<T>() where T : class
        {
            return (T)mMetaData.holdObject;
        }

        public void close()
        {
            this.onClose();
        }

        protected abstract void onClose();
    }

    /// <summary>
    /// 
    /// 对象生命周期控制器
    /// 既可以监视又可以控制
    /// 可以赋予监视权
    /// 不能转移控制权
    /// 
    /// <para>
    /// 思路来源于智能指针
    /// 用于验证holdObject的有效性
    /// </para>
    /// 
    /// <para>
    /// 比如你发射了一颗导弹
    /// 导弹飞行途中目标已经爆了
    /// 这时你需要让导弹寻找下一个目标
    /// 或者给定一个虚空目标
    /// 或者原地爆炸
    /// 这个验证器便可以知道目标的状态
    /// 以判断是否需要操作
    /// </para>
    /// 
    /// </summary>
    public class TezLifeHolder : TezLifeDataHolder
    {
        public void create(object sourceObject)
        {
            if (mMetaData != null)
            {
                mMetaData.holdObject = sourceObject;
                mMetaData.valid = true;
            }
            else
            {
                mMetaData = new TezLifeData()
                {
                    holdObject = sourceObject,
                    valid = true
                };
            }
        }

        public void setInvalid()
        {
            mMetaData.valid = false;
        }

        public void setValid()
        {
            mMetaData.valid = true;
        }

        protected override void onClose()
        {
            mMetaData.holdObject = null;
            mMetaData.valid = false;
            mMetaData = null;
        }
    }

    /// <summary>
    /// 
    /// 对象生命周期监视器
    /// 只能监视生命周期,不能修改
    /// 可以赋予监视权
    /// 
    /// <para>
    /// 思路来源于智能指针
    /// 用于验证holdObject的有效性
    /// </para>
    /// 
    /// <para>
    /// 比如你发射了一颗导弹
    /// 导弹飞行途中目标已经爆了
    /// 这时你需要让导弹寻找下一个目标
    /// 或者给定一个虚空目标
    /// 或者原地爆炸
    /// 这个验证器便可以知道目标的状态
    /// 以判断是否需要操作
    /// </para>
    /// 
    /// </summary>
    public class TezLifeMonitor : TezLifeDataHolder
    {
        public void create(ITezLifeDataHolder info)
        {
            mMetaData = info.metaData;
        }

        protected override void onClose()
        {
            mMetaData = null;
        }
    }
}
