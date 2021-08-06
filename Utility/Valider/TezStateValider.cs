using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezStateValider : ITezCloseable
    {
        void addStateMask(uint mask);

        void removeStateMask(uint mask);

        void clearStateMask();

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_AnyOf(uint mask, out object userData);

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_AllOf(uint mask, out object userData);

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        bool tryGetUserData_NoneOf(uint mask, out object userData);
    }

    /// <summary>
    /// 带状态的验证器
    /// 
    /// 与普通验证器相同
    /// 只是可以携带32种状态(uint)进行验证
    /// 
    /// </summary>
    public class TezStateValider<UserData>
        : ITezStateValider
        where UserData : class, new()
    {
        class Data
        {
            int m_Ref = 0;
            uint m_StateMask = 0;
            public UserData userData;

            public Data() { }

            public void add(uint value)
            {
                m_StateMask |= value;
            }

            public void remove(uint value)
            {
                m_StateMask &= ~value;
            }

            public bool allOf(uint value)
            {
                return (m_StateMask & value) == value;
            }

            public bool anyOf(uint value)
            {
                return (m_StateMask & value) > 0;
            }

            public bool noneOf(uint value)
            {
                return (m_StateMask & value) == 0;
            }

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

            public void reset()
            {
                m_StateMask = 0;
            }
        }

        Data m_Valider = null;

        /// <summary>
        /// 创建一个新的Valider
        /// 以及新的共享数据
        /// </summary>
        public TezStateValider()
        {
            m_Valider = new Data();
            m_Valider.retain();
        }

        /// <summary>
        /// 从另一个Valider中获得共享数据
        /// 用于监视
        /// 不会新创建贡献数据
        /// </summary>
        public TezStateValider(TezStateValider<UserData> entry)
        {
            this.hold(entry);
        }

        /// <summary>
        /// 持有一个新的entry
        /// </summary>
        public void hold(TezStateValider<UserData> entry)
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
        public bool tryGetUserData_AnyOf(uint mask, out UserData userData)
        {
            if (m_Valider.anyOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_AllOf(uint mask, out UserData userData)
        {
            if (m_Valider.allOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 获取实际数据
        /// 如果数据失效
        /// 则获得null
        /// </summary>
        public bool tryGetUserData_NoneOf(uint mask, out UserData userData)
        {
            if (m_Valider.noneOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        /// <summary>
        /// 添加一种状态
        /// </summary>
        public void addStateMask(uint mask)
        {
            m_Valider.add(mask);
        }

        /// <summary>
        /// 移除一种状态
        /// </summary>
        public void removeStateMask(uint mask)
        {
            m_Valider.remove(mask);
        }

        /// <summary>
        /// 清空所有状态
        /// </summary>
        public void clearStateMask()
        {
            m_Valider.reset();
        }


        public void close()
        {
            m_Valider.release();
            m_Valider = null;
        }

        bool ITezStateValider.tryGetUserData_AllOf(uint mask, out object userData)
        {
            if (m_Valider.allOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        bool ITezStateValider.tryGetUserData_AnyOf(uint mask, out object userData)
        {
            if (m_Valider.anyOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }

        bool ITezStateValider.tryGetUserData_NoneOf(uint mask, out object userData)
        {
            if (m_Valider.noneOf(mask))
            {
                userData = m_Valider.userData;
                return true;
            }

            userData = null;
            return false;
        }
    }
}
