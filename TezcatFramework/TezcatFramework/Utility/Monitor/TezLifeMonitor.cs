﻿using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /*
     * 生命周期探测器
     * 用于在运行时检测对象是否处于有效状态
     * 以便进行下一步操作
     * 
     * 有两种版本
     * 
     * 1.非模版类
     * 只有主对象有修改有效性的权限
     * Slot检测类不具有修改对象是否有效的权限
     * 
     * 2.模版类
     * 各个检测类都可以检测并修改对象的有效性
     * 
     * 
     */

    public interface ITezLifeMonitorEntry
    {
        TezLifeMonitor lifeMonitor { get; }
    }

    public class TezLifeMonitor : ITezCloseable
    {
        public class MetaData
        {
            public object managedObject;
        }

        MetaData mMetaData = new MetaData();
        public MetaData metaData => mMetaData;

        public void setManagedObject(object obj)
        {
            mMetaData.managedObject = obj;
        }

        public virtual void close()
        {
            mMetaData.managedObject = null;
            mMetaData = null;
        }
    }

    public class TezLifeMonitorSlot : ITezCloseable
    {
        private TezLifeMonitor.MetaData mMetaData = null;

        public bool isValied => mMetaData != null && mMetaData.managedObject != null;

        public TezLifeMonitorSlot(ITezLifeMonitorEntry entry)
        {
            mMetaData = entry.lifeMonitor.metaData;
        }

        public bool tryUse<T>(out T result) where T : class
        {
            if (mMetaData.managedObject != null)
            {
                result = (T)mMetaData.managedObject;
                return true;
            }

            result = null;
            return false;
        }

        public void close()
        {
            mMetaData = null;
        }
    }

    /// <summary>
    /// 对象生命周期探测器
    /// 
    /// <para>
    /// 思路来源于智能指针
    /// 用于验证UserData的有效性
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
    public class TezLifeMonitor<UserData> where UserData : class
    {
        class MetaData
        {
            int mRef = 0;
            public bool valid = false;
            public UserData userData = null;

            public MetaData() { }

            public void retain()
            {
                mRef++;
            }

            public void release()
            {
                mRef--;
                if (mRef == 0)
                {
                    this.userData = null;
                }
            }
        }

        MetaData mMetaData = null;

        /// <summary>
        /// 
        /// </summary>
        public UserData owner => mMetaData.userData;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isValied => mMetaData.valid;

        /// <summary>
        /// 创建一个新的Detector
        /// 以及新的共享数据
        /// </summary>
        public TezLifeMonitor(UserData userData)
        {
            mMetaData = new MetaData()
            {
                userData = userData,
                valid = userData == null ? false : true
            };
            mMetaData.retain();
        }

        /// <summary>
        /// 从另一个Detector中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezLifeMonitor(TezLifeMonitor<UserData> other)
        {
            this.hold(other);
        }

        /// <summary>
        /// 持有一个新的Detector
        /// </summary>
        public void hold(TezLifeMonitor<UserData> other)
        {
            mMetaData?.release();
            mMetaData = other.mMetaData;
            mMetaData.retain();
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData(out UserData userData)
        {
            if (mMetaData.valid)
            {
                userData = mMetaData.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 将内部共享数据
        /// 即UserData设置为失效状态
        /// 其他共享对象就无法使用其进行计算
        /// </summary>
        public void setInvalid()
        {
            mMetaData.valid = false;
        }

        public void setValid()
        {
            mMetaData.valid = true;
        }

        public void close()
        {
            mMetaData.release();
            mMetaData = null;
        }
    }
}
