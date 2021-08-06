using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezValider : ITezCloseable
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        bool isValied { get; }

        /// <summary>
        /// 将内部共享数据
        /// 即UserData设置为失效状态
        /// 其他共享对象就无法使用其进行计算
        /// </summary>
        void setInvalid();

        void setValid();

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData(out object userData);
    }

    /// <summary>
    /// 有效验证器
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
    public class TezValider<UserData>
        : ITezValider
        where UserData : class, new()
    {
        class Data
        {
            int m_Ref = 0;
            public bool valid = false;
            public UserData userData;

            public Data() { }

            public void retain()
            {
                m_Ref++;
            }

            public void release()
            {
                m_Ref--;
                if (m_Ref == 0)
                {
                    this.userData = null;
                }
            }
        }

        Data m_Valider = null;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isValied => m_Valider.valid;

        /// <summary>
        /// 创建一个新的Valider
        /// 以及新的共享数据
        /// </summary>
        public TezValider()
        {
            m_Valider = new Data();
            m_Valider.retain();
        }

        /// <summary>
        /// 从另一个Valider中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezValider(TezValider<UserData> entry)
        {
            this.hold(entry);
        }

        /// <summary>
        /// 持有一个新的entry
        /// </summary>
        public void hold(TezValider<UserData> entry)
        {
            m_Valider?.release();
            m_Valider = entry.m_Valider;
            m_Valider.retain();
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData(out UserData userData)
        {
            if (m_Valider.valid)
            {
                userData = m_Valider.userData;
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
            m_Valider.valid = false;
        }

        public void setValid()
        {
            m_Valider.valid = true;
        }

        public void close()
        {
            m_Valider.release();
            m_Valider = null;
        }

        bool ITezValider.tryGetUserData(out object userData)
        {
            if (m_Valider.valid)
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }
    }




}
